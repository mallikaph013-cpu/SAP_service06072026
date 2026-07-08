# IT Repair Service (ASP.NET Core MVC)

Production-ready IT repair ticket system using ASP.NET Core MVC, Identity, and Entity Framework Core.

## Features

- Login and registration
- Role-based authorization with 3 roles:
  - User
  - ITSupport
  - Admin
- Ticket CRUD with permission control
  - User: create/view tickets
  - ITSupport: edit tickets
  - Admin: full access including delete
- Dual database support:
  - SQLite
  - SQL Server
- Separate migrations projects per provider

## Solution Structure

- ITRepairService: main web app
- ITRepairService.Migrations.Sqlite: SQLite migrations assembly
- ITRepairService.Migrations.SqlServer: SQL Server migrations assembly

## Configuration

Edit appsettings.json:

- DatabaseProvider: Sqlite or SqlServer
- ConnectionStrings: SqliteConnection / SqlServerConnection
- SeedAdmin: Email and Password (optional)

If SeedAdmin values are set, startup seeds roles and creates an Admin user.

## First-Time Setup

1. Restore and build

```bash
dotnet restore
dotnet build ITRepairService.sln
```

2. Apply migrations (choose one provider)

SQLite:

```bash
dotnet dotnet-ef database update \
  --project ../ITRepairService.Migrations.Sqlite/ITRepairService.Migrations.Sqlite.csproj \
  --startup-project ../ITRepairService.Migrations.Sqlite/ITRepairService.Migrations.Sqlite.csproj \
  --context AppDbContext
```

SQL Server:

```bash
dotnet dotnet-ef database update \
  --project ../ITRepairService.Migrations.SqlServer/ITRepairService.Migrations.SqlServer.csproj \
  --startup-project ../ITRepairService.Migrations.SqlServer/ITRepairService.Migrations.SqlServer.csproj \
  --context AppDbContext
```

3. Run app

```bash
dotnet run --project ITRepairService.csproj
```

## Migrations Workflow

Create a new SQLite migration:

```bash
dotnet dotnet-ef migrations add <MigrationName> \
  --project ../ITRepairService.Migrations.Sqlite/ITRepairService.Migrations.Sqlite.csproj \
  --startup-project ../ITRepairService.Migrations.Sqlite/ITRepairService.Migrations.Sqlite.csproj \
  --context AppDbContext \
  --output-dir Migrations
```

Create a new SQL Server migration:

```bash
dotnet dotnet-ef migrations add <MigrationName> \
  --project ../ITRepairService.Migrations.SqlServer/ITRepairService.Migrations.SqlServer.csproj \
  --startup-project ../ITRepairService.Migrations.SqlServer/ITRepairService.Migrations.SqlServer.csproj \
  --context AppDbContext \
  --output-dir Migrations
```

## Deployment SQL Scripts

Generated scripts are available in scripts:

- scripts/sqlite-init.sql
- scripts/sqlserver-init.sql
