# FormXChange Suite

Eine modulare Plattform zur Erstellung komplexer, dynamischer Geschäftsformulare mit Enterprise-Integrationen, Workflow-Automatisierung und AI-gestützten Funktionen.

## Projektübersicht

FormXChange Suite ist eine moderne Alternative zu Microsoft InfoPath, Lotus Forms und Adobe LiveCycle. Die Plattform ermöglicht die Low-Code-Erstellung komplexer Formulare, nahtlose Enterprise-Integrationen und umfassende Compliance-Funktionen.

## Dokumentation

- **[Product Requirements Document (PRD)](docs/prd.md)** - Vollständige Produktspezifikation mit Epics und Stories
- **[Architecture Document](docs/architecture.md)** - Systemarchitektur-Dokumentation
- **[Frontend Specification](docs/front-end-spec.md)** - UI/UX Spezifikation
- **[Stories](docs/stories/)** - User Stories für die Entwicklung

## Technologie-Stack

- **Frontend**: React 18+ mit TypeScript, Material-UI/Ant Design
- **Backend**: .NET 8 (C#), ASP.NET Core
- **API**: GraphQL (Hot Chocolate) + REST
- **Workflow Engine**: Camunda 8 (Zeebe) oder Temporal
- **Database**: PostgreSQL 15+
- **Cache**: Redis 7.2+
- **Storage**: S3-kompatibler Storage (MinIO/AWS S3)
- **Deployment**: Kubernetes (Helm Charts)
- **Authentication**: OAuth2/OIDC (Azure AD / Entra ID)
- **CI/CD**: GitHub Actions / GitLab CI
- **Monitoring**: Prometheus + Grafana
- **Logging**: ELK Stack

## Projektstruktur

```
formxchange-suite/
├── apps/                          # Alle Anwendungen (Frontend + Microservices)
│   ├── web/                       # React Frontend
│   ├── form-builder-service/      # Form Builder Microservice
│   ├── form-runtime-service/      # Form Runtime Microservice
│   ├── workflow-service/          # Workflow Microservice
│   ├── integration-gateway/       # Integration Gateway
│   ├── auth-service/              # Auth Microservice
│   ├── storage-service/           # Storage Microservice
│   ├── ai-service/               # AI Microservice
│   └── admin-service/            # Admin Microservice
├── packages/                      # Shared Packages
│   ├── shared/                   # Shared TypeScript types
│   ├── ui/                       # Shared UI components
│   └── config/                   # Shared configs
├── infrastructure/                # Infrastructure as Code
│   ├── kubernetes/               # Kubernetes manifests
│   ├── helm/                     # Helm charts
│   └── terraform/                # Terraform (optional)
├── scripts/                      # Build and deployment scripts
├── docs/                         # Dokumentation
│   ├── prd.md
│   ├── architecture.md
│   ├── front-end-spec.md
│   ├── stories/                  # User Stories
│   └── qa/                       # Quality Assurance
└── .bmad-core/                   # BMad Method Konfiguration
```

## Lokale Entwicklung

### Voraussetzungen

- Node.js 18+ und npm/yarn/pnpm
- .NET 8 SDK
- PostgreSQL 15+ (oder lokale Installation)
- Redis 7.2+ (oder lokale Installation)
- MinIO oder AWS S3 (für Object Storage)

### Setup

1. **Repository klonen:**
   ```bash
   git clone <repository-url>
   cd formxchange-suite
   ```

2. **Dependencies installieren:**
   ```bash
   npm install
   ```

3. **Umgebungsvariablen konfigurieren:**
   ```bash
   cp .env.example .env.development
   # Bearbeite .env.development und fülle die Werte aus
   ```

4. **Infrastruktur-Services starten:**
   - PostgreSQL: Lokal installiert oder über Docker (siehe docker-compose.yml)
   - Redis: Lokal installiert oder über Docker
   - MinIO: Lokal installiert oder über Docker

5. **Anwendung starten:**
   ```bash
   # Alle Services starten
   npm run dev
   
   # Nur Frontend
   npm run dev:web
   
   # Nur Backend Services
   npm run dev:api
   ```

### Entwicklungskommandos

```bash
# Tests ausführen
npm test                    # Alle Tests
npm run test:web           # Frontend Tests
npm run test:api          # Backend Tests

# Build
npm run build              # Alle Anwendungen bauen

# Linting
npm run lint               # Code-Qualität prüfen

# Formatierung
npm run format             # Code formatieren
```

## Deployment

### Kubernetes Deployment

Das Projekt verwendet Kubernetes für die Produktions-Deployment:

```bash
# Namespace erstellen
kubectl apply -f infrastructure/kubernetes/namespaces/formxchange.yaml

# Mit Helm deployen
helm install formxchange-suite ./infrastructure/helm/formxchange-suite \
  --namespace formxchange \
  --values ./infrastructure/helm/formxchange-suite/values.yaml
```

### CI/CD Pipeline

Die CI/CD Pipeline ist in `.github/workflows/ci.yaml` konfiguriert und führt automatisch aus:
- Tests (Frontend und Backend)
- Linting
- Build
- Security Scans

## Entwicklung

Dieses Projekt verwendet die BMad Method für strukturierte Softwareentwicklung. Siehe [AGENTS.md](AGENTS.md) für verfügbare Agents und Workflows.

### Story-basierte Entwicklung

- Stories befinden sich in `docs/stories/`
- Jede Story folgt dem Format: `{epic}.{story}.{title}.story.md`
- Stories werden sequenziell implementiert und durch QA-Review geprüft

## Lizenz

[Zu definieren]
