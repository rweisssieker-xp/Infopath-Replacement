# Getting Started with FormXChange Suite

## Prerequisites

- .NET 8 SDK
- Node.js 18+ and npm
- PostgreSQL 15+
- Redis (optional, for caching)

## Setup

### 1. Database Setup

Create a PostgreSQL database:

```sql
CREATE DATABASE formxchange;
```

Update connection strings in `appsettings.json` files.

### 2. Backend Setup

```bash
# Restore NuGet packages
dotnet restore

# Run database migrations (when EF Core migrations are set up)
dotnet ef database update --project src/backend/FormXChange.Shared

# Run Forms service
cd src/backend/FormXChange.Forms
dotnet run

# Run AI Forms service
cd src/backend/FormXChange.AI.Forms
dotnet run

# Run API Gateway
cd src/backend/FormXChange.Api
dotnet run
```

### 3. Frontend Setup

```bash
cd src/frontend
npm install
npm run dev
```

### 4. Configure AI Services

Update `appsettings.json` in AI services with your OpenAI API key:

```json
{
  "AI": {
    "OpenAI": {
      "ApiKey": "your-api-key-here",
      "Endpoint": "",
      "ModelName": "gpt-4"
    }
  }
}
```

## Usage

### Creating Forms with AI

1. Navigate to `/forms/create` in the web application
2. Start a conversation with the AI Form Designer
3. Describe the form you want to create, e.g., "Create a form for employee onboarding with fields for name, email, department, and start date"
4. The AI will generate a complete form schema
5. Review and refine as needed

### Creating Workflows with AI

1. Navigate to `/workflows/create`
2. Describe your workflow requirements
3. The AI will generate a workflow definition

## Development

### Running Tests

```bash
dotnet test
```

### Building

```bash
# Windows
.\infrastructure\scripts\build.ps1

# Linux/Mac
./infrastructure/scripts/build.sh
```

## Architecture

See [Architecture Documentation](architecture/README.md) for detailed information.




