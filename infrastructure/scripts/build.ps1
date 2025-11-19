# Build script for FormXChange Suite
# Usage: .\build.ps1

Write-Host "Building FormXChange Suite..." -ForegroundColor Green

# Build backend projects
Write-Host "`nBuilding Backend Services..." -ForegroundColor Yellow
$backendProjects = @(
    "src\backend\FormXChange.Shared\FormXChange.Shared.csproj",
    "src\backend\FormXChange.Forms\FormXChange.Forms.csproj",
    "src\backend\FormXChange.AI\FormXChange.AI.csproj",
    "src\backend\FormXChange.AI.Forms\FormXChange.AI.Forms.csproj",
    "src\backend\FormXChange.Api\FormXChange.Api.csproj"
)

foreach ($project in $backendProjects) {
    Write-Host "Building $project..." -ForegroundColor Cyan
    dotnet build $project --configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Build failed for $project" -ForegroundColor Red
        exit 1
    }
}

# Build frontend
Write-Host "`nBuilding Frontend..." -ForegroundColor Yellow
Set-Location "src\frontend"
npm install
npm run build
Set-Location "..\..\"

Write-Host "`nBuild completed successfully!" -ForegroundColor Green




