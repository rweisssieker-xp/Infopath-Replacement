# FormXChange Suite

Eine modulare Plattform zur Erstellung komplexer, dynamischer Geschäftsformulare mit Enterprise-Integrationen, Workflow-Automatisierung und AI-gestützten Funktionen.

## Projektübersicht

FormXChange Suite ist eine moderne Alternative zu Microsoft InfoPath, Lotus Forms und Adobe LiveCycle. Die Plattform ermöglicht die Low-Code-Erstellung komplexer Formulare, nahtlose Enterprise-Integrationen und umfassende Compliance-Funktionen.

## Dokumentation

- **[Product Requirements Document (PRD)](docs/prd.md)** - Vollständige Produktspezifikation mit Epics und Stories
- **Architecture** - Systemarchitektur-Dokumentation (wird erstellt)
- **Stories** - User Stories für die Entwicklung (wird erstellt)

## Technologie-Stack

- **Frontend**: React 18+ mit TypeScript
- **Backend**: .NET 8 (C#)
- **Workflow Engine**: Camunda 8 (Zeebe) oder Temporal
- **Database**: PostgreSQL 15+
- **Storage**: S3-kompatibler Storage (MinIO/AWS S3)
- **Deployment**: Docker/Kubernetes
- **Authentication**: OAuth2/OIDC (Azure AD / Entra ID)

## Projektstruktur

```
.
├── docs/
│   ├── prd.md                    # Product Requirements Document
│   ├── architecture/              # Architektur-Dokumentation
│   ├── stories/                   # User Stories
│   └── qa/                        # Quality Assurance Dokumente
│       ├── gates/                 # Quality Gates
│       └── assessments/           # QA Assessments
└── .bmad-core/                    # BMad Method Konfiguration
```

## Entwicklung

Dieses Projekt verwendet die BMad Method für strukturierte Softwareentwicklung. Siehe [AGENTS.md](AGENTS.md) für verfügbare Agents und Workflows.

## Lizenz

[Zu definieren]

