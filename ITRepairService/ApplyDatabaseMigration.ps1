# Stop the running application first, then run this script
Write-Host "Applying database migration..." -ForegroundColor Cyan

Set-Location "y:\git\DX_service\ITRepairService"

# Apply the migration
dotnet ef database update

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration applied successfully!" -ForegroundColor Green
    Write-Host "You can now restart the application." -ForegroundColor Yellow
} else {
    Write-Host "Migration failed. Please check the error above." -ForegroundColor Red
}

Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
