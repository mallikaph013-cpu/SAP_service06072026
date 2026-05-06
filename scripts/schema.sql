CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "AspNetRoles" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY,
    "Name" TEXT NULL,
    "NormalizedName" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL
);

CREATE TABLE "AspNetUsers" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY,
    "FirstName" TEXT NULL,
    "LastName" TEXT NULL,
    "Department" TEXT NULL,
    "Section" TEXT NULL,
    "Plant" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsIT" INTEGER NOT NULL,
    "UserName" TEXT NULL,
    "NormalizedUserName" TEXT NULL,
    "Email" TEXT NULL,
    "NormalizedEmail" TEXT NULL,
    "EmailConfirmed" INTEGER NOT NULL,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" INTEGER NOT NULL,
    "TwoFactorEnabled" INTEGER NOT NULL,
    "LockoutEnd" TEXT NULL,
    "LockoutEnabled" INTEGER NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL
);

CREATE TABLE "Departments" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Departments" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL
);

CREATE TABLE "MasterDataCombinations" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_MasterDataCombinations" PRIMARY KEY AUTOINCREMENT,
    "DepartmentName" TEXT NOT NULL,
    "SectionName" TEXT NOT NULL,
    "PlantName" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "UpdatedBy" TEXT NOT NULL
);

CREATE TABLE "RequestItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RequestItems" PRIMARY KEY AUTOINCREMENT,
    "Description" TEXT NOT NULL,
    "Requester" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "RequestDate" TEXT NOT NULL,
    "PlantFG" TEXT NULL,
    "ItemCode" TEXT NULL,
    "EnglishMatDescription" TEXT NULL,
    "ModelName" TEXT NULL,
    "BaseUnit" TEXT NULL,
    "MaterialGroup" TEXT NULL,
    "DivisionCode" TEXT NULL,
    "ProfitCenter" TEXT NULL,
    "DistributionChannel" TEXT NULL,
    "StandardPack" TEXT NULL,
    "BoiCode" TEXT NULL,
    "MrpController" TEXT NULL,
    "StorageLocation" TEXT NULL,
    "ProductionSupervisor" TEXT NULL,
    "CostingLotSize" INTEGER NULL,
    "ValClass" TEXT NULL,
    "Price" decimal(18, 2) NULL,
    "CreatedBy" TEXT NULL,
    "CreatedAt" TEXT NULL
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" TEXT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Plants" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Plants" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "DepartmentId" INTEGER NOT NULL,
    CONSTRAINT "FK_Plants_Departments_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES "Departments" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Sections" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Sections" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "DepartmentId" INTEGER NOT NULL,
    CONSTRAINT "FK_Sections_Departments_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES "Departments" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

CREATE INDEX "IX_Plants_DepartmentId" ON "Plants" ("DepartmentId");

CREATE INDEX "IX_Sections_DepartmentId" ON "Sections" ("DepartmentId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260221134529_InitialCreate', '9.0.0');

ALTER TABLE "RequestItems" RENAME COLUMN "CreatedBy" TO "Transportation";

ALTER TABLE "RequestItems" RENAME COLUMN "CreatedAt" TO "ToolingBSection";

ALTER TABLE "RequestItems" ADD "AccountAssignment" TEXT NULL;

ALTER TABLE "RequestItems" ADD "AsiOfPlant" TEXT NULL;

ALTER TABLE "RequestItems" ADD "AssemblyPlant" TEXT NULL;

ALTER TABLE "RequestItems" ADD "Availability" TEXT NULL;

ALTER TABLE "RequestItems" ADD "BoiDescription" TEXT NULL;

ALTER TABLE "RequestItems" ADD "Check" INTEGER NOT NULL DEFAULT 0;

ALTER TABLE "RequestItems" ADD "CodenMid" TEXT NULL;

ALTER TABLE "RequestItems" ADD "CommCodeTariffCode" TEXT NULL;

ALTER TABLE "RequestItems" ADD "Currency" TEXT NULL;

ALTER TABLE "RequestItems" ADD "CurrentICS" TEXT NULL;

ALTER TABLE "RequestItems" ADD "DateIn" TEXT NULL;

ALTER TABLE "RequestItems" ADD "DevicePlant" TEXT NULL;

ALTER TABLE "RequestItems" ADD "Effective" TEXT NULL;

ALTER TABLE "RequestItems" ADD "ExternalMaterialGroup" TEXT NULL;

ALTER TABLE "RequestItems" ADD "FixedLot" TEXT NULL;

ALTER TABLE "RequestItems" ADD "GeneralItemCategory" TEXT NULL;

ALTER TABLE "RequestItems" ADD "IpoPlant" TEXT NULL;

ALTER TABLE "RequestItems" ADD "Level" TEXT NULL;

ALTER TABLE "RequestItems" ADD "LoadingGroup" TEXT NULL;

ALTER TABLE "RequestItems" ADD "MakerMfrPartNumber" TEXT NULL;

ALTER TABLE "RequestItems" ADD "MatType" TEXT NULL;

ALTER TABLE "RequestItems" ADD "MaterialStatisticsGroup" TEXT NULL;

ALTER TABLE "RequestItems" ADD "MaxLot" TEXT NULL;

ALTER TABLE "RequestItems" ADD "MinLot" TEXT NULL;

ALTER TABLE "RequestItems" ADD "Mtlsm" TEXT NULL;

ALTER TABLE "RequestItems" ADD "PlanDelTime" TEXT NULL;

ALTER TABLE "RequestItems" ADD "Planner" TEXT NULL;

ALTER TABLE "RequestItems" ADD "PoNumber" TEXT NULL;

ALTER TABLE "RequestItems" ADD "PriceControl" TEXT NULL;

ALTER TABLE "RequestItems" ADD "PriceUnit" INTEGER NULL;

ALTER TABLE "RequestItems" ADD "PurchasingGroup" TEXT NULL;

ALTER TABLE "RequestItems" ADD "QuotationNumber" TEXT NULL;

ALTER TABLE "RequestItems" ADD "ReceiveStorage" TEXT NULL;

ALTER TABLE "RequestItems" ADD "RequestType" TEXT NOT NULL DEFAULT '';

ALTER TABLE "RequestItems" ADD "Rohs" TEXT NULL;

ALTER TABLE "RequestItems" ADD "Rounding" TEXT NULL;

ALTER TABLE "RequestItems" ADD "SalesOrg" TEXT NULL;

ALTER TABLE "RequestItems" ADD "SchedMargin" TEXT NULL;

ALTER TABLE "RequestItems" ADD "StatusInA" TEXT NULL;

ALTER TABLE "RequestItems" ADD "StorageLoc" TEXT NULL;

ALTER TABLE "RequestItems" ADD "StorageLocationB1" TEXT NULL;

ALTER TABLE "RequestItems" ADD "StorageLocationEP" TEXT NULL;

ALTER TABLE "RequestItems" ADD "SupplierCode" TEXT NULL;

ALTER TABLE "RequestItems" ADD "TariffCode" TEXT NULL;

ALTER TABLE "RequestItems" ADD "TaxTh" TEXT NULL;

ALTER TABLE "RequestItems" ADD "ToolingBModel" TEXT NULL;

ALTER TABLE "RequestItems" ADD "TraffCodePercentage" decimal(18, 2) NULL;

CREATE TABLE "BomComponents" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_BomComponents" PRIMARY KEY AUTOINCREMENT,
    "RequestItemId" INTEGER NOT NULL,
    "Level" INTEGER NOT NULL,
    "Item" TEXT NULL,
    "ItemCat" TEXT NULL,
    "ComponentNumber" TEXT NULL,
    "Description" TEXT NULL,
    "ItemQuantity" decimal(18, 5) NULL,
    "Unit" TEXT NULL,
    "BomUsage" TEXT NULL,
    "Plant" TEXT NULL,
    "Sloc" TEXT NULL,
    CONSTRAINT "FK_BomComponents_RequestItems_RequestItemId" FOREIGN KEY ("RequestItemId") REFERENCES "RequestItems" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Routings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Routings" PRIMARY KEY AUTOINCREMENT,
    "RequestItemId" INTEGER NOT NULL,
    "Material" TEXT NULL,
    "Description" TEXT NULL,
    "WorkCenter" TEXT NULL,
    "Operation" TEXT NULL,
    "BaseQty" decimal(18, 5) NULL,
    "Unit" TEXT NULL,
    "DirectLaborCosts" decimal(18, 5) NULL,
    "DirectExpenses" decimal(18, 5) NULL,
    "AllocationExpense" decimal(18, 5) NULL,
    "ProductionVersionCode" TEXT NULL,
    "Version" TEXT NULL,
    "ValidFrom" TEXT NULL,
    "ValidTo" TEXT NULL,
    "MaximumLotSize" decimal(18, 5) NULL,
    "Group" TEXT NULL,
    "GroupCounter" TEXT NULL,
    CONSTRAINT "FK_Routings_RequestItems_RequestItemId" FOREIGN KEY ("RequestItemId") REFERENCES "RequestItems" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_BomComponents_RequestItemId" ON "BomComponents" ("RequestItemId");

CREATE INDEX "IX_Routings_RequestItemId" ON "Routings" ("RequestItemId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260223065828_AddBomAndRoutingTables', '9.0.0');

ALTER TABLE "AspNetUsers" ADD "CreatedAt" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE "AspNetUsers" ADD "CreatedBy" TEXT NULL;

ALTER TABLE "AspNetUsers" ADD "UpdatedAt" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE "AspNetUsers" ADD "UpdatedBy" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260223092442_ConsolidateUser', '9.0.0');

ALTER TABLE "Sections" RENAME COLUMN "Name" TO "SectionName";

ALTER TABLE "Sections" RENAME COLUMN "Id" TO "SectionId";

ALTER TABLE "Plants" RENAME COLUMN "Name" TO "PlantName";

ALTER TABLE "Plants" RENAME COLUMN "Id" TO "PlantId";

ALTER TABLE "Departments" RENAME COLUMN "Name" TO "DepartmentName";

ALTER TABLE "Departments" RENAME COLUMN "Id" TO "DepartmentId";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260223094110_UpdateSchema', '9.0.0');

CREATE TABLE "DocumentTypes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DocumentTypes" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL
);

CREATE TABLE "DocumentRoutings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DocumentRoutings" PRIMARY KEY AUTOINCREMENT,
    "DocumentTypeId" INTEGER NOT NULL,
    "DepartmentId" INTEGER NOT NULL,
    "SectionId" INTEGER NOT NULL,
    "PlantId" INTEGER NOT NULL,
    "Step" INTEGER NOT NULL,
    CONSTRAINT "FK_DocumentRoutings_Departments_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES "Departments" ("DepartmentId") ON DELETE CASCADE,
    CONSTRAINT "FK_DocumentRoutings_DocumentTypes_DocumentTypeId" FOREIGN KEY ("DocumentTypeId") REFERENCES "DocumentTypes" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_DocumentRoutings_Plants_PlantId" FOREIGN KEY ("PlantId") REFERENCES "Plants" ("PlantId") ON DELETE CASCADE,
    CONSTRAINT "FK_DocumentRoutings_Sections_SectionId" FOREIGN KEY ("SectionId") REFERENCES "Sections" ("SectionId") ON DELETE CASCADE
);

CREATE INDEX "IX_DocumentRoutings_DepartmentId" ON "DocumentRoutings" ("DepartmentId");

CREATE INDEX "IX_DocumentRoutings_DocumentTypeId" ON "DocumentRoutings" ("DocumentTypeId");

CREATE INDEX "IX_DocumentRoutings_PlantId" ON "DocumentRoutings" ("PlantId");

CREATE INDEX "IX_DocumentRoutings_SectionId" ON "DocumentRoutings" ("SectionId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260224093217_AddDocumentRouting', '9.0.0');

ALTER TABLE "RequestItems" ADD "NextApproverId" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260225023651_AddNextApproverIdToRequestItem', '9.0.0');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260225050147_AddDepartmentAndSectionTables', '9.0.0');

ALTER TABLE "DocumentTypes" RENAME COLUMN "Id" TO "DocumentTypeId";

CREATE TABLE "ef_temp_DocumentTypes" (
    "DocumentTypeId" INTEGER NOT NULL CONSTRAINT "PK_DocumentTypes" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL
);

INSERT INTO "ef_temp_DocumentTypes" ("DocumentTypeId", "Name")
SELECT "DocumentTypeId", "Name"
FROM "DocumentTypes";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;
DROP TABLE "DocumentTypes";

ALTER TABLE "ef_temp_DocumentTypes" RENAME TO "DocumentTypes";

COMMIT;

PRAGMA foreign_keys = 1;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260225065550_AddDocumentTypeIdToDocumentRoutings', '9.0.0');

BEGIN TRANSACTION;
CREATE TABLE "NewsArticles" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_NewsArticles" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Content" TEXT NOT NULL,
    "ImageUrl" TEXT NULL,
    "PublishedDate" TEXT NOT NULL,
    "Author" TEXT NOT NULL,
    "IsFeatured" INTEGER NOT NULL
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260226015922_AddNewsArticle', '9.0.0');

ALTER TABLE "RequestItems" RENAME COLUMN "PlantFG" TO "Plant";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260226040308_RenamePlantFGToPlant', '9.0.0');

ALTER TABLE "RequestItems" ADD "ItemCodeForm" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260228021026_AddItemCodeForm', '9.0.0');

ALTER TABLE "RequestItems" RENAME COLUMN "ItemCodeForm" TO "UnitTo";

ALTER TABLE "RequestItems" ADD "BomUsageFrom" TEXT NULL;

ALTER TABLE "RequestItems" ADD "BomUsageTo" TEXT NULL;

ALTER TABLE "RequestItems" ADD "DescriptionFrom" TEXT NULL;

ALTER TABLE "RequestItems" ADD "DescriptionTo" TEXT NULL;

ALTER TABLE "RequestItems" ADD "ItemCodeTo" TEXT NULL;

ALTER TABLE "RequestItems" ADD "ItemQuantityFrom" TEXT NULL;

ALTER TABLE "RequestItems" ADD "ItemQuantityTo" TEXT NULL;

ALTER TABLE "RequestItems" ADD "PlantTo" TEXT NULL;

ALTER TABLE "RequestItems" ADD "SlocFrom" TEXT NULL;

ALTER TABLE "RequestItems" ADD "SlocTo" TEXT NULL;

ALTER TABLE "RequestItems" ADD "UnitFrom" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260228034856_AddDescriptionFrom', '9.0.0');

ALTER TABLE "RequestItems" RENAME COLUMN "UnitTo" TO "EditBomFg";

ALTER TABLE "RequestItems" ADD "EditBomAllFg" INTEGER NOT NULL DEFAULT 0;

CREATE TABLE "BomEditComponent" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_BomEditComponent" PRIMARY KEY AUTOINCREMENT,
    "ItemCodeFrom" TEXT NULL,
    "DescriptionFrom" TEXT NULL,
    "ItemQuantityFrom" TEXT NULL,
    "UnitFrom" TEXT NULL,
    "BomUsageFrom" TEXT NULL,
    "SlocFrom" TEXT NULL,
    "ItemCodeTo" TEXT NULL,
    "DescriptionTo" TEXT NULL,
    "ItemQuantityTo" TEXT NULL,
    "UnitTo" TEXT NULL,
    "BomUsageTo" TEXT NULL,
    "SlocTo" TEXT NULL,
    "PlantTo" TEXT NULL,
    "RequestItemId" INTEGER NULL,
    CONSTRAINT "FK_BomEditComponent_RequestItems_RequestItemId" FOREIGN KEY ("RequestItemId") REFERENCES "RequestItems" ("Id")
);

CREATE INDEX "IX_BomEditComponent_RequestItemId" ON "BomEditComponent" ("RequestItemId");

CREATE TABLE "ef_temp_RequestItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RequestItems" PRIMARY KEY AUTOINCREMENT,
    "AccountAssignment" TEXT NULL,
    "AsiOfPlant" TEXT NULL,
    "AssemblyPlant" TEXT NULL,
    "Availability" TEXT NULL,
    "BaseUnit" TEXT NULL,
    "BoiCode" TEXT NULL,
    "BoiDescription" TEXT NULL,
    "Check" INTEGER NOT NULL,
    "CodenMid" TEXT NULL,
    "CommCodeTariffCode" TEXT NULL,
    "CostingLotSize" INTEGER NULL,
    "Currency" TEXT NULL,
    "CurrentICS" TEXT NULL,
    "DateIn" TEXT NULL,
    "Description" TEXT NOT NULL,
    "DevicePlant" TEXT NULL,
    "DistributionChannel" TEXT NULL,
    "DivisionCode" TEXT NULL,
    "EditBomAllFg" INTEGER NOT NULL,
    "EditBomFg" TEXT NULL,
    "Effective" TEXT NULL,
    "EnglishMatDescription" TEXT NULL,
    "ExternalMaterialGroup" TEXT NULL,
    "FixedLot" TEXT NULL,
    "GeneralItemCategory" TEXT NULL,
    "IpoPlant" TEXT NULL,
    "ItemCode" TEXT NULL,
    "Level" TEXT NULL,
    "LoadingGroup" TEXT NULL,
    "MakerMfrPartNumber" TEXT NULL,
    "MatType" TEXT NULL,
    "MaterialGroup" TEXT NULL,
    "MaterialStatisticsGroup" TEXT NULL,
    "MaxLot" TEXT NULL,
    "MinLot" TEXT NULL,
    "ModelName" TEXT NULL,
    "MrpController" TEXT NULL,
    "Mtlsm" TEXT NULL,
    "NextApproverId" TEXT NULL,
    "PlanDelTime" TEXT NULL,
    "Planner" TEXT NULL,
    "Plant" TEXT NULL,
    "PoNumber" TEXT NULL,
    "Price" decimal(18, 2) NULL,
    "PriceControl" TEXT NULL,
    "PriceUnit" INTEGER NULL,
    "ProductionSupervisor" TEXT NULL,
    "ProfitCenter" TEXT NULL,
    "PurchasingGroup" TEXT NULL,
    "QuotationNumber" TEXT NULL,
    "ReceiveStorage" TEXT NULL,
    "RequestDate" TEXT NOT NULL,
    "RequestType" TEXT NOT NULL,
    "Requester" TEXT NOT NULL,
    "Rohs" TEXT NULL,
    "Rounding" TEXT NULL,
    "SalesOrg" TEXT NULL,
    "SchedMargin" TEXT NULL,
    "StandardPack" TEXT NULL,
    "Status" TEXT NOT NULL,
    "StatusInA" TEXT NULL,
    "StorageLoc" TEXT NULL,
    "StorageLocation" TEXT NULL,
    "StorageLocationB1" TEXT NULL,
    "StorageLocationEP" TEXT NULL,
    "SupplierCode" TEXT NULL,
    "TariffCode" TEXT NULL,
    "TaxTh" TEXT NULL,
    "ToolingBModel" TEXT NULL,
    "ToolingBSection" TEXT NULL,
    "TraffCodePercentage" decimal(18, 2) NULL,
    "Transportation" TEXT NULL,
    "ValClass" TEXT NULL
);

INSERT INTO "ef_temp_RequestItems" ("Id", "AccountAssignment", "AsiOfPlant", "AssemblyPlant", "Availability", "BaseUnit", "BoiCode", "BoiDescription", "Check", "CodenMid", "CommCodeTariffCode", "CostingLotSize", "Currency", "CurrentICS", "DateIn", "Description", "DevicePlant", "DistributionChannel", "DivisionCode", "EditBomAllFg", "EditBomFg", "Effective", "EnglishMatDescription", "ExternalMaterialGroup", "FixedLot", "GeneralItemCategory", "IpoPlant", "ItemCode", "Level", "LoadingGroup", "MakerMfrPartNumber", "MatType", "MaterialGroup", "MaterialStatisticsGroup", "MaxLot", "MinLot", "ModelName", "MrpController", "Mtlsm", "NextApproverId", "PlanDelTime", "Planner", "Plant", "PoNumber", "Price", "PriceControl", "PriceUnit", "ProductionSupervisor", "ProfitCenter", "PurchasingGroup", "QuotationNumber", "ReceiveStorage", "RequestDate", "RequestType", "Requester", "Rohs", "Rounding", "SalesOrg", "SchedMargin", "StandardPack", "Status", "StatusInA", "StorageLoc", "StorageLocation", "StorageLocationB1", "StorageLocationEP", "SupplierCode", "TariffCode", "TaxTh", "ToolingBModel", "ToolingBSection", "TraffCodePercentage", "Transportation", "ValClass")
SELECT "Id", "AccountAssignment", "AsiOfPlant", "AssemblyPlant", "Availability", "BaseUnit", "BoiCode", "BoiDescription", "Check", "CodenMid", "CommCodeTariffCode", "CostingLotSize", "Currency", "CurrentICS", "DateIn", "Description", "DevicePlant", "DistributionChannel", "DivisionCode", "EditBomAllFg", "EditBomFg", "Effective", "EnglishMatDescription", "ExternalMaterialGroup", "FixedLot", "GeneralItemCategory", "IpoPlant", "ItemCode", "Level", "LoadingGroup", "MakerMfrPartNumber", "MatType", "MaterialGroup", "MaterialStatisticsGroup", "MaxLot", "MinLot", "ModelName", "MrpController", "Mtlsm", "NextApproverId", "PlanDelTime", "Planner", "Plant", "PoNumber", "Price", "PriceControl", "PriceUnit", "ProductionSupervisor", "ProfitCenter", "PurchasingGroup", "QuotationNumber", "ReceiveStorage", "RequestDate", "RequestType", "Requester", "Rohs", "Rounding", "SalesOrg", "SchedMargin", "StandardPack", "Status", "StatusInA", "StorageLoc", "StorageLocation", "StorageLocationB1", "StorageLocationEP", "SupplierCode", "TariffCode", "TaxTh", "ToolingBModel", "ToolingBSection", "TraffCodePercentage", "Transportation", "ValClass"
FROM "RequestItems";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;
DROP TABLE "RequestItems";

ALTER TABLE "ef_temp_RequestItems" RENAME TO "RequestItems";

COMMIT;

PRAGMA foreign_keys = 1;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260302040659_AddEditBomFgFields', '9.0.0');

BEGIN TRANSACTION;
CREATE TABLE "LicensePermissionItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_LicensePermissionItems" PRIMARY KEY AUTOINCREMENT,
    "RequestItemId" INTEGER NOT NULL,
    "SapUsername" TEXT NULL,
    "TCode" TEXT NULL,
    CONSTRAINT "FK_LicensePermissionItems_RequestItems_RequestItemId" FOREIGN KEY ("RequestItemId") REFERENCES "RequestItems" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_LicensePermissionItems_RequestItemId" ON "LicensePermissionItems" ("RequestItemId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260303011140_AddLicensePermissionItems', '9.0.0');

CREATE TABLE "AuditLogs" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AuditLogs" PRIMARY KEY AUTOINCREMENT,
    "EntityName" TEXT NOT NULL,
    "EntityId" TEXT NULL,
    "Action" TEXT NOT NULL,
    "PerformedBy" TEXT NULL,
    "PerformedAt" TEXT NOT NULL,
    "Details" TEXT NULL
);

CREATE INDEX "IX_AuditLogs_PerformedAt" ON "AuditLogs" ("PerformedAt");

CREATE TABLE "ef_temp_DocumentRoutings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DocumentRoutings" PRIMARY KEY AUTOINCREMENT,
    "DepartmentId" INTEGER NOT NULL,
    "DocumentTypeId" INTEGER NOT NULL,
    "PlantId" INTEGER NOT NULL,
    "SectionId" INTEGER NOT NULL,
    "Step" INTEGER NOT NULL,
    CONSTRAINT "FK_DocumentRoutings_Departments_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES "Departments" ("DepartmentId"),
    CONSTRAINT "FK_DocumentRoutings_DocumentTypes_DocumentTypeId" FOREIGN KEY ("DocumentTypeId") REFERENCES "DocumentTypes" ("DocumentTypeId"),
    CONSTRAINT "FK_DocumentRoutings_Plants_PlantId" FOREIGN KEY ("PlantId") REFERENCES "Plants" ("PlantId"),
    CONSTRAINT "FK_DocumentRoutings_Sections_SectionId" FOREIGN KEY ("SectionId") REFERENCES "Sections" ("SectionId")
);

INSERT INTO "ef_temp_DocumentRoutings" ("Id", "DepartmentId", "DocumentTypeId", "PlantId", "SectionId", "Step")
SELECT "Id", "DepartmentId", "DocumentTypeId", "PlantId", "SectionId", "Step"
FROM "DocumentRoutings";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;
DROP TABLE "DocumentRoutings";

ALTER TABLE "ef_temp_DocumentRoutings" RENAME TO "DocumentRoutings";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;
CREATE INDEX "IX_DocumentRoutings_DepartmentId" ON "DocumentRoutings" ("DepartmentId");

CREATE INDEX "IX_DocumentRoutings_DocumentTypeId" ON "DocumentRoutings" ("DocumentTypeId");

CREATE INDEX "IX_DocumentRoutings_PlantId" ON "DocumentRoutings" ("PlantId");

CREATE INDEX "IX_DocumentRoutings_SectionId" ON "DocumentRoutings" ("SectionId");

COMMIT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260305052442_AddAuditLogs', '9.0.0');

BEGIN TRANSACTION;
ALTER TABLE "RequestItems" ADD "UpdatedAt" TEXT NULL;

ALTER TABLE "RequestItems" ADD "UpdatedBy" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260306041659_AddRequestItemUpdatedMetadata', '9.0.0');

ALTER TABLE "RequestItems" ADD "UsageStatus" INTEGER NOT NULL DEFAULT 1;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260306042203_AddRequestItemUsageStatusSoftDelete', '9.0.0');

ALTER TABLE "Sections" ADD "CreatedAt" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE "Sections" ADD "CreatedBy" TEXT NOT NULL DEFAULT '';

ALTER TABLE "Sections" ADD "UpdatedAt" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE "Sections" ADD "UpdatedBy" TEXT NOT NULL DEFAULT '';

ALTER TABLE "NewsArticles" ADD "CreatedAt" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE "NewsArticles" ADD "CreatedBy" TEXT NOT NULL DEFAULT '';

ALTER TABLE "NewsArticles" ADD "UpdatedAt" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE "NewsArticles" ADD "UpdatedBy" TEXT NOT NULL DEFAULT '';

ALTER TABLE "DocumentRoutings" ADD "CreatedAt" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE "DocumentRoutings" ADD "CreatedBy" TEXT NOT NULL DEFAULT '';

ALTER TABLE "DocumentRoutings" ADD "UpdatedAt" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE "DocumentRoutings" ADD "UpdatedBy" TEXT NOT NULL DEFAULT '';

ALTER TABLE "Departments" ADD "CreatedAt" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE "Departments" ADD "CreatedBy" TEXT NOT NULL DEFAULT '';

ALTER TABLE "Departments" ADD "UpdatedAt" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE "Departments" ADD "UpdatedBy" TEXT NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260306052640_AddAuditFieldsToManagementEntitiesV2', '9.0.0');

ALTER TABLE "Routings" ADD "Alternative" TEXT NULL;

ALTER TABLE "Routings" ADD "BomUsage" TEXT NULL;

ALTER TABLE "Routings" ADD "Counter" TEXT NULL;

ALTER TABLE "Routings" ADD "Plant" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260309040523_AddRoutingAdditionalFields', '9.0.0');

ALTER TABLE "AspNetUsers" ADD "MustChangePasswordOnFirstLogin" INTEGER NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260313055109_AddMustChangePasswordOnFirstLogin', '9.0.0');

ALTER TABLE "Sections" ADD "IsActive" INTEGER NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260313071437_AddSectionIsActiveStatus', '9.0.0');

ALTER TABLE "Departments" ADD "IsActive" INTEGER NOT NULL DEFAULT 1;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260313072205_AddDepartmentIsActiveStatus', '9.0.0');

ALTER TABLE "RequestItems" ADD "AttachmentFileName" TEXT NULL;

ALTER TABLE "RequestItems" ADD "AttachmentPath" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260316015952_AddRequestAttachmentFields', '9.0.0');

ALTER TABLE "RequestItems" ADD "DocumentNumber" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260316020925_AddRequestDocumentNumber', '9.0.0');

CREATE TABLE "NewsAttachments" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_NewsAttachments" PRIMARY KEY AUTOINCREMENT,
    "NewsArticleId" INTEGER NOT NULL,
    "FileName" TEXT NOT NULL,
    "FilePath" TEXT NOT NULL,
    "UploadedAt" TEXT NOT NULL,
    CONSTRAINT "FK_NewsAttachments_NewsArticles_NewsArticleId" FOREIGN KEY ("NewsArticleId") REFERENCES "NewsArticles" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_NewsAttachments_NewsArticleId" ON "NewsAttachments" ("NewsArticleId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260325090514_AddNewsAttachment', '9.0.0');

COMMIT;

