# Implementation Plan: PetSalon Management System

**Branch**: `001-petsalon-system` | **Date**: 2025-10-11 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-petsalon-system/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

PetSalon is a comprehensive management system for pet grooming businesses, enabling staff to manage pet records, contact persons, appointments, subscription packages, payments, and generate business reports. The system follows a full-stack architecture with .NET 8 backend (RESTful API), Vue 3 TypeScript frontend (responsive design), and SQL Server database. It supports both desktop and mobile devices with touch-friendly interfaces for on-floor operations.

## Technical Context

**Language/Version**:
- Backend: .NET 8.0 (C#)
- Frontend: TypeScript 5.4+

**Primary Dependencies**:
- Backend: ASP.NET Core 8.0, Entity Framework Core 8.0, JWT Bearer authentication, Swagger/OpenAPI
- Frontend: Vue 3.4+ (Composition API), PrimeVue 4.3+, Pinia 2.2+, Axios, Vite 5.3+

**Storage**: SQL Server 2019+ with Entity Framework Core (database-first approach)

**Testing**:
- Backend: xUnit or NUnit (if specified)
- Frontend: Vitest for component tests (if specified)
- Manual: Swagger UI for API testing, browser DevTools for responsive testing

**Target Platform**:
- Backend: Windows/Linux server (IIS, Docker, or Kestrel)
- Frontend: Modern web browsers (Chrome, Firefox, Safari, Edge) on desktop (1024px+), tablet (768px), mobile (320px+)

**Project Type**: Web application (full-stack with backend API + frontend SPA)

**Performance Goals**:
- API response time: <2 seconds for CRUD operations
- Mobile page load: <3 seconds on 3G network
- Concurrent users: 50+ without degradation
- Calendar rendering: <1 second for monthly view with 100+ appointments

**Constraints**:
- Mobile-first responsive design with minimum 44px touch targets
- HTTPS only in production
- CORS restricted to known frontend origins
- Database-first: never manually edit generated entity models
- JWT token expiration: 30 minutes
- File upload limit: 10MB for pet photos

**Scale/Scope**:
- Expected users: 10-50 staff members per salon
- Data volume: 1,000-10,000 pets, 5,000-50,000 appointments per year
- Frontend screens: ~15-20 pages (Pet list/detail, Contact list/detail, Calendar, Appointments, Subscriptions, Payments, Reports, System admin)
- API endpoints: ~40-50 endpoints across 6 main controllers

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Principle I: Layered Architecture ✅ PASS

**Gate**: All features MUST follow strict separation across Web/Service/Model/Tools layers.

**Compliance**:
- ✅ Backend organized into PetSalon.Web (controllers), PetSalon.Services (business logic), PetSalon.Models (entities, DTOs), PetSalon.Tools (utilities)
- ✅ Controllers delegate to services via dependency injection
- ✅ Entity models auto-generated from database (database-first via EF Core Power Tools)
- ✅ DTOs used for API request/response models
- ✅ No cross-layer violations planned

**Status**: Compliant - existing architecture already follows this principle

### Principle II: API-First Development ✅ PASS

**Gate**: Every feature MUST expose functionality through well-defined RESTful APIs.

**Compliance**:
- ✅ All endpoints documented via Swagger/OpenAPI
- ✅ DTOs for request/response (never expose entity models directly)
- ✅ Consistent HTTP status codes (200, 201, 400, 404, 500)
- ✅ Vue 3 frontend consumes APIs through typed service layer (src/api/)
- ✅ TypeScript interfaces mirror backend DTOs
- ✅ Axios interceptors handle authentication and errors

**Status**: Compliant - plan includes OpenAPI contracts and typed API layer

### Principle III: Type Safety & Validation ✅ PASS

**Gate**: Type safety MUST be enforced at all boundaries (frontend, backend, database).

**Compliance**:
- ✅ Backend: Model binding with validation attributes, FluentValidation for complex rules
- ✅ Frontend: TypeScript strict mode, interface definitions for all API responses
- ✅ Database: EF Core constraints (NOT NULL, FOREIGN KEY, UNIQUE)
- ✅ Audit fields via EntitySaveChangesInterceptor
- ✅ PrimeVue validation on form inputs

**Status**: Compliant - comprehensive validation strategy at all layers

### Principle IV: Database-First Design ✅ PASS

**Gate**: Database schema drives entity model generation; never manually edit generated models.

**Compliance**:
- ✅ SQL scripts in `/SQL/` folder define schema
- ✅ EF Core Power Tools regenerate models from database
- ✅ Audit trail: CreateUser, CreateTime, ModifyUser, ModifyTime on all tables
- ✅ EntitySaveChangesInterceptor automatically populates audit fields
- ✅ SystemCode pattern for configurable lookup data

**Status**: Compliant - existing database structure follows this workflow

### Principle V: Security by Default ✅ PASS

**Gate**: Security MUST be implemented from day one, not retrofitted later.

**Compliance**:
- ✅ JWT bearer tokens with 30-minute expiration
- ✅ Role-based access control (Admin, User roles)
- ✅ All controllers protected with [Authorize] attribute
- ✅ CORS configured for known frontend origins only
- ✅ Input validation on all endpoints
- ✅ Parameterized queries via EF Core prevent SQL injection
- ✅ File upload validation (size, type, secure storage)
- ✅ Audit logging for all data modifications

**Status**: Compliant - comprehensive security from the start

### Principle VI: Responsive & Mobile-First Design ✅ PASS

**Gate**: All user interfaces MUST be accessible and functional on both desktop and mobile devices.

**Compliance**:
- ✅ Mobile-first CSS approach (design for smallest screen first, then scale up)
- ✅ Responsive breakpoints: tablet (768px), desktop (1024px), large (1440px+)
- ✅ Touch-friendly UI elements (minimum 44px touch targets)
- ✅ Viewport meta tag configured for proper mobile scaling
- ✅ PrimeVue components used in responsive mode
- ✅ Testing on real devices and DevTools emulation
- ✅ Image optimization for mobile bandwidth
- ✅ Code splitting for faster mobile load times

**Status**: Compliant - mobile access is core requirement, responsive design mandated

### Overall Constitution Compliance: ✅ ALL GATES PASS

All six core principles are satisfied. No violations requiring justification. Proceed to Phase 0 research.

## Project Structure

### Documentation (this feature)

```
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```
PetSalon/
├── PetSalon.Backend/                    # .NET 8 Backend Solution
│   ├── PetSalon.sln                     # Solution file
│   ├── PetSalon.Web/                    # Web API Layer
│   │   ├── Controllers/                 # API controllers (Pet, ContactPerson, Reservation, etc.)
│   │   ├── Models/                      # Request/Response DTOs
│   │   ├── Program.cs                   # Application entry point, DI configuration
│   │   ├── appsettings.json             # Configuration (connection strings, JWT settings)
│   │   └── wwwroot/uploads/pets/        # Pet photo storage
│   ├── PetSalon.Services/               # Business Logic Layer
│   │   ├── Interfaces/                  # Service interfaces (IPetService, IReservationService, etc.)
│   │   ├── Implementations/             # Service implementations
│   │   └── DTOs/                        # Data transfer objects
│   ├── PetSalon.Models/                 # Data Model Layer
│   │   ├── EntityModels/                # EF Core entities (auto-generated from database)
│   │   ├── DTOs/                        # Additional DTOs
│   │   └── PetSalonContext.cs           # EF Core DbContext (auto-generated)
│   └── PetSalon.Tools/                  # Utilities Layer
│       ├── Interceptors/                # EntitySaveChangesInterceptor for audit fields
│       ├── Helpers/                     # JwtHelpers, etc.
│       └── Extensions/                  # Extension methods
│
├── PetSalon.Frontend/                   # Vue 3 + TypeScript Frontend
│   ├── src/
│   │   ├── views/                       # Page components (Pet, Contact, Calendar, etc.)
│   │   ├── components/                  # Reusable UI components
│   │   ├── api/                         # API service layer (petApi, reservationApi, etc.)
│   │   ├── types/                       # TypeScript type definitions
│   │   ├── stores/                      # Pinia state management stores
│   │   ├── router/                      # Vue Router configuration
│   │   ├── utils/                       # Utility functions
│   │   ├── assets/                      # Static assets (images, styles)
│   │   └── main.ts                      # Application entry point
│   ├── public/                          # Public static files
│   ├── index.html                       # HTML entry file
│   ├── vite.config.ts                   # Vite build configuration
│   ├── tsconfig.json                    # TypeScript configuration
│   └── package.json                     # NPM dependencies
│
├── SQL/                                 # Database Scripts
│   ├── 10-Table/                        # Table creation scripts
│   ├── 70-InintialData/                 # Initial data (SystemCode, etc.)
│   └── 80-Migration/                    # Database migration scripts
│
├── specs/                               # Feature specifications
│   └── 001-petsalon-system/            # This feature
│       ├── spec.md                      # Feature specification
│       ├── plan.md                      # This implementation plan
│       ├── research.md                  # Phase 0 research (to be generated)
│       ├── data-model.md                # Phase 1 data model (to be generated)
│       ├── quickstart.md                # Phase 1 quickstart guide (to be generated)
│       └── contracts/                   # Phase 1 API contracts (to be generated)
│
├── .specify/                            # SpecKit configuration
│   ├── memory/                          # Constitution and memory
│   ├── templates/                       # Document templates
│   └── scripts/                         # Automation scripts
│
├── CLAUDE.md                            # Development guidance for AI agents
├── README.md                            # Project documentation
└── docker-compose.yml                   # Docker configuration for SQL Server (optional)
```

**Structure Decision**: Web application with separate backend and frontend directories. Backend follows 4-layer architecture (Web/Services/Models/Tools) as mandated by Constitution Principle I. Frontend follows standard Vue 3 SPA structure with typed API layer. Database-first approach with SQL scripts in `/SQL/` folder.

## Complexity Tracking

*Fill ONLY if Constitution Check has violations that must be justified*

**No violations detected.** All constitution principles are satisfied. The project follows established patterns:
- 4-layer backend architecture (Web/Services/Models/Tools)
- API-first with OpenAPI documentation
- Type-safe with validation at all boundaries
- Database-first with EF Core Power Tools
- Secure by default with JWT and audit logging
- Responsive mobile-first design

No complexity justification required.
