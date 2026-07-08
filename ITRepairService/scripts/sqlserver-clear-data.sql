/*
  Clear all data in ITRepairService tables (SQL Server)
  - Deletes all rows but keeps table structure intact
  - Resets identity columns back to 0
  - Safe to run multiple times (idempotent)

  Usage: run against target ITRepairService database
  NOTE: This does NOT drop tables or touch __EFMigrationsHistory
*/

SET NOCOUNT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    /* ── 1. ลบข้อมูลที่มี foreign key ก่อน ─────────────────────── */

    /* RepairTicketStatusHistories อ้างอิง RepairTickets */
    IF OBJECT_ID(N'dbo.RepairTicketStatusHistories', N'U') IS NOT NULL
    BEGIN
        DELETE FROM dbo.RepairTicketStatusHistories;
        IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE OBJECT_NAME(object_id) = 'RepairTicketStatusHistories')
            DBCC CHECKIDENT (N'dbo.RepairTicketStatusHistories', RESEED, 0);
    END;

    /* RepairTickets */
    IF OBJECT_ID(N'dbo.RepairTickets', N'U') IS NOT NULL
    BEGIN
        DELETE FROM dbo.RepairTickets;
        IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE OBJECT_NAME(object_id) = 'RepairTickets')
            DBCC CHECKIDENT (N'dbo.RepairTickets', RESEED, 0);
    END;

    /* NewsItems */
    IF OBJECT_ID(N'dbo.NewsItems', N'U') IS NOT NULL
    BEGIN
        DELETE FROM dbo.NewsItems;
        IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE OBJECT_NAME(object_id) = 'NewsItems')
            DBCC CHECKIDENT (N'dbo.NewsItems', RESEED, 0);
    END;

    /* ── 2. ลบข้อมูล Identity (ลบ link tables ก่อน) ─────────────── */

    IF OBJECT_ID(N'dbo.AspNetUserTokens', N'U') IS NOT NULL
        DELETE FROM dbo.AspNetUserTokens;

    IF OBJECT_ID(N'dbo.AspNetUserLogins', N'U') IS NOT NULL
        DELETE FROM dbo.AspNetUserLogins;

    IF OBJECT_ID(N'dbo.AspNetUserClaims', N'U') IS NOT NULL
        DELETE FROM dbo.AspNetUserClaims;

    IF OBJECT_ID(N'dbo.AspNetRoleClaims', N'U') IS NOT NULL
        DELETE FROM dbo.AspNetRoleClaims;

    IF OBJECT_ID(N'dbo.AspNetUserRoles', N'U') IS NOT NULL
        DELETE FROM dbo.AspNetUserRoles;

    IF OBJECT_ID(N'dbo.AspNetUsers', N'U') IS NOT NULL
        DELETE FROM dbo.AspNetUsers;

    IF OBJECT_ID(N'dbo.AspNetRoles', N'U') IS NOT NULL
        DELETE FROM dbo.AspNetRoles;

    COMMIT TRANSACTION;
    PRINT N'Clear data completed. Table structure and __EFMigrationsHistory are untouched.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage nvarchar(4000) = ERROR_MESSAGE();
    DECLARE @ErrorLine int = ERROR_LINE();

    RAISERROR(N'Clear data failed at line %d: %s', 16, 1, @ErrorLine, @ErrorMessage);
END CATCH;
GO
