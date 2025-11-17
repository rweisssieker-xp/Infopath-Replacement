# FormXChange Suite

Vollwertiger, moderner und marktfähiger Ersatz für Microsoft InfoPath zur Erstellung, Verwaltung, Automatisierung und Integration dynamischer Geschäftsformulare in Enterprise-Umgebungen.

## Architektur

- **Frontend**: React 18+ mit TypeScript, AI-First Designer
- **Backend**: .NET 8 Microservices
- **Database**: PostgreSQL 15+
- **AI/ML**: OpenAI GPT-4 / Azure OpenAI, ML.NET
- **Workflow**: Camunda 8 (Zeebe)
- **Deployment**: Direct deployment (Windows/Linux), no Docker

## Projektstruktur

```
FormXChange-Suite/
├── src/
│   ├── frontend/          # React + TypeScript SPA
│   ├── backend/           # .NET 8 Microservices
│   ├── workflow/          # Workflow engine integration
│   ├── integrations/      # Integration connectors
│   └── ai-ml/             # AI/ML models and services
├── infrastructure/        # Deployment scripts
├── database/              # PostgreSQL migrations
├── tests/                 # Test suites
└── docs/                  # Documentation
```

## Features

### AI-First Design (PRIMARY USP)
- Conversational Form Creation: Designer erstellen Formulare durch natürliche Sprache
- AI-Generated Workflows: Workflows werden automatisch aus Formularanalyse oder natürlicher Sprache erstellt
- Zero-Code AI Design: Keine technischen Kenntnisse erforderlich

### Weitere USPs
- Git-like Versioning & Branching
- Schema-based Architecture (JSON Schema)
- Vendor-Neutral & Open Architecture
- Enterprise-Grade Features (RBAC/ABAC, Multi-Tenancy, Compliance)
- Advanced Collaboration (Real-time co-authoring)
- Marketplace Ecosystem

## Getting Started

### Voraussetzungen
- .NET 8 SDK
- Node.js 18+
- PostgreSQL 15+
- Redis (optional)

### Lokale Entwicklung
```bash
# Backend
cd src/backend/FormXChange.Api
dotnet run

# Frontend
cd src/frontend/apps/web
npm install
npm run dev
```

## Lizenz

Proprietär

