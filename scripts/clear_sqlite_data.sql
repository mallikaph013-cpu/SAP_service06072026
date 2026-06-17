-- SQL script to delete all data in all tables except admin user in AspNetUsers
-- This script is for SQLite (myapp.db)

PRAGMA foreign_keys = OFF;

DELETE FROM "AspNetUserRoles";
DELETE FROM "AspNetUserClaims";
DELETE FROM "AspNetUserLogins";
DELETE FROM "AspNetUserTokens";
DELETE FROM "AspNetRoleClaims";
DELETE FROM "AuditLogs";
DELETE FROM "Departments";
DELETE FROM "DocumentTypes";
DELETE FROM "MasterDataCombinations";
DELETE FROM "NewsArticles";
DELETE FROM "RequestItems";
DELETE FROM "BomComponents";
DELETE FROM "BomEditComponent";
DELETE FROM "LicensePermissionItems";
DELETE FROM "Routings";
DELETE FROM "DocumentRoutings";
DELETE FROM "Plants";
DELETE FROM "Sections";

-- Delete all users except admin (ituser@example.com)
DELETE FROM "AspNetUsers" WHERE "UserName" <> 'ituser@example.com';

PRAGMA foreign_keys = ON;
