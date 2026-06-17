-- ============================================================
-- Seed Data Script
-- SAP Service Application
-- Generated: 2026-04-16
-- Contains: Roles, MasterData, DocumentTypes, EF Migrations History
-- NOTE: Admin user is created at app startup by IdentityDataInitializer
-- ============================================================

USE [SAP_Service];
GO

-- ============================================================
-- SECTION 1: ROLES
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [NormalizedName] = N'ADMIN')
    INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
    VALUES (NEWID(), N'Admin', N'ADMIN', NEWID());
GO

IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [NormalizedName] = N'USER')
    INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
    VALUES (NEWID(), N'User', N'USER', NEWID());
GO

-- ============================================================
-- SECTION 2: MASTER DATA COMBINATIONS
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM [MasterDataCombinations])
BEGIN
    INSERT INTO [MasterDataCombinations] ([DepartmentName], [SectionName], [PlantName], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy])
    VALUES
        (N'HR',         N'Recruitment',     N'Main Office', GETUTCDATE(), GETUTCDATE(), N'System', N'System'),
        (N'HR',         N'Payroll',         N'Main Office', GETUTCDATE(), GETUTCDATE(), N'System', N'System'),
        (N'IT',         N'Support',         N'Main Office', GETUTCDATE(), GETUTCDATE(), N'System', N'System'),
        (N'IT',         N'Development',     N'Main Office', GETUTCDATE(), GETUTCDATE(), N'System', N'System'),
        (N'Production', N'Assembly Line 1', N'Factory A',   GETUTCDATE(), GETUTCDATE(), N'System', N'System'),
        (N'Production', N'Assembly Line 2', N'Factory B',   GETUTCDATE(), GETUTCDATE(), N'System', N'System');
END
GO

-- ============================================================
-- SECTION 3: DOCUMENT TYPES
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM [DocumentTypes])
BEGIN
    INSERT INTO [DocumentTypes] ([Name])
    VALUES
        (N'New Item Code'),
        (N'Edit BOM'),
        (N'Edit Routing'),
        (N'License Permission');
END
GO

-- ============================================================
-- SECTION 4: EF MIGRATIONS HISTORY
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260221134529_InitialCreate')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260221134529_InitialCreate', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260223065828_AddBomAndRoutingTables')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260223065828_AddBomAndRoutingTables', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260223092442_ConsolidateUser')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260223092442_ConsolidateUser', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260223094110_UpdateSchema')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260223094110_UpdateSchema', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260224093217_AddDocumentRouting')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260224093217_AddDocumentRouting', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260225023651_AddNextApproverIdToRequestItem')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260225023651_AddNextApproverIdToRequestItem', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260225050147_AddDepartmentAndSectionTables')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260225050147_AddDepartmentAndSectionTables', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260225065550_AddDocumentTypeIdToDocumentRoutings')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260225065550_AddDocumentTypeIdToDocumentRoutings', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260226015922_AddNewsArticle')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260226015922_AddNewsArticle', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260226040308_RenamePlantFGToPlant')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260226040308_RenamePlantFGToPlant', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260228021026_AddItemCodeForm')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260228021026_AddItemCodeForm', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260228034856_AddDescriptionFrom')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260228034856_AddDescriptionFrom', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260302040659_AddEditBomFgFields')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260302040659_AddEditBomFgFields', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260303011140_AddLicensePermissionItems')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260303011140_AddLicensePermissionItems', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260305052442_AddAuditLogs')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260305052442_AddAuditLogs', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260306041659_AddRequestItemUpdatedMetadata')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260306041659_AddRequestItemUpdatedMetadata', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260306042203_AddRequestItemUsageStatusSoftDelete')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260306042203_AddRequestItemUsageStatusSoftDelete', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260306052640_AddAuditFieldsToManagementEntitiesV2')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260306052640_AddAuditFieldsToManagementEntitiesV2', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260309040523_AddRoutingAdditionalFields')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260309040523_AddRoutingAdditionalFields', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260313055109_AddMustChangePasswordOnFirstLogin')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260313055109_AddMustChangePasswordOnFirstLogin', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260313071437_AddSectionIsActiveStatus')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260313071437_AddSectionIsActiveStatus', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260313072205_AddDepartmentIsActiveStatus')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260313072205_AddDepartmentIsActiveStatus', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260316015952_AddRequestAttachmentFields')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260316015952_AddRequestAttachmentFields', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260316020925_AddRequestDocumentNumber')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260316020925_AddRequestDocumentNumber', N'9.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260325090514_AddNewsAttachment')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260325090514_AddNewsAttachment', N'9.0.0');
GO

PRINT 'Seed data inserted successfully.';
GO
