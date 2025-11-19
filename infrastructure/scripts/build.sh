#!/bin/bash
# Build script for FormXChange Suite
# Usage: ./build.sh

echo "Building FormXChange Suite..."

# Build backend projects
echo ""
echo "Building Backend Services..."
BACKEND_PROJECTS=(
    "src/backend/FormXChange.Shared/FormXChange.Shared.csproj"
    "src/backend/FormXChange.Forms/FormXChange.Forms.csproj"
    "src/backend/FormXChange.AI/FormXChange.AI.csproj"
    "src/backend/FormXChange.AI.Forms/FormXChange.AI.Forms.csproj"
    "src/backend/FormXChange.Api/FormXChange.Api.csproj"
)

for project in "${BACKEND_PROJECTS[@]}"; do
    echo "Building $project..."
    dotnet build "$project" --configuration Release
    if [ $? -ne 0 ]; then
        echo "Build failed for $project"
        exit 1
    fi
done

# Build frontend
echo ""
echo "Building Frontend..."
cd src/frontend
npm install
npm run build
cd ../..

echo ""
echo "Build completed successfully!"




