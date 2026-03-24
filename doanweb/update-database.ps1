#!/usr/bin/env pwsh
# Database Update Script for DOANWEB (PowerShell)
# Run: .\update-database.ps1

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "  DATABASE UPDATE SCRIPT - DOANWEB" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Check if dotnet is installed
try {
    $dotnetVersion = dotnet --version
    Write-Host "[1/5] .NET Version: $dotnetVersion" -ForegroundColor Green
}
catch {
    Write-Host "[ERROR] .NET CLI not installed" -ForegroundColor Red
    Write-Host "Download from: https://dotnet.microsoft.com/download"
    exit 1
}

Write-Host ""

# Check Entity Framework CLI
Write-Host "[2/5] Checking Entity Framework CLI..." -ForegroundColor Yellow
try {
    dotnet ef --version | Out-Null
    Write-Host "? EF CLI installed" -ForegroundColor Green
}
catch {
    Write-Host "Installing EF CLI..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
}

Write-Host ""

# Check database connection
Write-Host "[3/5] Checking database connection..." -ForegroundColor Yellow
try {
    $dbInfo = dotnet ef database info
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Database connection OK" -ForegroundColor Green
        Write-Host $dbInfo
    }
    else {
        Write-Host "? Database connection issue" -ForegroundColor Yellow
        Write-Host "Check appsettings.json file" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "? Could not verify database" -ForegroundColor Yellow
}

Write-Host ""

# Update database
Write-Host "[4/5] Updating database..." -ForegroundColor Yellow
try {
    $updateResult = dotnet ef database update 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Database updated successfully" -ForegroundColor Green
        Write-Host $updateResult
    }
    else {
        Write-Host "? Database update failed" -ForegroundColor Red
        Write-Host $updateResult
        Write-Host ""
        Write-Host "Try these steps:" -ForegroundColor Yellow
        Write-Host "1. Ensure SQL Server is running" -ForegroundColor Yellow
        Write-Host "2. Check server name in appsettings.json" -ForegroundColor Yellow
        Write-Host "3. Run: dotnet ef database drop -f" -ForegroundColor Yellow
        Write-Host "4. Run: dotnet ef database update" -ForegroundColor Yellow
        exit 1
    }
}
catch {
    Write-Host "? Error during database update" -ForegroundColor Red
    Write-Host $_.Exception.Message
    exit 1
}

Write-Host ""

# Final message
Write-Host "[5/5] Verification..." -ForegroundColor Yellow
Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "  SUCCESS! Database updated." -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "? Your database is ready to use!" -ForegroundColor Green
Write-Host ""
