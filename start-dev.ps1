#!/usr/bin/env pwsh
# Build and run the Elevator Control API, then open the test client

# Kill any process using port 8080
Write-Host "Checking for processes on port 8080..." -ForegroundColor Cyan
$procs = netstat -ano | findstr :8080 | ForEach-Object { $_ -match '\s+(\d+)$' | Out-Null; $matches[1] } | Select-Object -Unique
foreach ($p in $procs) { 
    Write-Host "Stopping process $p..." -ForegroundColor Yellow
    Stop-Process -Id $p -Force -ErrorAction SilentlyContinue 
}
Start-Sleep -Seconds 1

Write-Host "Cleaning API..." -ForegroundColor Cyan
dotnet clean src\Api\Api.csproj | Out-Null

Write-Host "Building API..." -ForegroundColor Cyan
dotnet build src\Api\Api.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed. Aborting." -ForegroundColor Red
    exit 1
}

Write-Host "Build successful. Opening test client..." -ForegroundColor Green

# Open the static test client in default browser
$clientPath = Join-Path $PSScriptRoot "src\Client\index.html"
Start-Process $clientPath

Write-Host "`nStarting API in Development mode..." -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop the API`n" -ForegroundColor Yellow

# Set environment and run API in foreground (Ctrl+C will stop it)
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project src\Api\Api.csproj
