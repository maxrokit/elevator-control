#!/usr/bin/env pwsh
# Export OpenAPI/Swagger JSON for Postman import

Write-Host "Exporting OpenAPI JSON for Postman..." -ForegroundColor Cyan

# Kill any process using port 8080
$procs = netstat -ano | findstr :8080 | ForEach-Object { $_ -match '\s+(\d+)$' | Out-Null; $matches[1] } | Select-Object -Unique
foreach ($p in $procs) { 
    Stop-Process -Id $p -Force -ErrorAction SilentlyContinue 
}
Start-Sleep -Seconds 1

# Start API in background with Development environment
$env:ASPNETCORE_ENVIRONMENT = "Development"
$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project src\Api\Api.csproj" -PassThru -NoNewWindow

Write-Host "Waiting for API to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Download OpenAPI JSON
try {
    $outputFile = "ElevatorControl-OpenAPI.json"
    Write-Host "Downloading OpenAPI specification..." -ForegroundColor Cyan
    
    Invoke-WebRequest -Uri "http://localhost:8080/swagger/v1/swagger.json" -OutFile $outputFile
    
    Write-Host "Successfully exported to: $outputFile" -ForegroundColor Green
    Write-Host ""
    Write-Host "To import into Postman:" -ForegroundColor Cyan
    Write-Host "1. Open Postman" -ForegroundColor White
    Write-Host "2. Click 'Import' button" -ForegroundColor White
    Write-Host "3. Select '$outputFile'" -ForegroundColor White
    Write-Host "4. Click 'Import'" -ForegroundColor White
}
catch {
    Write-Host "Failed to download OpenAPI JSON: $_" -ForegroundColor Red
    Write-Host "Make sure the API is running in Development mode." -ForegroundColor Yellow
}
finally {
    # Stop the API
    Write-Host ""
    Write-Host "Stopping API..." -ForegroundColor Yellow
    Stop-Process -Id $apiProcess.Id -Force -ErrorAction SilentlyContinue
    Write-Host "Done." -ForegroundColor Green
}
