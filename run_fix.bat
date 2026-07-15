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

echo Step 2: Building and running SimpleFix...
cd /d SimpleFix
dotnet run

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo ✓ Database fix completed!
    echo ========================================
) else (
    echo.
    echo ✗ Failed to run SimpleFix
)

echo.
pause