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
    "MustChangePasswordOnFirstLogin" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "CreatedBy" TEXT NULL,
    "UpdatedBy" TEXT NULL,
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


CREATE TABLE "AuditLogs" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AuditLogs" PRIMARY KEY AUTOINCREMENT,
    "EntityName" TEXT NOT NULL,
    "EntityId" TEXT NULL,
    "Action" TEXT NOT NULL,
    "PerformedBy" TEXT NULL,
    "PerformedAt" TEXT NOT NULL,
    "Details" TEXT NULL
);


CREATE TABLE "Departments" (
    "DepartmentId" INTEGER NOT NULL CONSTRAINT "PK_Departments" PRIMARY KEY AUTOINCREMENT,
    "DepartmentName" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "UpdatedBy" TEXT NOT NULL
);


CREATE TABLE "DocumentTypes" (
    "DocumentTypeId" INTEGER NOT NULL CONSTRAINT "PK_DocumentTypes" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL
);


CREATE TABLE "MasterDataCombinations" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_MasterDataCombinations" PRIMARY KEY AUTOINCREMENT,
    "DepartmentName" TEXT NOT NULL,
    "SectionName" TEXT NOT NULL,
    "PlantName" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "UpdatedBy" TEXT NOT NULL
);


CREATE TABLE "NewsArticles" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_NewsArticles" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Content" TEXT NOT NULL,
    "ImageUrl" TEXT NULL,
    "PublishedDate" TEXT NOT NULL,
    "Author" TEXT NOT NULL,
    "IsFeatured" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "UpdatedBy" TEXT NOT NULL
);


CREATE TABLE "RequestItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RequestItems" PRIMARY KEY AUTOINCREMENT,
    "RequestType" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "Requester" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "UsageStatus" INTEGER NOT NULL,
    "RequestDate" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "UpdatedBy" TEXT NULL,
    "NextApproverId" TEXT NULL,
    "AttachmentFileName" TEXT NULL,
    "AttachmentPath" TEXT NULL,
    "DocumentNumber" TEXT NULL,
    "Plant" TEXT NULL,
    "ItemCode" TEXT NULL,
    "EnglishMatDescription" TEXT NULL,
    "ModelName" TEXT NULL,
    "BaseUnit" TEXT NULL,
    "MaterialGroup" TEXT NULL,
    "ExternalMaterialGroup" TEXT NULL,
    "DivisionCode" TEXT NULL,
    "ProfitCenter" TEXT NULL,
    "DistributionChannel" TEXT NULL,
    "BoiCode" TEXT NULL,
    "MrpController" TEXT NULL,
    "StorageLocation" TEXT NULL,
    "ProductionSupervisor" TEXT NULL,
    "CostingLotSize" INTEGER NULL,
    "ValClass" TEXT NULL,
    "StandardPack" TEXT NULL,
    "BoiDescription" TEXT NULL,
    "MakerMfrPartNumber" TEXT NULL,
    "CommCodeTariffCode" TEXT NULL,
    "TraffCodePercentage" decimal(18, 2) NULL,
    "StorageLocationB1" TEXT NULL,
    "PriceControl" TEXT NULL,
    "Currency" TEXT NULL,
    "SupplierCode" TEXT NULL,
    "MatType" TEXT NULL,
    "Check" INTEGER NOT NULL,
    "DevicePlant" TEXT NULL,
    "AssemblyPlant" TEXT NULL,
    "IpoPlant" TEXT NULL,
    "AsiOfPlant" TEXT NULL,
    "PriceUnit" INTEGER NULL,
    "StorageLocationEP" TEXT NULL,
    "ToolingBSection" TEXT NULL,
    "PoNumber" TEXT NULL,
    "StatusInA" TEXT NULL,
    "DateIn" TEXT NULL,
    "QuotationNumber" TEXT NULL,
    "ToolingBModel" TEXT NULL,
    "TariffCode" TEXT NULL,
    "Planner" TEXT NULL,
    "CurrentICS" TEXT NULL,
    "Level" TEXT NULL,
    "Rohs" TEXT NULL,
    "CodenMid" TEXT NULL,
    "SalesOrg" TEXT NULL,
    "TaxTh" TEXT NULL,
    "MaterialStatisticsGroup" TEXT NULL,
    "AccountAssignment" TEXT NULL,
    "GeneralItemCategory" TEXT NULL,
    "Availability" TEXT NULL,
    "Transportation" TEXT NULL,
    "LoadingGroup" TEXT NULL,
    "PlanDelTime" TEXT NULL,
    "SchedMargin" TEXT NULL,
    "MinLot" TEXT NULL,
    "MaxLot" TEXT NULL,
    "FixedLot" TEXT NULL,
    "Rounding" TEXT NULL,
    "Mtlsm" TEXT NULL,
    "Effective" TEXT NULL,
    "StorageLoc" TEXT NULL,
    "ReceiveStorage" TEXT NULL,
    "PurchasingGroup" TEXT NULL,
    "EditBomFg" TEXT NULL,
    "EditBomAllFg" INTEGER NOT NULL,
    "Price" decimal(18, 2) NULL
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
    "PlantId" INTEGER NOT NULL CONSTRAINT "PK_Plants" PRIMARY KEY AUTOINCREMENT,
    "PlantName" TEXT NOT NULL,
    "DepartmentId" INTEGER NOT NULL,
    CONSTRAINT "FK_Plants_Departments_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES "Departments" ("DepartmentId") ON DELETE CASCADE
);


CREATE TABLE "Sections" (
    "SectionId" INTEGER NOT NULL CONSTRAINT "PK_Sections" PRIMARY KEY AUTOINCREMENT,
    "SectionName" TEXT NOT NULL,
    "DepartmentId" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "UpdatedBy" TEXT NOT NULL,
    CONSTRAINT "FK_Sections_Departments_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES "Departments" ("DepartmentId") ON DELETE CASCADE
);


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


CREATE TABLE "LicensePermissionItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_LicensePermissionItems" PRIMARY KEY AUTOINCREMENT,
    "RequestItemId" INTEGER NOT NULL,
    "SapUsername" TEXT NULL,
    "TCode" TEXT NULL,
    CONSTRAINT "FK_LicensePermissionItems_RequestItems_RequestItemId" FOREIGN KEY ("RequestItemId") REFERENCES "RequestItems" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Routings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Routings" PRIMARY KEY AUTOINCREMENT,
    "RequestItemId" INTEGER NOT NULL,
    "Material" TEXT NULL,
    "Description" TEXT NULL,
    "Counter" TEXT NULL,
    "Plant" TEXT NULL,
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
    "Alternative" TEXT NULL,
    "BomUsage" TEXT NULL,
    "Group" TEXT NULL,
    "GroupCounter" TEXT NULL,
    CONSTRAINT "FK_Routings_RequestItems_RequestItemId" FOREIGN KEY ("RequestItemId") REFERENCES "RequestItems" ("Id") ON DELETE CASCADE
);


CREATE TABLE "DocumentRoutings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_DocumentRoutings" PRIMARY KEY AUTOINCREMENT,
    "DocumentTypeId" INTEGER NOT NULL,
    "DepartmentId" INTEGER NOT NULL,
    "SectionId" INTEGER NOT NULL,
    "PlantId" INTEGER NOT NULL,
    "Step" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "UpdatedBy" TEXT NOT NULL,
    CONSTRAINT "FK_DocumentRoutings_Departments_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES "Departments" ("DepartmentId"),
    CONSTRAINT "FK_DocumentRoutings_DocumentTypes_DocumentTypeId" FOREIGN KEY ("DocumentTypeId") REFERENCES "DocumentTypes" ("DocumentTypeId"),
    CONSTRAINT "FK_DocumentRoutings_Plants_PlantId" FOREIGN KEY ("PlantId") REFERENCES "Plants" ("PlantId"),
    CONSTRAINT "FK_DocumentRoutings_Sections_SectionId" FOREIGN KEY ("SectionId") REFERENCES "Sections" ("SectionId")
);


CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");


CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");


CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");


CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");


CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");


CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");


CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");


CREATE INDEX "IX_AuditLogs_PerformedAt" ON "AuditLogs" ("PerformedAt");


CREATE INDEX "IX_BomComponents_RequestItemId" ON "BomComponents" ("RequestItemId");


CREATE INDEX "IX_BomEditComponent_RequestItemId" ON "BomEditComponent" ("RequestItemId");


CREATE INDEX "IX_DocumentRoutings_DepartmentId" ON "DocumentRoutings" ("DepartmentId");


CREATE INDEX "IX_DocumentRoutings_DocumentTypeId" ON "DocumentRoutings" ("DocumentTypeId");


CREATE INDEX "IX_DocumentRoutings_PlantId" ON "DocumentRoutings" ("PlantId");


CREATE INDEX "IX_DocumentRoutings_SectionId" ON "DocumentRoutings" ("SectionId");


CREATE INDEX "IX_LicensePermissionItems_RequestItemId" ON "LicensePermissionItems" ("RequestItemId");


CREATE INDEX "IX_Plants_DepartmentId" ON "Plants" ("DepartmentId");


CREATE INDEX "IX_Routings_RequestItemId" ON "Routings" ("RequestItemId");


CREATE INDEX "IX_Sections_DepartmentId" ON "Sections" ("DepartmentId");


