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
    "FullName" TEXT NOT NULL,
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

CREATE TABLE "RepairTickets" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RepairTickets" PRIMARY KEY AUTOINCREMENT,
    "RequesterName" TEXT NOT NULL,
    "Department" TEXT NOT NULL,
    "DeviceName" TEXT NOT NULL,
    "IssueDescription" TEXT NOT NULL,
    "Priority" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL
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

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260422024224_InitialIdentityAndTickets', '9.0.4');

CREATE TABLE "NewsItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_NewsItems" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Content" TEXT NOT NULL,
    "CreatedByName" TEXT NULL,
    "CreatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260422085917_AddNewsItems', '9.0.4');

ALTER TABLE "NewsItems" ADD "AttachmentFileName" TEXT NULL;

ALTER TABLE "NewsItems" ADD "AttachmentUrl" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260422091110_AddNewsAttachment', '9.0.4');

ALTER TABLE "RepairTickets" ADD "ApproverDepartment" TEXT NOT NULL DEFAULT '';

ALTER TABLE "RepairTickets" ADD "ApproverName" TEXT NOT NULL DEFAULT '';

ALTER TABLE "RepairTickets" ADD "ApproverUserId" TEXT NOT NULL DEFAULT '';

ALTER TABLE "RepairTickets" ADD "RepairType" TEXT NOT NULL DEFAULT '';

ALTER TABLE "AspNetUsers" ADD "Department" TEXT NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260423041052_AddRepairWorkflowFields', '9.0.4');

ALTER TABLE "AspNetUsers" ADD "IsActive" INTEGER NOT NULL DEFAULT 0;

ALTER TABLE "AspNetUsers" ADD "Section" TEXT NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260423051308_AddUserSectionAndActiveStatus', '9.0.4');

ALTER TABLE "RepairTickets" ADD "RequesterUserId" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260425141935_AddRequesterUserId', '9.0.4');

ALTER TABLE "RepairTickets" ADD "CreatedByName" TEXT NULL;

ALTER TABLE "RepairTickets" ADD "UpdatedByName" TEXT NULL;

ALTER TABLE "NewsItems" ADD "UpdatedAt" TEXT NULL;

ALTER TABLE "NewsItems" ADD "UpdatedByName" TEXT NULL;

ALTER TABLE "AspNetUsers" ADD "CreatedAt" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

ALTER TABLE "AspNetUsers" ADD "CreatedByName" TEXT NULL;

ALTER TABLE "AspNetUsers" ADD "UpdatedAt" TEXT NULL;

ALTER TABLE "AspNetUsers" ADD "UpdatedByName" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260427011942_AddAuditTrailFields', '9.0.4');

ALTER TABLE "RepairTickets" ADD "AssignedItName" TEXT NOT NULL DEFAULT '';

ALTER TABLE "RepairTickets" ADD "AssignedItUserId" TEXT NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260427025754_AddItAssignmentFields', '9.0.4');

CREATE TABLE "RepairTicketStatusHistories" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RepairTicketStatusHistories" PRIMARY KEY AUTOINCREMENT,
    "RepairTicketId" INTEGER NOT NULL,
    "FromStatus" TEXT NULL,
    "ToStatus" TEXT NOT NULL,
    "Action" TEXT NOT NULL,
    "Remark" TEXT NULL,
    "ChangedAt" TEXT NOT NULL,
    "ChangedByUserId" TEXT NULL,
    "ChangedByName" TEXT NULL,
    CONSTRAINT "FK_RepairTicketStatusHistories_RepairTickets_RepairTicketId" FOREIGN KEY ("RepairTicketId") REFERENCES "RepairTickets" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_RepairTicketStatusHistories_RepairTicketId_ChangedAt" ON "RepairTicketStatusHistories" ("RepairTicketId", "ChangedAt");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260427085505_AddRepairTicketStatusHistory', '9.0.4');

ALTER TABLE "RepairTickets" ADD "DocumentNo" TEXT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260429015104_AddRepairTicketDocumentNo', '9.0.4');

ALTER TABLE "NewsItems" ADD "IsActive" INTEGER NOT NULL DEFAULT 1;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260430045439_AddNewsItemActiveStatus', '9.0.4');

COMMIT;

