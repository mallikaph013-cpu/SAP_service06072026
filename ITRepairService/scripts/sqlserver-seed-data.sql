/*
  Seed data for ITRepairService (SQL Server)
  - Safe to run multiple times (idempotent)
  - Run after schema script: scripts/sqlserver-create.sql
*/

SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.AspNetRoles', N'U') IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE NormalizedName = N'USER')
            INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
            VALUES (N'4f4e2f7e-9f2b-4b2f-b2f7-000000000001', N'User', N'USER', NEWID());

        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE NormalizedName = N'ITSUPPORT')
            INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
            VALUES (N'4f4e2f7e-9f2b-4b2f-b2f7-000000000002', N'ITSupport', N'ITSUPPORT', NEWID());

        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE NormalizedName = N'APPROVE')
            INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
            VALUES (N'4f4e2f7e-9f2b-4b2f-b2f7-000000000003', N'Approve', N'APPROVE', NEWID());

        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE NormalizedName = N'ADMIN')
            INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
            VALUES (N'4f4e2f7e-9f2b-4b2f-b2f7-000000000004', N'Admin', N'ADMIN', NEWID());
    END;

    IF OBJECT_ID(N'dbo.NewsItems', N'U') IS NOT NULL
    BEGIN
        DECLARE @HasAttachmentUrl bit = CASE WHEN COL_LENGTH('dbo.NewsItems', 'AttachmentUrl') IS NOT NULL THEN 1 ELSE 0 END;
        DECLARE @HasAttachmentFileName bit = CASE WHEN COL_LENGTH('dbo.NewsItems', 'AttachmentFileName') IS NOT NULL THEN 1 ELSE 0 END;
        DECLARE @HasIsActive bit = CASE WHEN COL_LENGTH('dbo.NewsItems', 'IsActive') IS NOT NULL THEN 1 ELSE 0 END;
        DECLARE @HasCreatedAt bit = CASE WHEN COL_LENGTH('dbo.NewsItems', 'CreatedAt') IS NOT NULL THEN 1 ELSE 0 END;
        DECLARE @HasCreatedByName bit = CASE WHEN COL_LENGTH('dbo.NewsItems', 'CreatedByName') IS NOT NULL THEN 1 ELSE 0 END;
        DECLARE @HasUpdatedAt bit = CASE WHEN COL_LENGTH('dbo.NewsItems', 'UpdatedAt') IS NOT NULL THEN 1 ELSE 0 END;
        DECLARE @HasUpdatedByName bit = CASE WHEN COL_LENGTH('dbo.NewsItems', 'UpdatedByName') IS NOT NULL THEN 1 ELSE 0 END;
        DECLARE @ActorName nvarchar(120) = N'System Administrator';
        DECLARE @Sql nvarchar(max);

        IF NOT EXISTS (SELECT 1 FROM dbo.NewsItems WHERE Title = N'System Launch Announcement')
        BEGIN
            SET @Sql = N'INSERT INTO dbo.NewsItems (Title, Content';
            SET @Sql += CASE WHEN @HasAttachmentUrl = 1 THEN N', AttachmentUrl' ELSE N'' END;
            SET @Sql += CASE WHEN @HasAttachmentFileName = 1 THEN N', AttachmentFileName' ELSE N'' END;
            SET @Sql += CASE WHEN @HasIsActive = 1 THEN N', IsActive' ELSE N'' END;
            SET @Sql += CASE WHEN @HasCreatedAt = 1 THEN N', CreatedAt' ELSE N'' END;
            SET @Sql += CASE WHEN @HasCreatedByName = 1 THEN N', CreatedByName' ELSE N'' END;
            SET @Sql += CASE WHEN @HasUpdatedAt = 1 THEN N', UpdatedAt' ELSE N'' END;
            SET @Sql += CASE WHEN @HasUpdatedByName = 1 THEN N', UpdatedByName' ELSE N'' END;
            SET @Sql += N') VALUES (@Title, @Content';
            SET @Sql += CASE WHEN @HasAttachmentUrl = 1 THEN N', NULL' ELSE N'' END;
            SET @Sql += CASE WHEN @HasAttachmentFileName = 1 THEN N', NULL' ELSE N'' END;
            SET @Sql += CASE WHEN @HasIsActive = 1 THEN N', 1' ELSE N'' END;
            SET @Sql += CASE WHEN @HasCreatedAt = 1 THEN N', SYSUTCDATETIME()' ELSE N'' END;
            SET @Sql += CASE WHEN @HasCreatedByName = 1 THEN N', @ActorName' ELSE N'' END;
            SET @Sql += CASE WHEN @HasUpdatedAt = 1 THEN N', SYSUTCDATETIME()' ELSE N'' END;
            SET @Sql += CASE WHEN @HasUpdatedByName = 1 THEN N', @ActorName' ELSE N'' END;
            SET @Sql += N');';

            EXEC sp_executesql
                @Sql,
                N'@Title nvarchar(160), @Content nvarchar(4000), @ActorName nvarchar(120)',
                @Title = N'System Launch Announcement',
                @Content = N'Welcome to IT Repair Service. Please submit requests via the Repair Tickets menu.',
                @ActorName = @ActorName;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.NewsItems WHERE Title = N'Maintenance Window Notice')
        BEGIN
            SET @Sql = N'INSERT INTO dbo.NewsItems (Title, Content';
            SET @Sql += CASE WHEN @HasAttachmentUrl = 1 THEN N', AttachmentUrl' ELSE N'' END;
            SET @Sql += CASE WHEN @HasAttachmentFileName = 1 THEN N', AttachmentFileName' ELSE N'' END;
            SET @Sql += CASE WHEN @HasIsActive = 1 THEN N', IsActive' ELSE N'' END;
            SET @Sql += CASE WHEN @HasCreatedAt = 1 THEN N', CreatedAt' ELSE N'' END;
            SET @Sql += CASE WHEN @HasCreatedByName = 1 THEN N', CreatedByName' ELSE N'' END;
            SET @Sql += CASE WHEN @HasUpdatedAt = 1 THEN N', UpdatedAt' ELSE N'' END;
            SET @Sql += CASE WHEN @HasUpdatedByName = 1 THEN N', UpdatedByName' ELSE N'' END;
            SET @Sql += N') VALUES (@Title, @Content';
            SET @Sql += CASE WHEN @HasAttachmentUrl = 1 THEN N', NULL' ELSE N'' END;
            SET @Sql += CASE WHEN @HasAttachmentFileName = 1 THEN N', NULL' ELSE N'' END;
            SET @Sql += CASE WHEN @HasIsActive = 1 THEN N', 1' ELSE N'' END;
            SET @Sql += CASE WHEN @HasCreatedAt = 1 THEN N', SYSUTCDATETIME()' ELSE N'' END;
            SET @Sql += CASE WHEN @HasCreatedByName = 1 THEN N', @ActorName' ELSE N'' END;
            SET @Sql += CASE WHEN @HasUpdatedAt = 1 THEN N', SYSUTCDATETIME()' ELSE N'' END;
            SET @Sql += CASE WHEN @HasUpdatedByName = 1 THEN N', @ActorName' ELSE N'' END;
            SET @Sql += N');';

            EXEC sp_executesql
                @Sql,
                N'@Title nvarchar(160), @Content nvarchar(4000), @ActorName nvarchar(120)',
                @Title = N'Maintenance Window Notice',
                @Content = N'Planned maintenance is scheduled every Saturday from 22:00 to 23:00.',
                @ActorName = @ActorName;
        END;
    END;

    COMMIT TRANSACTION;
    PRINT N'Seed data completed successfully.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage nvarchar(4000) = ERROR_MESSAGE();
    DECLARE @ErrorLine int = ERROR_LINE();

    RAISERROR(N'Seed failed at line %d: %s', 16, 1, @ErrorLine, @ErrorMessage);
END CATCH;
GO
