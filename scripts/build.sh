#!/bin/bash
# Build script for FormXChange Suite

set -e

echo "Building FormXChange Suite..."

# Install dependencies
echo "Installing dependencies..."
npm ci

# Run linting
echo "Running linters..."
npm run lint

# Run tests
echo "Running tests..."
npm test

# Build all applications
echo "Building all applications..."
npm run build

echo "Build completed successfully!"

