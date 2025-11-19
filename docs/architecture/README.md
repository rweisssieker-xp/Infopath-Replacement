# FormXChange Suite Architecture

## Overview

FormXChange Suite is a comprehensive, AI-first platform for creating and managing dynamic business forms and workflows in enterprise environments.

## Architecture Principles

1. **AI-First Design**: Primary creation method is through conversational AI interfaces
2. **Microservices**: Backend services are separated by domain
3. **Schema-Based**: Forms use JSON Schema for type-safe definitions
4. **Git-like Versioning**: Full version control with branching support
5. **Vendor-Neutral**: Works with any enterprise system

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      Frontend (React)                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │ AI Form      │  │ AI Workflow  │  │ Form Runtime  │     │
│  │ Designer     │  │ Designer     │  │              │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                  GraphQL Gateway (Hot Chocolate)             │
└─────────────────────────────────────────────────────────────┘
                            │
        ┌───────────────────┼───────────────────┐
        ▼                   ▼                   ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│ Forms        │  │ AI.Forms     │  │ Workflow     │
│ Service      │  │ Service      │  │ Service      │
└──────────────┘  └──────────────┘  └──────────────┘
        │                   │                   │
        └───────────────────┼───────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    PostgreSQL Database                      │
└─────────────────────────────────────────────────────────────┘
```

## Technology Stack

### Backend
- .NET 8 (C#)
- Entity Framework Core
- Hot Chocolate (GraphQL)
- MediatR (CQRS)
- Azure OpenAI / OpenAI API

### Frontend
- React 18+
- TypeScript 5+
- Material-UI
- React Query
- Vite

### Database
- PostgreSQL 15+
- Redis (caching)

## Key Components

### AI Form Designer (PRIMARY)
- Conversational interface for form creation
- Natural language to JSON Schema conversion
- Real-time preview
- Multi-turn refinement

### AI Workflow Designer (PRIMARY)
- Conversational workflow creation
- Natural language to BPMN conversion
- Form analysis → Workflow generation

### Form Runtime
- Dynamic form renderer
- Offline PWA support
- Real-time collaboration
- Conversational form filling

## Deployment

### Windows
- IIS or Windows Service
- PowerShell deployment scripts

### Linux
- systemd services
- Nginx reverse proxy

## Security

- OAuth2/OIDC authentication
- RBAC + ABAC authorization
- Field-level audit trails
- GDPR compliance
- Data encryption at rest and in transit




