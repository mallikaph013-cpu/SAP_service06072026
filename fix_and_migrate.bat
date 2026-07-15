@echo off
chcp 65001 >nul
echo ========================================
echo Fix Database - Add DriveAccessDepartment
echo ========================================
echo.

echo Step 1: Stopping running .NET processes...
taskkill /F /IM dotnet.exe >nul 2>&1
timeout /t 2 /nobreak >nul
echo ✓ Processes stopped
echo.

echo Step 2: Building project...
cd /d ITRepairService
dotnet build
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ✗ Build failed. Please check the errors above.
    pause
    exit /b 1
)
echo ✓ Build successful
echo.

echo Step 3: Applying database migration...
dotnet ef database update --msbuildprojectextensionspath obj
if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo ✓ Migration applied successfully!
    echo ========================================
    echo.
    echo The column 'DriveAccessDepartment' has been added to the RepairTicket table.
    echo You can now start the application.
) else (
    echo.
    echo ✗ Migration failed. Please check the error above.
)

echo.
pause