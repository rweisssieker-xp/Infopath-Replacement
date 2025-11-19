# Unified Project Structure

```
formxchange-suite/
├── .github/
│   └── workflows/
│       ├── ci.yaml
│       └── deploy.yaml
├── apps/
│   ├── web/                          # React Frontend
│   │   ├── src/
│   │   │   ├── components/
│   │   │   ├── pages/
│   │   │   ├── hooks/
│   │   │   ├── services/
│   │   │   ├── stores/
│   │   │   └── utils/
│   │   ├── public/
│   │   ├── tests/
│   │   ├── package.json
│   │   └── vite.config.ts
│   ├── form-builder-service/         # Form Builder Microservice
│   │   ├── src/
│   │   │   ├── Controllers/
│   │   │   ├── Services/
│   │   │   ├── Repositories/
│   │   │   ├── Models/
│   │   │   ├── GraphQL/
│   │   │   └── Program.cs
│   │   ├── tests/
│   │   └── FormBuilderService.csproj
│   ├── form-runtime-service/         # Form Runtime Microservice
│   ├── workflow-service/              # Workflow Microservice
│   ├── integration-gateway/          # Integration Gateway
│   ├── auth-service/                 # Auth Microservice
│   ├── storage-service/              # Storage Microservice
│   ├── ai-service/                  # AI Microservice
│   └── admin-service/               # Admin Microservice
├── packages/
│   ├── shared/                       # Shared TypeScript types
│   │   ├── src/
│   │   │   ├── types/
│   │   │   │   ├── form.ts
│   │   │   │   ├── submission.ts
│   │   │   │   └── user.ts
│   │   │   ├── constants/
│   │   │   └── utils/
│   │   └── package.json
│   ├── ui/                          # Shared UI components
│   │   ├── src/
│   │   │   └── components/
│   │   └── package.json
│   └── config/                      # Shared configs
│       ├── eslint/
│       ├── typescript/
│       └── jest/
├── infrastructure/
│   ├── kubernetes/
│   │   ├── namespaces/
│   │   ├── deployments/
│   │   ├── services/
│   │   └── ingress/
│   ├── helm/
│   │   └── formxchange-suite/
│   └── terraform/                   # Optional IaC
├── scripts/
│   ├── build.sh
│   └── deploy.sh
├── docs/
│   ├── prd.md
│   ├── architecture.md
│   └── front-end-spec.md
├── .env.example
├── package.json                     # Root package.json (monorepo)
├── nx.json                          # Nx configuration
└── README.md
```

