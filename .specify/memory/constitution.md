# PetSalon Constitution

<!--
  Sync Impact Report
  ==================
  Version: 1.1.0 (Added Responsive Design Principle)
  Date: 2025-10-11

  Changes from v1.0.0:
  - Added new principle VI: Responsive & Mobile-First Design
  - Expanded frontend requirements to include mobile accessibility
  - Updated code review checklist to include responsive design verification

  Principles:
  1. Layered Architecture (unchanged)
  2. API-First Development (unchanged)
  3. Type Safety & Validation (unchanged)
  4. Database-First Design (unchanged)
  5. Security by Default (unchanged)
  6. Responsive & Mobile-First Design (NEW)

  Modified sections:
  - Code Review Requirements: Added responsive design checklist item

  Templates requiring updates:
  ✅ plan-template.md - Constitution Check section aligns with principles
  ✅ spec-template.md - Requirements align with functional and security needs
  ✅ tasks-template.md - Task categorization supports layered development

  Follow-up TODOs: None - all placeholders resolved

  Version Bump Rationale:
  MINOR (1.0.0 → 1.1.0) - Added new principle without breaking existing governance
-->

## Core Principles

### I. Layered Architecture

All features MUST follow a strict separation of concerns across four layers:

- **Web Layer** (PetSalon.Web): API controllers, request/response models, authentication middleware
- **Service Layer** (PetSalon.Services): Business logic, interfaces, and implementations
- **Model Layer** (PetSalon.Models): Entity models, DTOs, database context
- **Tools Layer** (PetSalon.Tools): Utility classes, interceptors, helpers

**Rules:**
- Controllers MUST NOT contain business logic (delegate to services)
- Services MUST use dependency injection and define clear interfaces
- Entity models MUST be auto-generated from database (database-first approach)
- Cross-layer violations require documented justification in Complexity Tracking

**Rationale:** Maintains testability, enables independent evolution of layers, and supports team
scalability by allowing parallel development across concerns.

### II. API-First Development

Every feature MUST expose functionality through well-defined RESTful APIs.

**Requirements:**
- All endpoints documented via Swagger/OpenAPI annotations
- DTOs for request/response (never expose entity models directly)
- Consistent HTTP status codes (200 success, 201 created, 400 bad request, 404 not found, 500
  server error)
- API versioning strategy when breaking changes occur

**Frontend Integration:**
- Vue 3 frontend consumes APIs through typed service layer (src/api/)
- TypeScript interfaces mirror backend DTOs
- Axios interceptors handle authentication and error responses

**Rationale:** API-first ensures frontend and backend can evolve independently, enables potential
mobile clients, and provides clear contracts for testing.

### III. Type Safety & Validation

Type safety MUST be enforced at all boundaries (frontend, backend, database).

**Backend (.NET):**
- Model binding with validation attributes ([Required], [StringLength], etc.)
- FluentValidation for complex validation logic
- Exception filters for consistent error responses
- Audit fields (CreateUser, CreateTime, ModifyUser, ModifyTime) via interceptors

**Frontend (Vue/TypeScript):**
- TypeScript strict mode enabled
- Interface definitions for all API responses
- PrimeVue validation on form inputs
- Type-safe API service layer

**Database:**
- EF Core migrations for schema changes
- Proper constraints (NOT NULL, FOREIGN KEY, UNIQUE)
- Stored procedures for complex business logic requiring database access

**Rationale:** Early validation catches errors before they propagate, TypeScript prevents frontend
runtime errors, and database constraints ensure data integrity.

### IV. Database-First Design

Database schema drives entity model generation; never manually edit generated models.

**Workflow:**
1. Create/modify SQL scripts in `/SQL/` folder (tables, views, stored procedures)
2. Execute scripts against database
3. Use EF Core Power Tools to regenerate models from database
4. Update DTOs and services to work with new models

**Audit Trail:**
- All tables include CreateUser, CreateTime, ModifyUser, ModifyTime
- EntitySaveChangesInterceptor automatically populates audit fields
- JWT claims provide current user context

**SystemCode Pattern:**
- Configuration/lookup data stored in SystemCode table
- CodeType groups related codes (Breed, Gender, ServiceType, etc.)
- Frontend fetches system codes via `/api/common/systemcodes/{codeType}`

**Rationale:** Database-first ensures single source of truth for schema, reduces model drift, and
leverages database design tools for complex relationships.

### V. Security by Default

Security MUST be implemented from day one, not retrofitted later.

**Authentication & Authorization:**
- JWT bearer tokens with 30-minute expiration
- Role-based access control (Admin, User roles)
- All controllers protected with [Authorize] attribute (unless explicitly public)
- Claims-based authorization for fine-grained access control

**API Security:**
- CORS configured to allow only known frontend origins
- Input validation on all endpoints
- Parameterized queries (via EF Core) prevent SQL injection
- File upload validation (size, type, secure storage)

**Data Protection:**
- Sensitive configuration in appsettings (connection strings, JWT secrets)
- Audit logging for all data modifications
- No passwords or secrets in code or git history

**Rationale:** Security breaches are costly and damage trust; building security in from the start is
cheaper than fixing vulnerabilities later.

### VI. Responsive & Mobile-First Design

All user interfaces MUST be accessible and functional on both desktop and mobile devices.

**Requirements:**
- Mobile-first CSS approach (design for smallest screen first, then scale up)
- Responsive breakpoints for tablet (768px), desktop (1024px), and large screens (1440px+)
- Touch-friendly UI elements (minimum 44px touch targets)
- Viewport meta tag configured for proper mobile scaling
- PrimeVue components used in responsive mode

**Testing:**
- Test on real devices or browser DevTools device emulation
- Verify all features work on iOS Safari and Android Chrome
- Check portrait and landscape orientations
- Validate touch gestures (tap, swipe, pinch-to-zoom where appropriate)

**Performance:**
- Optimize images for mobile bandwidth (responsive images, lazy loading)
- Minimize bundle size for faster mobile load times
- Use code splitting for route-based chunks
- Consider Progressive Web App (PWA) features for offline capability

**Accessibility:**
- Semantic HTML5 elements
- ARIA labels where needed
- Keyboard navigation support
- Screen reader compatibility

**Rationale:** Pet grooming staff need access to the system while on the floor with clients, making
mobile access essential for day-to-day operations. Responsive design ensures consistent user
experience across all devices without maintaining separate mobile apps.

## Development Workflow

### Feature Development Process

1. **Specification**: Create feature spec in `/specs/{feature-name}/spec.md` using spec-template
2. **Planning**: Run `/speckit.plan` to generate implementation plan with research and design
3. **Tasks**: Run `/speckit.tasks` to generate dependency-ordered task list
4. **Implementation**: Execute tasks sequentially or in parallel (marked with [P])
5. **Testing**: Manual testing via Swagger UI and frontend, automated tests if specified
6. **Review**: Code review ensures compliance with all constitution principles
7. **Deployment**: Backend to IIS/Docker, frontend to static hosting, database migrations

### Code Review Requirements

All pull requests MUST verify:
- [ ] Layered architecture maintained (no cross-layer violations)
- [ ] API endpoints documented in Swagger
- [ ] DTOs used for API boundaries (not entity models)
- [ ] TypeScript types defined for API responses
- [ ] Validation attributes or FluentValidation applied
- [ ] Audit fields populated via interceptor (not manually)
- [ ] [Authorize] attribute on protected endpoints
- [ ] No hardcoded secrets or connection strings
- [ ] Database changes include migration scripts
- [ ] UI tested on mobile and desktop viewports (if frontend changes)

### Testing Standards

**Manual Testing (Minimum):**
- Swagger UI for API endpoint verification
- Frontend user journey testing
- Cross-browser compatibility (Chrome, Firefox, Safari)
- Mobile responsiveness (test on actual devices or browser DevTools emulation)

**Automated Testing (If Specified):**
- Unit tests for service layer logic
- Integration tests for API endpoints
- Contract tests for API stability
- Frontend component tests with Vitest

### Complexity Justification

Any violation of constitution principles MUST be documented in implementation plan's Complexity
Tracking section with:
- What principle is violated
- Why the violation is necessary
- What simpler alternative was rejected and why

Example violations requiring justification:
- Adding a 5th layer beyond Web/Service/Model/Tools
- Exposing entity models directly in API responses
- Bypassing validation for "performance reasons"
- Storing configuration in database instead of appsettings

## Technology Constraints

### Approved Technology Stack

**Backend:**
- .NET 8.0 or higher
- Entity Framework Core 8.0 or higher
- SQL Server 2019 or higher
- JWT Bearer authentication
- Swagger/OpenAPI for documentation

**Frontend:**
- Vue 3.4+ with Composition API
- TypeScript 5.4+
- PrimeVue 4.3+ for UI components
- Pinia 2.2+ for state management
- Vite 5.3+ for build tooling

**Development Tools:**
- Git for version control
- Docker for local SQL Server (optional)
- EF Core Power Tools for model generation
- ESLint + Prettier for frontend code quality

### Technology Decisions

**Why .NET?**
- Enterprise-grade performance and scalability
- Strong typing and compile-time safety
- Mature ecosystem for business applications
- Excellent tooling (Visual Studio, Rider)

**Why Vue 3?**
- Gentle learning curve for team
- Composition API provides excellent TypeScript support
- PrimeVue offers comprehensive UI components
- Smaller bundle size than Angular

**Why SQL Server?**
- Robust transaction support for financial data (payments)
- Excellent tooling and management
- Strong backup/recovery capabilities
- Compatible with Azure for cloud deployment

**Why Database-First?**
- SQL Server Management Studio provides excellent design tools
- Database schema is source of truth for enterprise systems
- Easier to apply database performance optimizations
- Supports stored procedures for complex queries

## Governance

### Amendment Process

Constitution changes require:
1. **Proposal**: Document why change is needed, what principles are affected
2. **Impact Analysis**: Review all dependent templates and existing features
3. **Version Bump**: MAJOR for breaking changes, MINOR for additions, PATCH for clarifications
4. **Migration Plan**: How will existing code comply with new principles?
5. **Approval**: Team review and consensus
6. **Propagation**: Update all templates, specs, and documentation

### Version Numbering

**MAJOR.MINOR.PATCH** format:

- **MAJOR**: Backward incompatible governance changes (e.g., removing a layer, changing DB strategy)
- **MINOR**: New principles or materially expanded guidance (e.g., adding observability principle)
- **PATCH**: Clarifications, wording improvements, typo fixes

### Compliance Review

**Per Feature:**
- Constitution Check in implementation plan (before coding starts)
- Code review verifies compliance (before merge)

**Quarterly:**
- Review all features for constitution violations
- Identify patterns suggesting constitution needs amendment
- Update guidance based on lessons learned

### Runtime Guidance

Day-to-day development guidance is maintained in `/CLAUDE.md` (agent-agnostic) and covers:
- Common commands (dotnet, npm)
- API endpoint catalog
- Development patterns and examples
- Troubleshooting tips

Constitution governs *what* and *why*; runtime guidance covers *how*.

**Version**: 1.1.0 | **Ratified**: 2025-10-11 | **Last Amended**: 2025-10-11
