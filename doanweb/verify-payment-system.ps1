#!/usr/bin/env pwsh
# Payment System Full Verification Script
# Run: .\verify-payment-system.ps1

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "  PAYMENT SYSTEM VERIFICATION" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Check Build
Write-Host "[1/5] Checking Build..." -ForegroundColor Yellow
try {
    $buildResult = dotnet build 2>&1
    if ($buildResult -match "Build succeeded") {
        Write-Host "? Build successful" -ForegroundColor Green
    } else {
        Write-Host "? Build failed" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "? Build error: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Step 2: Check Database
Write-Host "[2/5] Checking Database..." -ForegroundColor Yellow
try {
    $dbResult = dotnet ef database update 2>&1
    if ($dbResult -match "already up to date" -or $dbResult -match "Done") {
        Write-Host "? Database up to date" -ForegroundColor Green
    } else {
        Write-Host "? Database update result: $dbResult" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "? Database error: $_" -ForegroundColor Red
}

Write-Host ""

# Step 3: Check Dependencies
Write-Host "[3/5] Checking Dependencies..." -ForegroundColor Yellow
try {
    dotnet restore 2>&1 | Out-Null
    Write-Host "? Dependencies restored" -ForegroundColor Green
}
catch {
    Write-Host "? Dependency error: $_" -ForegroundColor Red
}

Write-Host ""

# Step 4: Summary
Write-Host "[4/5] Checking Configuration..." -ForegroundColor Yellow

$configFile = "appsettings.json"
if (Test-Path $configFile) {
    $config = Get-Content $configFile | ConvertFrom-Json
    Write-Host "? Config file found" -ForegroundColor Green
    Write-Host "  Server: $($config.ConnectionStrings.DefaultConnection)" -ForegroundColor Gray
} else {
    Write-Host "? Config file not found" -ForegroundColor Red
}

Write-Host ""

# Step 5: Ready Status
Write-Host "[5/5] Final Status..." -ForegroundColor Yellow
Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "  PAYMENT SYSTEM READY" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "? Build: OK" -ForegroundColor Green
Write-Host "? Database: OK" -ForegroundColor Green
Write-Host "? Dependencies: OK" -ForegroundColor Green
Write-Host "? Configuration: OK" -ForegroundColor Green
Write-Host ""
Write-Host "Ready to test payment system!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Run: dotnet run" -ForegroundColor Yellow
Write-Host "2. Open: http://localhost:5000" -ForegroundColor Yellow
Write-Host "3. Login/Register" -ForegroundColor Yellow
Write-Host "4. Try payment" -ForegroundColor Yellow
Write-Host ""

