# PowerShell script to add DriveAccessDepartment column
# This script uses .NET SQLite library directly

$databasePath = "ITRepairService/itrepair.db"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Adding DriveAccessDepartment Column" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path $databasePath)) {
    Write-Host "✗ Database not found at: $databasePath" -ForegroundColor Red
    exit 1
}

Write-Host "Database found: $databasePath" -ForegroundColor Green
Write-Host ""

# Try to load SQLite from the project's bin folder
$sqliteDll = Get-ChildItem -Path "ITRepairService" -Filter "Microsoft.Data.Sqlite.dll" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1

if ($sqliteDll) {
    Write-Host "Loading SQLite from: $($sqliteDll.FullName)" -ForegroundColor Gray
    Add-Type -Path $sqliteDll.FullName
} else {
    Write-Host "SQLite DLL not found in project. Trying to install..." -ForegroundColor Yellow
    Write-Host "Please wait..." -ForegroundColor Gray
    
    # Use dotnet to install the package temporarily
    Set-Location ITRepairService
    dotnet add package Microsoft.Data.Sqlite --version 9.0.4
    Set-Location ..
    
    $sqliteDll = Get-ChildItem -Path "ITRepairService" -Filter "Microsoft.Data.Sqlite.dll" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($sqliteDll) {
        Add-Type -Path $sqliteDll.FullName
    } else {
        Write-Host "✗ Could not load SQLite library" -ForegroundColor Red
        exit 1
    }
}

Write-Host "✓ SQLite library loaded" -ForegroundColor Green
Write-Host ""

try {
    $connectionString = "Data Source=$databasePath"
    $connection = New-Object Microsoft.Data.Sqlite.SqliteConnection($connectionString)
    $connection.Open()
    
    Write-Host "✓ Connected to database" -ForegroundColor Green
    Write-Host ""
    
    $command = $connection.CreateCommand()
    
    # Check current columns
    Write-Host "Current columns in RepairTicket table:" -ForegroundColor Cyan
    $command.CommandText = "PRAGMA table_info(RepairTicket)"
    $reader = $command.ExecuteReader()
    $columns = @()
    while ($reader.Read()) {
        $columns += $reader.GetString(1)
        Write-Host "  $($reader.GetString(1)) ($($reader.GetString(2)))"
    }
    $reader.Close()
    Write-Host ""
    
    # Check if DriveAccessDepartment column exists
    if ($columns -contains "DriveAccessDepartment") {
        Write-Host "✓ DriveAccessDepartment column already exists!" -ForegroundColor Green
    } else {
        Write-Host "✗ DriveAccessDepartment column is missing. Adding it now..." -ForegroundColor Yellow
        $command.CommandText = "ALTER TABLE RepairTicket ADD COLUMN DriveAccessDepartment TEXT"
        $command.ExecuteNonQuery() | Out-Null
        Write-Host "✓ DriveAccessDepartment column added successfully!" -ForegroundColor Green
    }
    Write-Host ""
    
    # Verify the change
    Write-Host "Updated columns in RepairTicket table:" -ForegroundColor Cyan
    $command.CommandText = "PRAGMA table_info(RepairTicket)"
    $reader = $command.ExecuteReader()
    while ($reader.Read()) {
        Write-Host "  $($reader.GetString(1)) ($($reader.GetString(2)))"
    }
    $reader.Close()
    
    $connection.Close()
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "✓ Database fix completed successfully!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "You can now start the application." -ForegroundColor Yellow
    
} catch {
    Write-Host ""
    Write-Host "✗ Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Try running as Administrator" -ForegroundColor Yellow
}

Write-Host ""
Read-Host "Press Enter to exit"