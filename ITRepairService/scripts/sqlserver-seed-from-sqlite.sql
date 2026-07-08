/*
  SQL Server seed generated from SQLite data source: itrepair.db
  Generated automatically. Safe to run multiple times (idempotent).
*/

SET NOCOUNT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.AspNetRoles', N'U') IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE [Id] = N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47')
            INSERT INTO dbo.AspNetRoles ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47', N'User', N'USER', NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE [Id] = N'29df7d0c-8e4a-4789-8e4a-ea44a9dc2905')
            INSERT INTO dbo.AspNetRoles ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'29df7d0c-8e4a-4789-8e4a-ea44a9dc2905', N'ITSupport', N'ITSUPPORT', NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE [Id] = N'dccef850-a86f-4eb8-9d55-d7f9121aa458')
            INSERT INTO dbo.AspNetRoles ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'dccef850-a86f-4eb8-9d55-d7f9121aa458', N'Admin', N'ADMIN', NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE [Id] = N'1e610076-7eb1-42e8-9f99-d270a9c1d64c')
            INSERT INTO dbo.AspNetRoles ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'1e610076-7eb1-42e8-9f99-d270a9c1d64c', N'Approve', N'APPROVE', NULL);
    END;

    IF OBJECT_ID(N'dbo.AspNetUsers', N'U') IS NOT NULL
       AND COL_LENGTH(N'dbo.AspNetUsers', N'MustChangePassword') IS NULL
    BEGIN
        ALTER TABLE dbo.AspNetUsers
            ADD [MustChangePassword] bit NOT NULL
                CONSTRAINT [DF_AspNetUsers_MustChangePassword] DEFAULT (0);
    END;

    IF OBJECT_ID(N'dbo.AspNetUsers', N'U') IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUsers WHERE [Id] = N'c53a0ce7-9c56-44e9-9cb6-aabb35928853')
            INSERT INTO dbo.AspNetUsers ([Id], [FullName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Department], [IsActive], [Section], [CreatedAt], [CreatedByName], [UpdatedAt], [UpdatedByName]) VALUES (N'c53a0ce7-9c56-44e9-9cb6-aabb35928853', N'System Administrator', N'admin@itrepair.local', N'ADMIN@ITREPAIR.LOCAL', N'admin@itrepair.local', N'ADMIN@ITREPAIR.LOCAL', 1, N'AQAAAAIAAYagAAAAEEN7Q1PYYpZ/j108Jysxym81OjieSAE9/AX8RnXTbLAKdkDPDEhUvUyd21ytALpdhQ==', N'ONSXBUB3GF2LED3LYHRYJ7UPPYUXJXOA', N'62431f7e-e265-47f8-87ef-0d3235f1a540', NULL, 0, 0, NULL, 1, 0, N'', 0, N'', N'0001-01-01 00:00:00', NULL, NULL, NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUsers WHERE [Id] = N'570137dd-beff-4f45-a911-d3f8748c14d2')
            INSERT INTO dbo.AspNetUsers ([Id], [FullName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Department], [IsActive], [Section], [CreatedAt], [CreatedByName], [UpdatedAt], [UpdatedByName]) VALUES (N'570137dd-beff-4f45-a911-d3f8748c14d2', N'Mallika Phonok', N'ASI006038@stanley-electic.com', N'ASI006038@STANLEY-ELECTIC.COM', N'ASI006038@stanley-electic.com', N'ASI006038@STANLEY-ELECTIC.COM', 1, N'AQAAAAIAAYagAAAAEC5Sxof2egYDIQKE2gSZra8GWkdwn+X5d0hlqI0FhFEdzqXsS6rcWf7RU6SCWWGEsQ==', N'6DTUR45XDTCGYMNKFGWBJWLIC6AY2HD7', N'923ab31e-b75c-4c50-9544-4a8f266a6e40', NULL, 0, 0, NULL, 1, 1, N'DX Tecnology', 0, N'Enterprise System', N'0001-01-01 00:00:00', NULL, N'2026-04-30 08:26:20.7975604', N'ASI006038');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUsers WHERE [Id] = N'687f33bd-2052-4355-b6c7-fd3fbf7508cd')
            INSERT INTO dbo.AspNetUsers ([Id], [FullName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Department], [IsActive], [Section], [CreatedAt], [CreatedByName], [UpdatedAt], [UpdatedByName]) VALUES (N'687f33bd-2052-4355-b6c7-fd3fbf7508cd', N'Chadagon', N'ASI002525@stanley-electic.com', N'ASI002525@STANLEY-ELECTIC.COM', N'ASI002525@stanley-electic.com', N'ASI002525@STANLEY-ELECTIC.COM', 1, N'AQAAAAIAAYagAAAAEHIuda2VGD8NflGU78KogG3ZJmhisbAFOk0PWfxOhwmegCss1+PQ2/XRHnQHb93R/g==', N'RETOEPJXKNQEFDXMUOY73XPFHKEYFKM7', N'1f0e2cc0-97a1-4424-ad11-67d004dbfe0a', NULL, 0, 0, NULL, 1, 0, N'DX Tecnology', 1, N'Enterprise System', N'0001-01-01 00:00:00', NULL, NULL, NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUsers WHERE [Id] = N'fd698db2-e8b1-422c-a24e-ab510f84b564')
            INSERT INTO dbo.AspNetUsers ([Id], [FullName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Department], [IsActive], [Section], [CreatedAt], [CreatedByName], [UpdatedAt], [UpdatedByName]) VALUES (N'fd698db2-e8b1-422c-a24e-ab510f84b564', N'Productin1', N'Production@stanley-electric.com', N'PRODUCTION@STANLEY-ELECTRIC.COM', N'Production@stanley-electric.com', N'PRODUCTION@STANLEY-ELECTRIC.COM', 1, N'AQAAAAIAAYagAAAAEAXye5IC37bhjzOm1Z8EtCxlp2JyJVbzm14qyycRjurNgk0hjMYQGJMDqvWjHG5qEg==', N'DOHEQ2Y4F7XVITVIUUWTWZ5TODKKNMIY', N'94384f49-5f45-4ad2-92d7-2907b3977b47', NULL, 0, 0, NULL, 1, 0, N'Production', 1, N'Planning', N'0001-01-01 00:00:00', NULL, NULL, NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUsers WHERE [Id] = N'fbce7156-e80c-49f4-8bad-c35538dcd937')
            INSERT INTO dbo.AspNetUsers ([Id], [FullName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Department], [IsActive], [Section], [CreatedAt], [CreatedByName], [UpdatedAt], [UpdatedByName]) VALUES (N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1', N'marketing@stanley-electric.com', N'MARKETING@STANLEY-ELECTRIC.COM', N'marketing@stanley-electric.com', N'MARKETING@STANLEY-ELECTRIC.COM', 1, N'AQAAAAIAAYagAAAAEBcF833vFiDPLWa6aC2xIXpvslN6ja2u3Wnw5u9ABWbGkzRnKkJ4mh9IluaV7NSVLw==', N'UBJVUKIXWSFN2JDVY3DYTLNFF4YARS7W', N'0c82aaf4-364a-475d-8935-f3749f12f3db', NULL, 0, 0, NULL, 1, 0, N'Marketing', 1, N'New Model', N'0001-01-01 00:00:00', NULL, NULL, NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUsers WHERE [Id] = N'424420dc-bb65-47f2-80b3-9036d4b21c20')
            INSERT INTO dbo.AspNetUsers ([Id], [FullName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Department], [IsActive], [Section], [CreatedAt], [CreatedByName], [UpdatedAt], [UpdatedByName]) VALUES (N'424420dc-bb65-47f2-80b3-9036d4b21c20', N'Productin dm', N'Production.sp@stanley-electric.com', N'PRODUCTION.SP@STANLEY-ELECTRIC.COM', N'Production.sp@stanley-electric.com', N'PRODUCTION.SP@STANLEY-ELECTRIC.COM', 1, N'AQAAAAIAAYagAAAAEOcYV3AMvn/9ZTyrz9bpdh8WPNl/EkVTk/TuBmLQvfFpBgBnKKl5QMlGLGctaX25Vg==', N'6OE6TD7TMFREOLFQZYMWKKH5SATY7NC6', N'e93864a0-a94e-4825-bf82-e60edb6951c5', NULL, 0, 0, NULL, 1, 0, N'Production', 1, N'Planning', N'2026-04-27 08:31:27.1835688', N'Mallika Phonok', N'2026-04-27 08:31:27.1835688', N'Mallika Phonok');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUsers WHERE [Id] = N'9fcb13b7-0c85-483a-8b6b-1e826d4f4a24')
            INSERT INTO dbo.AspNetUsers ([Id], [FullName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Department], [IsActive], [Section], [CreatedAt], [CreatedByName], [UpdatedAt], [UpdatedByName]) VALUES (N'9fcb13b7-0c85-483a-8b6b-1e826d4f4a24', N'Marketing DM', N'marketing.su@stanley-electric.com', N'MARKETING.SU@STANLEY-ELECTRIC.COM', N'marketing.su@stanley-electric.com', N'MARKETING.SU@STANLEY-ELECTRIC.COM', 1, N'AQAAAAIAAYagAAAAEO6TEEYFZBJk09Ke0+AkzdBTCuHmQT/0kX3ttPqmByasRNaeLBT/TR012bbLW//o8A==', N'NMWDSNWGVRY2Q3DVL6RAYWVWEC7KOHKE', N'10e491a3-b8f1-4c60-b698-d81558321f3e', NULL, 0, 0, NULL, 1, 0, N'Marketing', 1, N'New Model', N'2026-04-27 09:37:27.1800929', N'Mallika Phonok', N'2026-04-27 09:37:27.1800929', N'Mallika Phonok');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUsers WHERE [Id] = N'c2b8347a-3d5d-4a3c-a00b-bb893cf6a808')
            INSERT INTO dbo.AspNetUsers ([Id], [FullName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Department], [IsActive], [Section], [CreatedAt], [CreatedByName], [UpdatedAt], [UpdatedByName]) VALUES (N'c2b8347a-3d5d-4a3c-a00b-bb893cf6a808', N'Administrator', N'admin@example.com', N'ADMIN@EXAMPLE.COM', N'admin@example.com', N'ADMIN@EXAMPLE.COM', 1, N'AQAAAAIAAYagAAAAEGxyiH6YEV6r0IoBGtPnBLmtqCLYyC56U3ljThhsVfgJasQkqQxy4dnD1gnYn8OWZQ==', N'CKNHQZUMGAJEP4FXMIEW4S5G6ZZ33CET', N'e079250d-821a-4b1f-92c1-09ecde5546df', NULL, 0, 0, NULL, 1, 0, N'', 1, N'', N'2026-04-29 06:40:03.1654073', N'Administrator', N'2026-04-29 06:40:03.1654319', N'Administrator');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUsers WHERE [Id] = N'cc3e7df7-f5f0-4df9-af2c-4117571d88ce')
            INSERT INTO dbo.AspNetUsers ([Id], [FullName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Department], [IsActive], [Section], [CreatedAt], [CreatedByName], [UpdatedAt], [UpdatedByName]) VALUES (N'cc3e7df7-f5f0-4df9-af2c-4117571d88ce', N'Mallika Phonok', N'ASI006038', N'ASI006038', N'ASI0060381@stanley-electic.com', N'ASI0060381@STANLEY-ELECTIC.COM', 1, N'AQAAAAIAAYagAAAAEBSaKQth1WpFMWmUgoEhigg0np848bm3gMRuaGj/VaatRyzAxY4jNWWODCZvXQ/Kzg==', N'7S3CU54E6QFQDPM3JVH3MHTCYCR34YTP', N'53a668e6-314e-4332-b1e9-7bef30b1d68f', NULL, 0, 0, NULL, 1, 0, N'DX Tecnology', 1, N'Enterprise System', N'2026-04-30 08:11:33.9435991', N'Administrator', N'2026-05-05 04:43:22.878851', N'Mallika Phonok');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUsers WHERE [Id] = N'f21c6d48-0689-4327-bf73-702fecd92055')
            INSERT INTO dbo.AspNetUsers ([Id], [FullName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Department], [IsActive], [Section], [CreatedAt], [CreatedByName], [UpdatedAt], [UpdatedByName]) VALUES (N'f21c6d48-0689-4327-bf73-702fecd92055', N'Finance', N'ASI000003', N'ASI000003', N'Finance@stanley-electric.com', N'FINANCE@STANLEY-ELECTRIC.COM', 1, N'AQAAAAIAAYagAAAAEHhZq0A28wmB7pSj5vmjsoxys9Wu5isfjnJe8iG2mBLHcmQi0bS/kS+mXzu5Nabh9Q==', N'KJC5KLEUAGHXDPDFCIR7PFARPQC7K6YU', N'868096c1-0dce-45f9-9f40-7d87dca02209', NULL, 0, 0, NULL, 1, 0, N'Finance', 1, N'Finance', N'2026-05-05 05:40:23.4322084', N'Mallika Phonok', N'2026-05-05 05:40:23.4322084', N'Mallika Phonok');
    END;

    IF OBJECT_ID(N'dbo.AspNetUserRoles', N'U') IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'c53a0ce7-9c56-44e9-9cb6-aabb35928853' AND [RoleId] = N'dccef850-a86f-4eb8-9d55-d7f9121aa458')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'c53a0ce7-9c56-44e9-9cb6-aabb35928853', N'dccef850-a86f-4eb8-9d55-d7f9121aa458');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'570137dd-beff-4f45-a911-d3f8748c14d2' AND [RoleId] = N'29df7d0c-8e4a-4789-8e4a-ea44a9dc2905')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'570137dd-beff-4f45-a911-d3f8748c14d2', N'29df7d0c-8e4a-4789-8e4a-ea44a9dc2905');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'570137dd-beff-4f45-a911-d3f8748c14d2' AND [RoleId] = N'dccef850-a86f-4eb8-9d55-d7f9121aa458')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'570137dd-beff-4f45-a911-d3f8748c14d2', N'dccef850-a86f-4eb8-9d55-d7f9121aa458');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'570137dd-beff-4f45-a911-d3f8748c14d2' AND [RoleId] = N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'570137dd-beff-4f45-a911-d3f8748c14d2', N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'687f33bd-2052-4355-b6c7-fd3fbf7508cd' AND [RoleId] = N'1e610076-7eb1-42e8-9f99-d270a9c1d64c')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'687f33bd-2052-4355-b6c7-fd3fbf7508cd', N'1e610076-7eb1-42e8-9f99-d270a9c1d64c');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'687f33bd-2052-4355-b6c7-fd3fbf7508cd' AND [RoleId] = N'29df7d0c-8e4a-4789-8e4a-ea44a9dc2905')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'687f33bd-2052-4355-b6c7-fd3fbf7508cd', N'29df7d0c-8e4a-4789-8e4a-ea44a9dc2905');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'687f33bd-2052-4355-b6c7-fd3fbf7508cd' AND [RoleId] = N'dccef850-a86f-4eb8-9d55-d7f9121aa458')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'687f33bd-2052-4355-b6c7-fd3fbf7508cd', N'dccef850-a86f-4eb8-9d55-d7f9121aa458');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'fd698db2-e8b1-422c-a24e-ab510f84b564' AND [RoleId] = N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'fd698db2-e8b1-422c-a24e-ab510f84b564', N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'fbce7156-e80c-49f4-8bad-c35538dcd937' AND [RoleId] = N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'424420dc-bb65-47f2-80b3-9036d4b21c20' AND [RoleId] = N'1e610076-7eb1-42e8-9f99-d270a9c1d64c')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'424420dc-bb65-47f2-80b3-9036d4b21c20', N'1e610076-7eb1-42e8-9f99-d270a9c1d64c');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'424420dc-bb65-47f2-80b3-9036d4b21c20' AND [RoleId] = N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'424420dc-bb65-47f2-80b3-9036d4b21c20', N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'9fcb13b7-0c85-483a-8b6b-1e826d4f4a24' AND [RoleId] = N'1e610076-7eb1-42e8-9f99-d270a9c1d64c')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'9fcb13b7-0c85-483a-8b6b-1e826d4f4a24', N'1e610076-7eb1-42e8-9f99-d270a9c1d64c');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'9fcb13b7-0c85-483a-8b6b-1e826d4f4a24' AND [RoleId] = N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'9fcb13b7-0c85-483a-8b6b-1e826d4f4a24', N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'c2b8347a-3d5d-4a3c-a00b-bb893cf6a808' AND [RoleId] = N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'c2b8347a-3d5d-4a3c-a00b-bb893cf6a808', N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'c2b8347a-3d5d-4a3c-a00b-bb893cf6a808' AND [RoleId] = N'dccef850-a86f-4eb8-9d55-d7f9121aa458')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'c2b8347a-3d5d-4a3c-a00b-bb893cf6a808', N'dccef850-a86f-4eb8-9d55-d7f9121aa458');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'cc3e7df7-f5f0-4df9-af2c-4117571d88ce' AND [RoleId] = N'29df7d0c-8e4a-4789-8e4a-ea44a9dc2905')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'cc3e7df7-f5f0-4df9-af2c-4117571d88ce', N'29df7d0c-8e4a-4789-8e4a-ea44a9dc2905');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'cc3e7df7-f5f0-4df9-af2c-4117571d88ce' AND [RoleId] = N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'cc3e7df7-f5f0-4df9-af2c-4117571d88ce', N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'cc3e7df7-f5f0-4df9-af2c-4117571d88ce' AND [RoleId] = N'dccef850-a86f-4eb8-9d55-d7f9121aa458')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'cc3e7df7-f5f0-4df9-af2c-4117571d88ce', N'dccef850-a86f-4eb8-9d55-d7f9121aa458');
        IF NOT EXISTS (SELECT 1 FROM dbo.AspNetUserRoles WHERE [UserId] = N'f21c6d48-0689-4327-bf73-702fecd92055' AND [RoleId] = N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47')
            INSERT INTO dbo.AspNetUserRoles ([UserId], [RoleId]) VALUES (N'f21c6d48-0689-4327-bf73-702fecd92055', N'af86f0d0-c6e8-4eb6-ae9a-f5e37fc0aa47');
    END;

    IF OBJECT_ID(N'dbo.NewsItems', N'U') IS NOT NULL
    BEGIN
        SET IDENTITY_INSERT dbo.NewsItems ON;
        IF NOT EXISTS (SELECT 1 FROM dbo.NewsItems WHERE [Id] = 1)
            INSERT INTO dbo.NewsItems ([Id], [Title], [Content], [CreatedByName], [CreatedAt], [AttachmentFileName], [AttachmentUrl], [UpdatedAt], [UpdatedByName], [IsActive]) VALUES (1, N'ประกาศ ผู้ถือ Ipad', N'นำ IPad มาลงโปรแกรม', N'Mallika Phonok', N'2026-04-27 05:49:27.1301961', N'SR-264-009.pdf', N'/uploads/news/72e38308ac974ba49a4b72c059bb3323.pdf', N'2026-04-30 07:38:35.0631553', N'Mallika Phonok', 0);
        SET IDENTITY_INSERT dbo.NewsItems OFF;
    END;

    IF OBJECT_ID(N'dbo.RepairTickets', N'U') IS NOT NULL
    BEGIN
        SET IDENTITY_INSERT dbo.RepairTickets ON;
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTickets WHERE [Id] = 1)
            INSERT INTO dbo.RepairTickets ([Id], [RequesterName], [Department], [DeviceName], [IssueDescription], [Priority], [Status], [CreatedAt], [UpdatedAt], [ApproverDepartment], [ApproverName], [ApproverUserId], [RepairType], [RequesterUserId], [CreatedByName], [UpdatedByName], [AssignedItName], [AssignedItUserId], [DocumentNo]) VALUES (1, N'Marketing1', N'Marketing', N'N/A', N'คอมพิวเตอร์เสีย



[Returned to requester - 2026-04-25 14:25 UTC] ไม่สามารถดำเนินการได้



[Returned to requester - 2026-04-25 14:27 UTC] no



[Returned to requester - 2026-04-27 01:04 UTC] ไม่สามารถดำเนินการได้



[Returned to requester - 2026-04-27 01:33 UTC] ไม่สามารถดำเนินการได้', N'Medium', N'Closed', N'2026-04-25 13:29:34', N'2026-04-27 05:07:55.2866991', N'DX Tecnology', N'Chadagon', N'687f33bd-2052-4355-b6c7-fd3fbf7508cd', N'Hardware', NULL, NULL, N'Marketing1', N'', N'', NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTickets WHERE [Id] = 2)
            INSERT INTO dbo.RepairTickets ([Id], [RequesterName], [Department], [DeviceName], [IssueDescription], [Priority], [Status], [CreatedAt], [UpdatedAt], [ApproverDepartment], [ApproverName], [ApproverUserId], [RepairType], [RequesterUserId], [CreatedByName], [UpdatedByName], [AssignedItName], [AssignedItUserId], [DocumentNo]) VALUES (2, N'Marketing1', N'Marketing', N'N/A', N'MS team ใช้งานไม่ได้', N'Medium', N'Closed', N'2026-04-27 05:15:26', N'2026-04-27 05:17:22.3260822', N'DX Tecnology', N'Chadagon', N'687f33bd-2052-4355-b6c7-fd3fbf7508cd', N'Software', N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1', N'Marketing1', N'', N'', NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTickets WHERE [Id] = 3)
            INSERT INTO dbo.RepairTickets ([Id], [RequesterName], [Department], [DeviceName], [IssueDescription], [Priority], [Status], [CreatedAt], [UpdatedAt], [ApproverDepartment], [ApproverName], [ApproverUserId], [RepairType], [RequesterUserId], [CreatedByName], [UpdatedByName], [AssignedItName], [AssignedItUserId], [DocumentNo]) VALUES (3, N'Marketing1', N'Marketing', N'N/A', N'ขอเข้า Drive O', N'Medium', N'InProgress', N'2026-04-27 08:32:38', N'2026-04-27 09:40:35.8199929', N'DX Tecnology', N'Productin dm', N'687f33bd-2052-4355-b6c7-fd3fbf7508cd', N'Other', N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1', N'Chadagon', N'Mallika Phonok', N'570137dd-beff-4f45-a911-d3f8748c14d2', NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTickets WHERE [Id] = 4)
            INSERT INTO dbo.RepairTickets ([Id], [RequesterName], [Department], [DeviceName], [IssueDescription], [Priority], [Status], [CreatedAt], [UpdatedAt], [ApproverDepartment], [ApproverName], [ApproverUserId], [RepairType], [RequesterUserId], [CreatedByName], [UpdatedByName], [AssignedItName], [AssignedItUserId], [DocumentNo]) VALUES (4, N'Marketing1', N'Marketing', N'N/A', N'ขอเข้า Drive O', N'Medium', N'Approved', N'2026-04-27 09:38:04', N'2026-04-27 09:39:32.1199514', N'DX Tecnology', N'Marketing DM', N'687f33bd-2052-4355-b6c7-fd3fbf7508cd', N'Hardware', N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1', N'Productin dm', N'', N'', NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTickets WHERE [Id] = 5)
            INSERT INTO dbo.RepairTickets ([Id], [RequesterName], [Department], [DeviceName], [IssueDescription], [Priority], [Status], [CreatedAt], [UpdatedAt], [ApproverDepartment], [ApproverName], [ApproverUserId], [RepairType], [RequesterUserId], [CreatedByName], [UpdatedByName], [AssignedItName], [AssignedItUserId], [DocumentNo]) VALUES (5, N'Marketing1', N'Marketing', N'N/A', N'ขอเข้า Drive /2', N'Medium', N'Approved', N'2026-04-27 09:46:31', N'2026-04-28 02:24:49.5812978', N'Production', N'Marketing DM', N'424420dc-bb65-47f2-80b3-9036d4b21c20', N'Hardware', N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1', N'Marketing DM', N'', N'', NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTickets WHERE [Id] = 6)
            INSERT INTO dbo.RepairTickets ([Id], [RequesterName], [Department], [DeviceName], [IssueDescription], [Priority], [Status], [CreatedAt], [UpdatedAt], [ApproverDepartment], [ApproverName], [ApproverUserId], [RepairType], [RequesterUserId], [CreatedByName], [UpdatedByName], [AssignedItName], [AssignedItUserId], [DocumentNo]) VALUES (6, N'Marketing1', N'Marketing', N'N/A', N'ขอเข้า Drive /3', N'Medium', N'InProgress', N'2026-04-27 09:47:02', N'2026-04-27 09:49:22.473809', N'DX Tecnology', N'Marketing DM', N'687f33bd-2052-4355-b6c7-fd3fbf7508cd', N'Other', N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1', N'Chadagon', N'Mallika Phonok', N'570137dd-beff-4f45-a911-d3f8748c14d2', NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTickets WHERE [Id] = 7)
            INSERT INTO dbo.RepairTickets ([Id], [RequesterName], [Department], [DeviceName], [IssueDescription], [Priority], [Status], [CreatedAt], [UpdatedAt], [ApproverDepartment], [ApproverName], [ApproverUserId], [RepairType], [RequesterUserId], [CreatedByName], [UpdatedByName], [AssignedItName], [AssignedItUserId], [DocumentNo]) VALUES (7, N'Marketing1', N'Marketing', N'N/A', N'ขอเข้าใช้ Drive O', N'Medium', N'Approved', N'2026-04-28 01:58:15', N'2026-04-28 02:00:33.6573036', N'DX Tecnology', N'Marketing DM', N'687f33bd-2052-4355-b6c7-fd3fbf7508cd', N'Other', N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1', N'Productin dm', N'', N'', NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTickets WHERE [Id] = 8)
            INSERT INTO dbo.RepairTickets ([Id], [RequesterName], [Department], [DeviceName], [IssueDescription], [Priority], [Status], [CreatedAt], [UpdatedAt], [ApproverDepartment], [ApproverName], [ApproverUserId], [RepairType], [RequesterUserId], [CreatedByName], [UpdatedByName], [AssignedItName], [AssignedItUserId], [DocumentNo]) VALUES (8, N'Marketing1', N'Marketing', N'N/A', N'Drive O', N'Medium', N'Closed', N'2026-04-28 02:58:57', N'2026-04-29 01:48:41.1570817', N'DX Tecnology', N'Marketing DM', N'687f33bd-2052-4355-b6c7-fd3fbf7508cd', N'Other', N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1', N'Mallika Phonok', N'', N'', NULL);
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTickets WHERE [Id] = 9)
            INSERT INTO dbo.RepairTickets ([Id], [RequesterName], [Department], [DeviceName], [IssueDescription], [Priority], [Status], [CreatedAt], [UpdatedAt], [ApproverDepartment], [ApproverName], [ApproverUserId], [RepairType], [RequesterUserId], [CreatedByName], [UpdatedByName], [AssignedItName], [AssignedItUserId], [DocumentNo]) VALUES (9, N'Marketing1', N'Marketing', N'N/A', N'เปลี่ยน RAM', N'Medium', N'Open', N'2026-04-29 01:54:18.610062', NULL, N'Marketing', N'Marketing DM', N'9fcb13b7-0c85-483a-8b6b-1e826d4f4a24', N'Hardware', N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1', NULL, N'', N'', N'SR-264-009');
        SET IDENTITY_INSERT dbo.RepairTickets OFF;
    END;

    IF OBJECT_ID(N'dbo.RepairTicketStatusHistories', N'U') IS NOT NULL
    BEGIN
        SET IDENTITY_INSERT dbo.RepairTicketStatusHistories ON;
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 1)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (1, 4, NULL, N'Open', N'Created', NULL, N'2026-04-27 09:38:04.9380856', N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 2)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (2, 4, N'Open', N'Approved', N'StatusChanged', NULL, N'2026-04-27 09:38:34.3453205', N'9fcb13b7-0c85-483a-8b6b-1e826d4f4a24', N'Marketing DM');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 3)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (3, 3, N'Open', N'Approved', N'StatusChanged', NULL, N'2026-04-27 09:39:37.3037345', N'424420dc-bb65-47f2-80b3-9036d4b21c20', N'Productin dm');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 4)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (4, 3, N'Approved', N'InProgress', N'StatusChanged', NULL, N'2026-04-27 09:40:35.819999', N'687f33bd-2052-4355-b6c7-fd3fbf7508cd', N'Chadagon');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 5)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (5, 5, NULL, N'Open', N'Created', NULL, N'2026-04-27 09:46:31.82179', N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 6)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (6, 6, NULL, N'Open', N'Created', NULL, N'2026-04-27 09:47:02.0489033', N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 7)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (7, 6, N'Open', N'Approved', N'StatusChanged', NULL, N'2026-04-27 09:47:55.4034299', N'9fcb13b7-0c85-483a-8b6b-1e826d4f4a24', N'Marketing DM');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 8)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (8, 6, N'Approved', N'InProgress', N'StatusChanged', NULL, N'2026-04-27 09:49:22.4738146', N'687f33bd-2052-4355-b6c7-fd3fbf7508cd', N'Chadagon');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 9)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (9, 7, NULL, N'Open', N'Created', NULL, N'2026-04-28 01:58:15.6545488', N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 10)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (10, 7, N'Open', N'Approved', N'StatusChanged', NULL, N'2026-04-28 02:00:04.8981367', N'9fcb13b7-0c85-483a-8b6b-1e826d4f4a24', N'Marketing DM');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 11)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (11, 7, N'Approved', N'Approved', N'Saved', NULL, N'2026-04-28 02:00:33.6576374', N'424420dc-bb65-47f2-80b3-9036d4b21c20', N'Productin dm');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 12)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (12, 5, N'Open', N'Approved', N'StatusChanged', NULL, N'2026-04-28 02:24:49.5819429', N'9fcb13b7-0c85-483a-8b6b-1e826d4f4a24', N'Marketing DM');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 13)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (13, 8, NULL, N'Open', N'Created', NULL, N'2026-04-28 02:58:57.5540906', N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 14)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (14, 8, N'Open', N'Approved', N'StatusChanged', NULL, N'2026-04-28 03:36:20.5513832', N'9fcb13b7-0c85-483a-8b6b-1e826d4f4a24', N'Marketing DM');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 15)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (15, 8, N'Approved', N'Approved', N'Saved', NULL, N'2026-04-28 06:46:46.1059008', N'424420dc-bb65-47f2-80b3-9036d4b21c20', N'Productin dm');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 16)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (16, 8, N'Approved', N'InProgress', N'StatusChanged', NULL, N'2026-04-28 06:58:03.3903841', N'687f33bd-2052-4355-b6c7-fd3fbf7508cd', N'Chadagon');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 17)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (17, 8, N'InProgress', N'Approved', N'StatusChanged', NULL, N'2026-04-29 01:20:00.9450196', N'570137dd-beff-4f45-a911-d3f8748c14d2', N'Mallika Phonok');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 18)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (18, 8, N'Approved', N'Complete', N'StatusChanged', NULL, N'2026-04-29 01:20:00.9955392', N'570137dd-beff-4f45-a911-d3f8748c14d2', N'Mallika Phonok');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 19)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (19, 8, N'Complete', N'Closed', N'Closed', NULL, N'2026-04-29 01:48:41.1575613', N'570137dd-beff-4f45-a911-d3f8748c14d2', N'Mallika Phonok');
        IF NOT EXISTS (SELECT 1 FROM dbo.RepairTicketStatusHistories WHERE [Id] = 20)
            INSERT INTO dbo.RepairTicketStatusHistories ([Id], [RepairTicketId], [FromStatus], [ToStatus], [Action], [Remark], [ChangedAt], [ChangedByUserId], [ChangedByName]) VALUES (20, 9, NULL, N'Open', N'Created', NULL, N'2026-04-29 01:54:18.6818807', N'fbce7156-e80c-49f4-8bad-c35538dcd937', N'Marketing1');
        SET IDENTITY_INSERT dbo.RepairTicketStatusHistories OFF;
    END;

    COMMIT TRANSACTION;
    PRINT N'Seed from SQLite completed successfully.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage nvarchar(4000) = ERROR_MESSAGE();
    DECLARE @ErrorLine int = ERROR_LINE();
    RAISERROR(N'Seed from SQLite failed at line %d: %s', 16, 1, @ErrorLine, @ErrorMessage);
END CATCH;
GO
