-- ============================================================
-- SQL Server Clear Data Script
-- SAP Service Application
-- Deletes all data except admin user (ituser@example.com)
-- ============================================================

USE [SAP_Service];
GO

-- Disable all foreign key constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';
GO

-- Delete all data in dependent tables first
DELETE FROM [AspNetUserRoles];
DELETE FROM [AspNetUserClaims];
DELETE FROM [AspNetUserLogins];
DELETE FROM [AspNetUserTokens];
DELETE FROM [AspNetRoleClaims];
DELETE FROM [AuditLogs];
DELETE FROM [LicensePermissionItems];
DELETE FROM [BomEditComponent];
DELETE FROM [BomComponents];
DELETE FROM [RequestItems];
DELETE FROM [DocumentRoutings];
DELETE FROM [Routings];
DELETE FROM [NewsArticles];
DELETE FROM [MasterDataCombinations];
DELETE FROM [DocumentTypes];
DELETE FROM [Sections];
DELETE FROM [Departments];
DELETE FROM [Plants];

-- Delete all users except admin
DELETE FROM [AspNetUsers] WHERE [UserName] <> 'ituser@example.com';

-- Delete all roles except keep them (comment out if you want to keep roles)
-- DELETE FROM [AspNetRoles];

-- Re-enable all foreign key constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL';
GO

-- Reset identity columns (optional - uncomment if needed)
-- DBCC CHECKIDENT ('[Departments]', RESEED, 0);
-- DBCC CHECKIDENT ('[Sections]', RESEED, 0);
-- DBCC CHECKIDENT ('[Plants]', RESEED, 0);
-- DBCC CHECKIDENT ('[DocumentTypes]', RESEED, 0);
-- DBCC CHECKIDENT ('[MasterDataCombinations]', RESEED, 0);
-- DBCC CHECKIDENT ('[NewsArticles]', RESEED, 0);
-- DBCC CHECKIDENT ('[RequestItems]', RESEED, 0);
-- DBCC CHECKIDENT ('[BomComponents]', RESEED, 0);
-- DBCC CHECKIDENT ('[BomEditComponent]', RESEED, 0);
-- DBCC CHECKIDENT ('[LicensePermissionItems]', RESEED, 0);
-- DBCC CHECKIDENT ('[Routings]', RESEED, 0);
-- DBCC CHECKIDENT ('[DocumentRoutings]', RESEED, 0);
-- DBCC CHECKIDENT ('[AuditLogs]', RESEED, 0);

PRINT 'Database cleared successfully. Admin user retained.';
GO
