@echo off
chcp 65001 >nul
echo Applying database migration...
echo.

cd /d ITRepairService

echo Building project first...
dotnet build

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ✗ Build failed. Please check the errors above.
    pause
    exit /b 1
)

echo.
echo Running: dotnet ef database update
dotnet ef database update --msbuildprojectextensionspath obj --verbose

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✓ Migration applied successfully!
) else (
    echo.
    echo ✗ Migration failed. Trying alternative method...
    echo.
    
    echo Building project...
    dotnet build
    
    echo.
    echo Running migration again...
    dotnet ef database update
)

echo.
echo Press any key to exit...
pause >nul