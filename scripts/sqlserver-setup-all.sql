-- ============================================================
-- SQL Server Complete Setup Script
-- SAP Service Application
-- Generated: 2026-04-16
-- Contains: Schema (CREATE TABLE) + Seed Data (INSERT)
-- ============================================================

USE master;
GO

-- ============================================================
-- SECTION 1: CREATE DATABASE (if not exists)
-- ============================================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'SAP_Service')
BEGIN
    CREATE DATABASE [SAP_Service];
END
GO

USE [SAP_Service];
GO

-- ============================================================
-- SECTION 2: SCHEMA - CREATE TABLES
-- ============================================================

-- EF Migrations History
IF OBJECT_ID(N'[__EFMigrationsHistory]', 'U') IS NULL
CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId]    NVARCHAR(150) NOT NULL,
    [ProductVersion] NVARCHAR(32)  NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
GO

-- AdminPasswordLog (for seeded admin password tracking)
IF OBJECT_ID(N'[AdminPasswordLog]', 'U') IS NULL
CREATE TABLE [AdminPasswordLog] (
    [Id]        INT IDENTITY(1,1) PRIMARY KEY,
    [Username]  NVARCHAR(256) NULL,
    [Password]  NVARCHAR(256) NULL,
    [CreatedAt] DATETIME NULL
);
GO

-- AspNet Identity Tables
IF OBJECT_ID(N'[AspNetRoles]', 'U') IS NULL
CREATE TABLE [AspNetRoles] (
    [Id]               NVARCHAR(450) NOT NULL,
    [Name]             NVARCHAR(256) NULL,
    [NormalizedName]   NVARCHAR(256) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

IF OBJECT_ID(N'[AspNetUsers]', 'U') IS NULL
CREATE TABLE [AspNetUsers] (
    [Id]                              NVARCHAR(450) NOT NULL,
    [FirstName]                       NVARCHAR(MAX) NULL,
    [LastName]                        NVARCHAR(MAX) NULL,
    [Department]                      NVARCHAR(MAX) NULL,
    [Section]                         NVARCHAR(MAX) NULL,
    [Plant]                           NVARCHAR(MAX) NULL,
    [IsActive]                        BIT           NOT NULL DEFAULT 1,
    [IsIT]                            BIT           NOT NULL DEFAULT 0,
    [MustChangePasswordOnFirstLogin]  BIT           NOT NULL DEFAULT 0,
    [CreatedAt]                       DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]                       DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]                       NVARCHAR(MAX) NULL,
    [UpdatedBy]                       NVARCHAR(MAX) NULL,
    [UserName]                        NVARCHAR(256) NULL,
    [NormalizedUserName]              NVARCHAR(256) NULL,
    [Email]                           NVARCHAR(256) NULL,
    [NormalizedEmail]                 NVARCHAR(256) NULL,
    [EmailConfirmed]                  BIT           NOT NULL DEFAULT 0,
    [PasswordHash]                    NVARCHAR(MAX) NULL,
    [SecurityStamp]                   NVARCHAR(MAX) NULL,
    [ConcurrencyStamp]                NVARCHAR(MAX) NULL,
    [PhoneNumber]                     NVARCHAR(MAX) NULL,
    [PhoneNumberConfirmed]            BIT           NOT NULL DEFAULT 0,
    [TwoFactorEnabled]                BIT           NOT NULL DEFAULT 0,
    [LockoutEnd]                      DATETIMEOFFSET NULL,
    [LockoutEnabled]                  BIT           NOT NULL DEFAULT 1,
    [AccessFailedCount]               INT           NOT NULL DEFAULT 0,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

IF OBJECT_ID(N'[AspNetRoleClaims]', 'U') IS NULL
CREATE TABLE [AspNetRoleClaims] (
    [Id]         INT IDENTITY(1,1) NOT NULL,
    [RoleId]     NVARCHAR(450) NOT NULL,
    [ClaimType]  NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
        FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

IF OBJECT_ID(N'[AspNetUserClaims]', 'U') IS NULL
CREATE TABLE [AspNetUserClaims] (
    [Id]         INT IDENTITY(1,1) NOT NULL,
    [UserId]     NVARCHAR(450) NOT NULL,
    [ClaimType]  NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
        FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

IF OBJECT_ID(N'[AspNetUserLogins]', 'U') IS NULL
CREATE TABLE [AspNetUserLogins] (
    [LoginProvider]       NVARCHAR(450) NOT NULL,
    [ProviderKey]         NVARCHAR(450) NOT NULL,
    [ProviderDisplayName] NVARCHAR(MAX) NULL,
    [UserId]              NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
        FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

IF OBJECT_ID(N'[AspNetUserRoles]', 'U') IS NULL
CREATE TABLE [AspNetUserRoles] (
    [UserId] NVARCHAR(450) NOT NULL,
    [RoleId] NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
        FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
        FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

IF OBJECT_ID(N'[AspNetUserTokens]', 'U') IS NULL
CREATE TABLE [AspNetUserTokens] (
    [UserId]        NVARCHAR(450) NOT NULL,
    [LoginProvider] NVARCHAR(450) NOT NULL,
    [Name]          NVARCHAR(450) NOT NULL,
    [Value]         NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
        FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

-- Application Tables
IF OBJECT_ID(N'[Departments]', 'U') IS NULL
CREATE TABLE [Departments] (
    [DepartmentId]   INT IDENTITY(1,1) NOT NULL,
    [DepartmentName] NVARCHAR(MAX)     NOT NULL,
    [IsActive]       BIT               NOT NULL DEFAULT 1,
    [CreatedAt]      DATETIME2         NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]      DATETIME2         NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]      NVARCHAR(256)     NOT NULL DEFAULT '',
    [UpdatedBy]      NVARCHAR(256)     NOT NULL DEFAULT '',
    CONSTRAINT [PK_Departments] PRIMARY KEY ([DepartmentId])
);
GO

IF OBJECT_ID(N'[Sections]', 'U') IS NULL
CREATE TABLE [Sections] (
    [SectionId]    INT IDENTITY(1,1) NOT NULL,
    [SectionName]  NVARCHAR(MAX)     NOT NULL,
    [DepartmentId] INT               NOT NULL,
    [IsActive]     BIT               NOT NULL DEFAULT 1,
    [CreatedAt]    DATETIME2         NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]    DATETIME2         NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]    NVARCHAR(256)     NOT NULL DEFAULT '',
    [UpdatedBy]    NVARCHAR(256)     NOT NULL DEFAULT '',
    CONSTRAINT [PK_Sections] PRIMARY KEY ([SectionId]),
    CONSTRAINT [FK_Sections_Departments_DepartmentId]
        FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([DepartmentId]) ON DELETE CASCADE
);
GO

IF OBJECT_ID(N'[Plants]', 'U') IS NULL
CREATE TABLE [Plants] (
    [PlantId]      INT IDENTITY(1,1) NOT NULL,
    [PlantName]    NVARCHAR(MAX)     NOT NULL,
    [DepartmentId] INT               NOT NULL,
    CONSTRAINT [PK_Plants] PRIMARY KEY ([PlantId]),
    CONSTRAINT [FK_Plants_Departments_DepartmentId]
        FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([DepartmentId]) ON DELETE CASCADE
);
GO

IF OBJECT_ID(N'[DocumentTypes]', 'U') IS NULL
CREATE TABLE [DocumentTypes] (
    [DocumentTypeId] INT IDENTITY(1,1) NOT NULL,
    [Name]           NVARCHAR(100)     NOT NULL,
    CONSTRAINT [PK_DocumentTypes] PRIMARY KEY ([DocumentTypeId])
);
GO

IF OBJECT_ID(N'[DocumentRoutings]', 'U') IS NULL
CREATE TABLE [DocumentRoutings] (
    [Id]             INT IDENTITY(1,1) NOT NULL,
    [DocumentTypeId] INT               NOT NULL,
    [DepartmentId]   INT               NOT NULL,
    [SectionId]      INT               NOT NULL,
    [PlantId]        INT               NOT NULL,
    [Step]           INT               NOT NULL,
    [CreatedAt]      DATETIME2         NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]      DATETIME2         NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]      NVARCHAR(256)     NOT NULL DEFAULT '',
    [UpdatedBy]      NVARCHAR(256)     NOT NULL DEFAULT '',
    CONSTRAINT [PK_DocumentRoutings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DocumentRoutings_DocumentTypes_DocumentTypeId]
        FOREIGN KEY ([DocumentTypeId]) REFERENCES [DocumentTypes] ([DocumentTypeId]),
    CONSTRAINT [FK_DocumentRoutings_Departments_DepartmentId]
        FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([DepartmentId]),
    CONSTRAINT [FK_DocumentRoutings_Sections_SectionId]
        FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([SectionId]),
    CONSTRAINT [FK_DocumentRoutings_Plants_PlantId]
        FOREIGN KEY ([PlantId]) REFERENCES [Plants] ([PlantId])
);
GO

IF OBJECT_ID(N'[MasterDataCombinations]', 'U') IS NULL
CREATE TABLE [MasterDataCombinations] (
    [Id]             INT IDENTITY(1,1) NOT NULL,
    [DepartmentName] NVARCHAR(MAX)     NOT NULL,
    [SectionName]    NVARCHAR(MAX)     NOT NULL,
    [PlantName]      NVARCHAR(MAX)     NOT NULL,
    [CreatedAt]      DATETIME2         NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]      DATETIME2         NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]      NVARCHAR(MAX)     NOT NULL DEFAULT '',
    [UpdatedBy]      NVARCHAR(MAX)     NOT NULL DEFAULT '',
    CONSTRAINT [PK_MasterDataCombinations] PRIMARY KEY ([Id])
);
GO

IF OBJECT_ID(N'[RequestItems]', 'U') IS NULL
CREATE TABLE [RequestItems] (
    [Id]                     INT IDENTITY(1,1)  NOT NULL,
    [RequestType]            NVARCHAR(MAX)       NOT NULL,
    [Description]            NVARCHAR(MAX)       NOT NULL,
    [Requester]              NVARCHAR(MAX)       NOT NULL,
    [Status]                 NVARCHAR(MAX)       NOT NULL DEFAULT N'Pending',
    [UsageStatus]            INT                 NOT NULL DEFAULT 1,
    [RequestDate]            DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]              DATETIME2           NULL,
    [UpdatedBy]              NVARCHAR(256)       NULL,
    [NextApproverId]         NVARCHAR(MAX)       NULL,
    [AttachmentFileName]     NVARCHAR(260)       NULL,
    [AttachmentPath]         NVARCHAR(500)       NULL,
    [DocumentNumber]         NVARCHAR(20)        NULL,
    [Plant]                  NVARCHAR(MAX)       NULL,
    [ItemCode]               NVARCHAR(MAX)       NULL,
    [EnglishMatDescription]  NVARCHAR(MAX)       NULL,
    [ModelName]              NVARCHAR(MAX)       NULL,
    [BaseUnit]               NVARCHAR(MAX)       NULL,
    [MaterialGroup]          NVARCHAR(MAX)       NULL,
    [ExternalMaterialGroup]  NVARCHAR(MAX)       NULL,
    [DivisionCode]           NVARCHAR(MAX)       NULL,
    [ProfitCenter]           NVARCHAR(MAX)       NULL,
    [DistributionChannel]    NVARCHAR(MAX)       NULL,
    [BoiCode]                NVARCHAR(MAX)       NULL,
    [BoiDescription]         NVARCHAR(MAX)       NULL,
    [MrpController]          NVARCHAR(MAX)       NULL,
    [StorageLocation]        NVARCHAR(MAX)       NULL,
    [ProductionSupervisor]   NVARCHAR(MAX)       NULL,
    [CostingLotSize]         INT                 NULL,
    [ValClass]               NVARCHAR(MAX)       NULL,
    [StandardPack]           NVARCHAR(MAX)       NULL,
    [MakerMfrPartNumber]     NVARCHAR(MAX)       NULL,
    [CommCodeTariffCode]     NVARCHAR(MAX)       NULL,
    [TraffCodePercentage]    DECIMAL(18, 2)      NULL,
    [StorageLocationB1]      NVARCHAR(MAX)       NULL,
    [PriceControl]           NVARCHAR(MAX)       NULL,
    [Currency]               NVARCHAR(MAX)       NULL,
    [SupplierCode]           NVARCHAR(MAX)       NULL,
    [MatType]                NVARCHAR(MAX)       NULL,
    [Check]                  BIT                 NOT NULL DEFAULT 0,
    [DevicePlant]            NVARCHAR(MAX)       NULL,
    [AssemblyPlant]          NVARCHAR(MAX)       NULL,
    [IpoPlant]               NVARCHAR(MAX)       NULL,
    [AsiOfPlant]             NVARCHAR(MAX)       NULL,
    [PriceUnit]              INT                 NULL,
    [StorageLocationEP]      NVARCHAR(MAX)       NULL,
    [ToolingBSection]        NVARCHAR(MAX)       NULL,
    [PoNumber]               NVARCHAR(MAX)       NULL,
    [StatusInA]              NVARCHAR(MAX)       NULL,
    [DateIn]                 DATETIME2           NULL,
    [QuotationNumber]        NVARCHAR(MAX)       NULL,
    [ToolingBModel]          NVARCHAR(MAX)       NULL,
    [TariffCode]             NVARCHAR(MAX)       NULL,
    [Planner]                NVARCHAR(MAX)       NULL,
    [CurrentICS]             NVARCHAR(MAX)       NULL,
    [Level]                  NVARCHAR(MAX)       NULL,
    [Rohs]                   NVARCHAR(MAX)       NULL,
    [CodenMid]               NVARCHAR(MAX)       NULL,
    [SalesOrg]               NVARCHAR(MAX)       NULL,
    [TaxTh]                  NVARCHAR(MAX)       NULL,
    [MaterialStatisticsGroup] NVARCHAR(MAX)      NULL,
    [AccountAssignment]      NVARCHAR(MAX)       NULL,
    [GeneralItemCategory]    NVARCHAR(MAX)       NULL,
    [Availability]           NVARCHAR(MAX)       NULL,
    [Transportation]         NVARCHAR(MAX)       NULL,
    [LoadingGroup]           NVARCHAR(MAX)       NULL,
    [PlanDelTime]            NVARCHAR(MAX)       NULL,
    [SchedMargin]            NVARCHAR(MAX)       NULL,
    [MinLot]                 NVARCHAR(MAX)       NULL,
    [MaxLot]                 NVARCHAR(MAX)       NULL,
    [FixedLot]               NVARCHAR(MAX)       NULL,
    [Rounding]               NVARCHAR(MAX)       NULL,
    [Mtlsm]                  NVARCHAR(MAX)       NULL,
    [Effective]              DATETIME2           NULL,
    [StorageLoc]             NVARCHAR(MAX)       NULL,
    [ReceiveStorage]         NVARCHAR(MAX)       NULL,
    [PurchasingGroup]        NVARCHAR(MAX)       NULL,
    [EditBomFg]              NVARCHAR(MAX)       NULL,
    [EditBomAllFg]           BIT                 NOT NULL DEFAULT 0,
    [Price]                  DECIMAL(18, 2)      NULL,
    CONSTRAINT [PK_RequestItems] PRIMARY KEY ([Id])
);
GO

IF OBJECT_ID(N'[BomComponents]', 'U') IS NULL
CREATE TABLE [BomComponents] (
    [Id]            INT IDENTITY(1,1) NOT NULL,
    [RequestItemId] INT               NOT NULL,
    [Level]         INT               NOT NULL DEFAULT 0,
    [Item]          NVARCHAR(MAX)     NULL,
    [ItemCat]       NVARCHAR(MAX)     NULL,
    [ComponentNumber] NVARCHAR(MAX)   NULL,
    [Description]   NVARCHAR(MAX)     NULL,
    [ItemQuantity]  DECIMAL(18, 5)    NULL,
    [Unit]          NVARCHAR(MAX)     NULL,
    [BomUsage]      NVARCHAR(MAX)     NULL,
    [Plant]         NVARCHAR(MAX)     NULL,
    [Sloc]          NVARCHAR(MAX)     NULL,
    CONSTRAINT [PK_BomComponents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BomComponents_RequestItems_RequestItemId]
        FOREIGN KEY ([RequestItemId]) REFERENCES [RequestItems] ([Id]) ON DELETE CASCADE
);
GO

IF OBJECT_ID(N'[BomEditComponent]', 'U') IS NULL
CREATE TABLE [BomEditComponent] (
    [Id]               INT IDENTITY(1,1) NOT NULL,
    [RequestItemId]    INT               NULL,
    [ItemCodeFrom]     NVARCHAR(MAX)     NULL,
    [ItemCodeTo]       NVARCHAR(MAX)     NULL,
    [DescriptionFrom]  NVARCHAR(MAX)     NULL,
    [DescriptionTo]    NVARCHAR(MAX)     NULL,
    [ItemQuantityFrom] DECIMAL(18, 5)    NULL,
    [ItemQuantityTo]   DECIMAL(18, 5)    NULL,
    [UnitFrom]         NVARCHAR(MAX)     NULL,
    [UnitTo]           NVARCHAR(MAX)     NULL,
    [BomUsageFrom]     NVARCHAR(MAX)     NULL,
    [BomUsageTo]       NVARCHAR(MAX)     NULL,
    [PlantTo]          NVARCHAR(MAX)     NULL,
    [SlocFrom]         NVARCHAR(MAX)     NULL,
    [SlocTo]           NVARCHAR(MAX)     NULL,
    CONSTRAINT [PK_BomEditComponent] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BomEditComponent_RequestItems_RequestItemId]
        FOREIGN KEY ([RequestItemId]) REFERENCES [RequestItems] ([Id])
);
GO

IF OBJECT_ID(N'[Routings]', 'U') IS NULL
CREATE TABLE [Routings] (
    [Id]                    INT IDENTITY(1,1) NOT NULL,
    [RequestItemId]         INT               NOT NULL,
    [Material]              NVARCHAR(MAX)     NULL,
    [Description]           NVARCHAR(MAX)     NULL,
    [Counter]               NVARCHAR(MAX)     NULL,
    [Plant]                 NVARCHAR(MAX)     NULL,
    [WorkCenter]            NVARCHAR(MAX)     NULL,
    [Operation]             NVARCHAR(MAX)     NULL,
    [BaseQty]               DECIMAL(18, 5)    NULL,
    [Unit]                  NVARCHAR(MAX)     NULL,
    [DirectLaborCosts]      DECIMAL(18, 5)    NULL,
    [DirectExpenses]        DECIMAL(18, 5)    NULL,
    [AllocationExpense]     DECIMAL(18, 5)    NULL,
    [ProductionVersionCode] NVARCHAR(MAX)     NULL,
    [Version]               NVARCHAR(MAX)     NULL,
    [ValidFrom]             DATETIME2         NULL,
    [ValidTo]               DATETIME2         NULL,
    [MaximumLotSize]        DECIMAL(18, 5)    NULL,
    [Alternative]           NVARCHAR(MAX)     NULL,
    [BomUsage]              NVARCHAR(MAX)     NULL,
    [Group]                 NVARCHAR(MAX)     NULL,
    [GroupCounter]          NVARCHAR(MAX)     NULL,
    CONSTRAINT [PK_Routings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Routings_RequestItems_RequestItemId]
        FOREIGN KEY ([RequestItemId]) REFERENCES [RequestItems] ([Id]) ON DELETE CASCADE
);
GO

IF OBJECT_ID(N'[LicensePermissionItems]', 'U') IS NULL
CREATE TABLE [LicensePermissionItems] (
    [Id]            INT IDENTITY(1,1) NOT NULL,
    [RequestItemId] INT               NOT NULL,
    [SapUsername]   NVARCHAR(MAX)     NULL,
    [TCode]         NVARCHAR(MAX)     NULL,
    CONSTRAINT [PK_LicensePermissionItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LicensePermissionItems_RequestItems_RequestItemId]
        FOREIGN KEY ([RequestItemId]) REFERENCES [RequestItems] ([Id]) ON DELETE CASCADE
);
GO

IF OBJECT_ID(N'[NewsArticles]', 'U') IS NULL
CREATE TABLE [NewsArticles] (
    [Id]            INT IDENTITY(1,1) NOT NULL,
    [Title]         NVARCHAR(200)     NOT NULL,
    [Content]       NVARCHAR(MAX)     NOT NULL,
    [ImageUrl]      NVARCHAR(500)     NULL,
    [PublishedDate] DATETIME2         NOT NULL DEFAULT GETUTCDATE(),
    [Author]        NVARCHAR(100)     NOT NULL DEFAULT '',
    [IsFeatured]    BIT               NOT NULL DEFAULT 0,
    [CreatedAt]     DATETIME2         NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]     DATETIME2         NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]     NVARCHAR(256)     NOT NULL DEFAULT '',
    [UpdatedBy]     NVARCHAR(256)     NOT NULL DEFAULT '',
    CONSTRAINT [PK_NewsArticles] PRIMARY KEY ([Id])
);
GO

IF OBJECT_ID(N'[NewsAttachments]', 'U') IS NULL
CREATE TABLE [NewsAttachments] (
    [Id]            INT IDENTITY(1,1) NOT NULL,
    [NewsArticleId] INT               NOT NULL,
    [FileName]      NVARCHAR(255)     NOT NULL,
    [FilePath]      NVARCHAR(500)     NOT NULL,
    [UploadedAt]    DATETIME2         NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_NewsAttachments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_NewsAttachments_NewsArticles_NewsArticleId]
        FOREIGN KEY ([NewsArticleId]) REFERENCES [NewsArticles] ([Id]) ON DELETE CASCADE
);
GO

IF OBJECT_ID(N'[AuditLogs]', 'U') IS NULL
CREATE TABLE [AuditLogs] (
    [Id]          INT IDENTITY(1,1) NOT NULL,
    [EntityName]  NVARCHAR(100)     NOT NULL,
    [EntityId]    NVARCHAR(100)     NULL,
    [Action]      NVARCHAR(50)      NOT NULL,
    [PerformedBy] NVARCHAR(256)     NULL,
    [PerformedAt] DATETIME2         NOT NULL DEFAULT GETUTCDATE(),
    [Details]     NVARCHAR(MAX)     NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
);
GO

-- ============================================================
-- SECTION 3: INDEXES
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'RoleNameIndex' AND object_id = OBJECT_ID('AspNetRoles'))
    CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'EmailIndex' AND object_id = OBJECT_ID('AspNetUsers'))
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UserNameIndex' AND object_id = OBJECT_ID('AspNetUsers'))
    CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AspNetRoleClaims_RoleId' AND object_id = OBJECT_ID('AspNetRoleClaims'))
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AspNetUserClaims_UserId' AND object_id = OBJECT_ID('AspNetUserClaims'))
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AspNetUserLogins_UserId' AND object_id = OBJECT_ID('AspNetUserLogins'))
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AspNetUserRoles_RoleId' AND object_id = OBJECT_ID('AspNetUserRoles'))
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_BomComponents_RequestItemId' AND object_id = OBJECT_ID('BomComponents'))
    CREATE INDEX [IX_BomComponents_RequestItemId] ON [BomComponents] ([RequestItemId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_BomEditComponent_RequestItemId' AND object_id = OBJECT_ID('BomEditComponent'))
    CREATE INDEX [IX_BomEditComponent_RequestItemId] ON [BomEditComponent] ([RequestItemId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DocumentRoutings_DepartmentId' AND object_id = OBJECT_ID('DocumentRoutings'))
    CREATE INDEX [IX_DocumentRoutings_DepartmentId] ON [DocumentRoutings] ([DepartmentId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DocumentRoutings_DocumentTypeId' AND object_id = OBJECT_ID('DocumentRoutings'))
    CREATE INDEX [IX_DocumentRoutings_DocumentTypeId] ON [DocumentRoutings] ([DocumentTypeId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DocumentRoutings_PlantId' AND object_id = OBJECT_ID('DocumentRoutings'))
    CREATE INDEX [IX_DocumentRoutings_PlantId] ON [DocumentRoutings] ([PlantId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DocumentRoutings_SectionId' AND object_id = OBJECT_ID('DocumentRoutings'))
    CREATE INDEX [IX_DocumentRoutings_SectionId] ON [DocumentRoutings] ([SectionId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LicensePermissionItems_RequestItemId' AND object_id = OBJECT_ID('LicensePermissionItems'))
    CREATE INDEX [IX_LicensePermissionItems_RequestItemId] ON [LicensePermissionItems] ([RequestItemId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_NewsAttachments_NewsArticleId' AND object_id = OBJECT_ID('NewsAttachments'))
    CREATE INDEX [IX_NewsAttachments_NewsArticleId] ON [NewsAttachments] ([NewsArticleId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Plants_DepartmentId' AND object_id = OBJECT_ID('Plants'))
    CREATE INDEX [IX_Plants_DepartmentId] ON [Plants] ([DepartmentId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Routings_RequestItemId' AND object_id = OBJECT_ID('Routings'))
    CREATE INDEX [IX_Routings_RequestItemId] ON [Routings] ([RequestItemId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Sections_DepartmentId' AND object_id = OBJECT_ID('Sections'))
    CREATE INDEX [IX_Sections_DepartmentId] ON [Sections] ([DepartmentId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AuditLogs_PerformedAt' AND object_id = OBJECT_ID('AuditLogs'))
    CREATE INDEX [IX_AuditLogs_PerformedAt] ON [AuditLogs] ([PerformedAt]);
GO

-- ============================================================
-- SECTION 4: SEED DATA
-- ============================================================

-- Roles
IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [NormalizedName] = N'ADMIN')
INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
VALUES (NEWID(), N'Admin', N'ADMIN', NEWID());
GO

IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [NormalizedName] = N'USER')
INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
VALUES (NEWID(), N'User', N'USER', NEWID());
GO

-- MasterData Combinations (from DbInitializer)
IF NOT EXISTS (SELECT 1 FROM [MasterDataCombinations])
BEGIN
    INSERT INTO [MasterDataCombinations] ([DepartmentName], [SectionName], [PlantName], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy])
    VALUES
        (N'HR',         N'Recruitment',    N'Main Office', GETUTCDATE(), GETUTCDATE(), N'System', N'System'),
        (N'HR',         N'Payroll',        N'Main Office', GETUTCDATE(), GETUTCDATE(), N'System', N'System'),
        (N'IT',         N'Support',        N'Main Office', GETUTCDATE(), GETUTCDATE(), N'System', N'System'),
        (N'IT',         N'Development',    N'Main Office', GETUTCDATE(), GETUTCDATE(), N'System', N'System'),
        (N'Production', N'Assembly Line 1',N'Factory A',   GETUTCDATE(), GETUTCDATE(), N'System', N'System'),
        (N'Production', N'Assembly Line 2',N'Factory B',   GETUTCDATE(), GETUTCDATE(), N'System', N'System');
END
GO

-- Document Types (common SAP document types)
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

-- EF Migrations History (mark all migrations as applied)
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

-- ============================================================
-- NOTE: Admin user is created at application startup by
-- IdentityDataInitializer.SeedData() and the generated password
-- is printed to the console output and stored in AdminPasswordLog.
-- ============================================================

PRINT 'Setup complete. Database [SAP_Service] is ready.';
GO
