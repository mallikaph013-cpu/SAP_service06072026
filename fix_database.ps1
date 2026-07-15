# PowerShell script to add DriveAccessDepartment column to SQLite database
$databasePath = "ITRepairService/itrepair.db"

if (-not (Test-Path $databasePath)) {
    Write-Host "Database not found at: $databasePath" -ForegroundColor Red
    Write-Host "Searching for database files..."
    Get-ChildItem -Path . -Filter "*.db" -Recurse | Select-Object FullName
    exit 1
}

Write-Host "Connecting to database: $databasePath" -ForegroundColor Cyan

# Load SQLite assembly
Add-Type -Path "ITRepairService/bin/Debug/net9.0/Microsoft.Data.Sqlite.dll" -ErrorAction SilentlyContinue

if (-not (Test-Path "ITRepairService/bin/Debug/net9.0/Microsoft.Data.Sqlite.dll")) {
    Write-Host "Building project to get SQLite assembly..." -ForegroundColor Yellow
    Set-Location ITRepairService
    dotnet build
    Set-Location ..
}

# Use .NET to connect to SQLite
$connectionString = "Data Source=$databasePath"
$connection = New-Object System.Data.SQLite.SQLiteConnection($connectionString)

try {
    $connection.Open()
    Write-Host "✓ Connected to database" -ForegroundColor Green
    
    $command = $connection.CreateCommand()
    
    # Check current columns
    Write-Host "`nCurrent columns in RepairTicket table:" -ForegroundColor Cyan
    $command.CommandText = "PRAGMA table_info(RepairTicket)"
    $reader = $command.ExecuteReader()
    $columns = @()
    while ($reader.Read()) {
        $columns += $reader.GetString(1)
        Write-Host "  $($reader.GetString(1)) ($($reader.GetString(2)))"
    }
    $reader.Close()
    
    # Check if DriveAccessDepartment column exists
    if ($columns -contains "DriveAccessDepartment") {
        Write-Host "`n✓ DriveAccessDepartment column already exists!" -ForegroundColor Green
    } else {
        Write-Host "`n✗ DriveAccessDepartment column is missing. Adding it now..." -ForegroundColor Yellow
        $command.CommandText = "ALTER TABLE RepairTicket ADD COLUMN DriveAccessDepartment TEXT"
        $command.ExecuteNonQuery() | Out-Null
        Write-Host "✓ DriveAccessDepartment column added successfully!" -ForegroundColor Green
    }
    
    # Verify the change
    Write-Host "`nUpdated columns in RepairTicket table:" -ForegroundColor Cyan
    $command.CommandText = "PRAGMA table_info(RepairTicket)"
    $reader = $command.ExecuteReader()
    while ($reader.Read()) {
        Write-Host "  $($reader.GetString(1)) ($($reader.GetString(2)))"
    }
    $reader.Close()
    
} catch {
    Write-Host "`nError: $_" -ForegroundColor Red
} finally {
    $connection.Close()
    Write-Host "`nDatabase connection closed." -ForegroundColor Gray
}