/*
  Create SQL Server login + database user for ITRepairService app (idempotent)

  Usage:
  1) Update @LoginName, @LoginPassword, @DatabaseName as needed
  2) Run this script in SQL Server (master context is fine)
  3) Use the same credentials in appsettings.Production.json connection string
*/

SET NOCOUNT ON;

DECLARE @LoginName sysname = N'itservice_app';
DECLARE @LoginPassword nvarchar(128) = N'ChangeMe_StrongP@ssw0rd!';
DECLARE @DatabaseName sysname = N'ITService';
DECLARE @UserName sysname = N'itservice_app';

IF DB_ID(@DatabaseName) IS NULL
BEGIN
    RAISERROR(N'Database %s does not exist. Create database first.', 16, 1, @DatabaseName);
    RETURN;
END;

BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM sys.sql_logins WHERE name = @LoginName)
    BEGIN
        DECLARE @CreateLoginSql nvarchar(max) =
            N'CREATE LOGIN ' + QUOTENAME(@LoginName) +
            N' WITH PASSWORD = N''' + REPLACE(@LoginPassword, '''', '''''') + N''', CHECK_POLICY = ON, CHECK_EXPIRATION = OFF;';
        EXEC(@CreateLoginSql);
    END;

    DECLARE @CreateUserSql nvarchar(max) =
        N'USE ' + QUOTENAME(@DatabaseName) + N';
          IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N''' + REPLACE(@UserName, '''', '''''') + N''')
          BEGIN
              CREATE USER ' + QUOTENAME(@UserName) + N' FOR LOGIN ' + QUOTENAME(@LoginName) + N';
          END;

          IF NOT EXISTS (
              SELECT 1
              FROM sys.database_role_members drm
              INNER JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
              INNER JOIN sys.database_principals u ON drm.member_principal_id = u.principal_id
              WHERE r.name = N''db_datareader'' AND u.name = N''' + REPLACE(@UserName, '''', '''''') + N'''
          )
          BEGIN
              ALTER ROLE [db_datareader] ADD MEMBER ' + QUOTENAME(@UserName) + N';
          END;

          IF NOT EXISTS (
              SELECT 1
              FROM sys.database_role_members drm
              INNER JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
              INNER JOIN sys.database_principals u ON drm.member_principal_id = u.principal_id
              WHERE r.name = N''db_datawriter'' AND u.name = N''' + REPLACE(@UserName, '''', '''''') + N'''
          )
          BEGIN
              ALTER ROLE [db_datawriter] ADD MEMBER ' + QUOTENAME(@UserName) + N';
          END;';

    EXEC(@CreateUserSql);

    COMMIT TRANSACTION;
    PRINT N'Login/user provisioning completed successfully.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage nvarchar(4000) = ERROR_MESSAGE();
    DECLARE @ErrorLine int = ERROR_LINE();
    RAISERROR(N'Provisioning failed at line %d: %s', 16, 1, @ErrorLine, @ErrorMessage);
END CATCH;
GO
