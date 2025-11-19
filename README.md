# FormXChange Suite

A modular platform for creating complex, dynamic business forms with enterprise integrations, workflow automation, and AI-powered features.

## Project Overview

FormXChange Suite is a modern alternative to Microsoft InfoPath, Lotus Forms, and Adobe LiveCycle. The platform enables low-code creation of complex forms, seamless enterprise integrations, and comprehensive compliance features.

## Documentation

- **[Product Requirements Document (PRD)](docs/prd.md)** - Complete product specification with epics and stories
- **[Architecture Document](docs/architecture.md)** - System architecture documentation
- **[Frontend Specification](docs/front-end-spec.md)** - UI/UX specification
- **[Stories](docs/stories/)** - User stories for development

## Technology Stack

- **Frontend**: React 18+ with TypeScript, Material-UI/Ant Design
- **Backend**: .NET 8 (C#), ASP.NET Core
- **API**: GraphQL (Hot Chocolate) + REST
- **Workflow Engine**: Camunda 8 (Zeebe) or Temporal
- **Database**: PostgreSQL 15+
- **Cache**: Redis 7.2+
- **Storage**: S3-compatible storage (MinIO/AWS S3)
- **Deployment**: Kubernetes (Helm Charts)
- **Authentication**: OAuth2/OIDC (Azure AD / Entra ID)
- **CI/CD**: GitHub Actions / GitLab CI
- **Monitoring**: Prometheus + Grafana
- **Logging**: ELK Stack

## Project Structure

```
formxchange-suite/
├── apps/                          # All applications (Frontend + Microservices)
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
├── docs/                         # Documentation
│   ├── prd.md
│   ├── architecture.md
│   ├── front-end-spec.md
│   ├── stories/                  # User Stories
│   └── qa/                       # Quality Assurance
└── .bmad-core/                   # BMad Method Configuration
```

## Local Development

### Prerequisites

- Node.js 18+ and npm/yarn/pnpm
- .NET 8 SDK
- PostgreSQL 15+ (or local installation)
- Redis 7.2+ (or local installation)
- MinIO or AWS S3 (for object storage)

### Setup

1. **Clone repository:**
   ```bash
   git clone <repository-url>
   cd formxchange-suite
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Configure environment variables:**
   ```bash
   cp .env.example .env.development
   # Edit .env.development and fill in the values
   ```

4. **Start infrastructure services:**
   - PostgreSQL: Locally installed or via Docker (see docker-compose.yml)
   - Redis: Locally installed or via Docker
   - MinIO: Locally installed or via Docker

5. **Start application:**
   ```bash
   # Start all services
   npm run dev
   
   # Frontend only
   npm run dev:web
   
   # Backend services only
   npm run dev:api
   ```

### Development Commands

```bash
# Run tests
npm test                    # All tests
npm run test:web           # Frontend tests
npm run test:api          # Backend tests

# Build
npm run build              # Build all applications

# Linting
npm run lint               # Check code quality

# Formatting
npm run format             # Format code
```

## Deployment

### Kubernetes Deployment

The project uses Kubernetes for production deployment:

```bash
# Create namespace
kubectl apply -f infrastructure/kubernetes/namespaces/formxchange.yaml

# Deploy with Helm
helm install formxchange-suite ./infrastructure/helm/formxchange-suite \
  --namespace formxchange \
  --values ./infrastructure/helm/formxchange-suite/values.yaml
```

### CI/CD Pipeline

The CI/CD pipeline is configured in `.github/workflows/ci.yaml` and automatically executes:
- Tests (Frontend and Backend)
- Linting
- Build
- Security Scans

## Development

This project uses the BMad Method for structured software development. See [AGENTS.md](AGENTS.md) for available agents and workflows.

### Story-based Development

- Stories are located in `docs/stories/`
- Each story follows the format: `{epic}.{story}.{title}.story.md`
- Stories are implemented sequentially and reviewed by QA

## License

[To be defined]
