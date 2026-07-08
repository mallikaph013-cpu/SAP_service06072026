/*
  Create SQL Server database for ITRepairService
  Update @DatabaseName if needed.
*/

DECLARE @DatabaseName sysname = N'ITRepairService';
DECLARE @Sql nvarchar(max);

IF DB_ID(@DatabaseName) IS NULL
BEGIN
    SET @Sql = N'CREATE DATABASE [' + @DatabaseName + N']';
    EXEC(@Sql);
END;
GO

PRINT N'Database is ready. Next step: run scripts/sqlserver-create.sql against the target database.';
GO
