/*
  Drop all ITRepairService tables (SQL Server)
  Usage: run against target database
*/

SET NOCOUNT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.RepairTicketStatusHistories', N'U') IS NOT NULL DROP TABLE dbo.RepairTicketStatusHistories;
    IF OBJECT_ID(N'dbo.RepairTickets', N'U') IS NOT NULL DROP TABLE dbo.RepairTickets;
    IF OBJECT_ID(N'dbo.NewsItems', N'U') IS NOT NULL DROP TABLE dbo.NewsItems;

    IF OBJECT_ID(N'dbo.AspNetUserTokens', N'U') IS NOT NULL DROP TABLE dbo.AspNetUserTokens;
    IF OBJECT_ID(N'dbo.AspNetUserRoles', N'U') IS NOT NULL DROP TABLE dbo.AspNetUserRoles;
    IF OBJECT_ID(N'dbo.AspNetUserLogins', N'U') IS NOT NULL DROP TABLE dbo.AspNetUserLogins;
    IF OBJECT_ID(N'dbo.AspNetUserClaims', N'U') IS NOT NULL DROP TABLE dbo.AspNetUserClaims;
    IF OBJECT_ID(N'dbo.AspNetRoleClaims', N'U') IS NOT NULL DROP TABLE dbo.AspNetRoleClaims;
    IF OBJECT_ID(N'dbo.AspNetUsers', N'U') IS NOT NULL DROP TABLE dbo.AspNetUsers;
    IF OBJECT_ID(N'dbo.AspNetRoles', N'U') IS NOT NULL DROP TABLE dbo.AspNetRoles;

    IF OBJECT_ID(N'dbo.__EFMigrationsHistory', N'U') IS NOT NULL DROP TABLE dbo.__EFMigrationsHistory;

    COMMIT TRANSACTION;
    PRINT N'Drop tables completed successfully.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage nvarchar(4000) = ERROR_MESSAGE();
    DECLARE @ErrorLine int = ERROR_LINE();

    RAISERROR(N'Drop tables failed at line %d: %s', 16, 1, @ErrorLine, @ErrorMessage);
END CATCH;
GO
