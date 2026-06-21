@echo off
REM Database Update Script for DOANWEB
REM This script will update your database automatically

echo.
echo =========================================
echo   DATABASE UPDATE SCRIPT - DOANWEB
echo =========================================
echo.

REM Check if dotnet is installed
dotnet --version > nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET CLI is not installed
    echo Download from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [1/5] Checking .NET version...
dotnet --version
echo.

echo [2/5] Checking Entity Framework Tools...
dotnet ef --version > nul 2>&1
if errorlevel 1 (
    echo Installing Entity Framework CLI...
    dotnet tool install --global dotnet-ef
)
echo.

echo [3/5] Checking database connection...
dotnet ef database info
if errorlevel 1 (
    echo.
    echo WARNING: Database connection might have issues
    echo Check your appsettings.json file
    echo.
)
echo.

echo [4/5] Updating database...
dotnet ef database update
if errorlevel 1 (
    echo.
    echo ERROR: Database update failed!
    echo Try these steps:
    echo 1. Ensure SQL Server is running
    echo 2. Check server name in appsettings.json
    echo 3. Run: dotnet ef database drop -f
    echo 4. Run: dotnet ef database update
    pause
    exit /b 1
)
echo.

echo [5/5] Database update completed!
echo.
echo =========================================
echo   SUCCESS! Database updated.
echo =========================================
echo.
echo Your database is now ready to use!
echo.
pause
