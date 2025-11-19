# Windows Service installation script for FormXChange.Api
# Requires: Administrator privileges
# Usage: .\FormXChange.Api.service.ps1 install|uninstall|start|stop|restart

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("install", "uninstall", "start", "stop", "restart")]
    [string]$Action
)

$ServiceName = "FormXChange.Api"
$DisplayName = "FormXChange API Service"
$Description = "FormXChange Suite API Gateway Service"
$ServicePath = "C:\FormXChange\Api\FormXChange.Api.exe"

function Install-Service {
    Write-Host "Installing $ServiceName..." -ForegroundColor Green
    
    # Check if service already exists
    $existingService = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    if ($existingService) {
        Write-Host "Service already exists. Uninstalling first..." -ForegroundColor Yellow
        Uninstall-Service
    }
    
    # Create service using sc.exe
    sc.exe create $ServiceName binPath= "`"$ServicePath`"" DisplayName= "`"$DisplayName`"" start= auto
    sc.exe description $ServiceName "`"$Description`""
    
    Write-Host "Service installed successfully!" -ForegroundColor Green
}

function Uninstall-Service {
    Write-Host "Uninstalling $ServiceName..." -ForegroundColor Yellow
    
    $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    if ($service) {
        if ($service.Status -eq "Running") {
            Stop-Service -Name $ServiceName -Force
        }
        sc.exe delete $ServiceName
        Write-Host "Service uninstalled successfully!" -ForegroundColor Green
    } else {
        Write-Host "Service not found." -ForegroundColor Yellow
    }
}

function Start-Service {
    Write-Host "Starting $ServiceName..." -ForegroundColor Green
    Start-Service -Name $ServiceName
    Write-Host "Service started!" -ForegroundColor Green
}

function Stop-Service {
    Write-Host "Stopping $ServiceName..." -ForegroundColor Yellow
    Stop-Service -Name $ServiceName -Force
    Write-Host "Service stopped!" -ForegroundColor Green
}

function Restart-Service {
    Write-Host "Restarting $ServiceName..." -ForegroundColor Yellow
    Restart-Service -Name $ServiceName -Force
    Write-Host "Service restarted!" -ForegroundColor Green
}

# Execute action
switch ($Action) {
    "install" { Install-Service }
    "uninstall" { Uninstall-Service }
    "start" { Start-Service }
    "stop" { Stop-Service }
    "restart" { Restart-Service }
}




