# Research: PetSalon Management System

**Feature**: 001-petsalon-system
**Date**: 2025-10-11
**Status**: Phase 0 Research Complete

## Research Summary

This document consolidates research findings for implementing the PetSalon management system. Since all technical details were clearly specified in the Technical Context (no "NEEDS CLARIFICATION" items), this research focuses on best practices, patterns, and implementation strategies for the chosen technology stack.

## Technology Stack Research

### 1. .NET 8 + Entity Framework Core Best Practices

**Decision**: Use .NET 8 with Entity Framework Core 8.0 for backend

**Rationale**:
- Latest LTS version with improved performance (up to 30% faster than .NET 6)
- Native AOT support for faster startup times
- Enhanced minimal APIs with better routing
- Built-in support for rate limiting and improved JWT authentication
- Better JSON serialization performance with System.Text.Json improvements

**Best Practices Adopted**:

1. **Layered Architecture**:
   - Clear separation: Web (controllers) → Services (business logic) → Models (data) → Tools (utilities)
   - Use dependency injection for all service dependencies
   - Controllers should be thin, delegating to services

2. **Entity Framework Core Patterns**:
   - Use `AsNoTracking()` for read-only queries to improve performance
   - Implement pagination with `Skip()` and `Take()` for large result sets
   - Use `Include()` for eager loading to prevent N+1 query problems
   - Leverage compiled queries for frequently executed operations
   - Use `SaveChangesInterceptor` for cross-cutting concerns (audit fields)

3. **Performance Optimizations**:
   - Enable response compression middleware
   - Use output caching for GET endpoints that don't change frequently
   - Configure connection pooling for SQL Server (default: min 0, max 100)
   - Use async/await throughout to avoid thread blocking

4. **Database-First Workflow**:
   - SQL scripts version-controlled in `/SQL/` folder
   - EF Core Power Tools to regenerate models from database
   - Never manually edit generated entity classes
   - Use partial classes for custom logic

**Alternatives Considered**:
- Code-First EF Core: Rejected because database-first is mandated by constitution and preferred for complex schemas
- Dapper for data access: Rejected because EF Core provides better abstraction and LINQ support

### 2. Vue 3 + TypeScript + PrimeVue Frontend

**Decision**: Use Vue 3 Composition API with TypeScript and PrimeVue component library

**Rationale**:
- Composition API provides better TypeScript support and code organization
- PrimeVue offers 90+ production-ready UI components with responsive design
- Smaller bundle size compared to Angular (Vue 3 core: 33KB, Angular: 150KB+)
- Excellent mobile performance with virtual scrolling for large lists
- Built-in accessibility (WCAG 2.0 compliant)

**Best Practices Adopted**:

1. **Project Structure**:
   - Feature-based organization: `/views` for pages, `/components` for reusable UI
   - Typed API layer in `/api` with Axios interceptors
   - Centralized state with Pinia stores (lighter than Vuex)
   - Route-based code splitting for optimal loading

2. **TypeScript Patterns**:
   - Enable `strict` mode in tsconfig.json
   - Define interfaces for all API responses
   - Use generic types for reusable components
   - Leverage type guards for runtime safety

3. **Responsive Design**:
   - Mobile-first CSS (design for 320px, then scale up)
   - Use PrimeVue's responsive utilities (p-grid, p-col)
   - Touch-friendly targets (minimum 44px as per Apple HIG and Material Design)
   - Test on real devices: iOS Safari, Android Chrome

4. **Performance Optimizations**:
   - Lazy load routes with dynamic imports
   - Use `v-memo` for expensive list renders
   - Implement virtual scrolling for lists with 100+ items (PrimeVue VirtualScroller)
   - Optimize images with responsive srcset and lazy loading
   - Use `defineAsyncComponent` for heavy components

**Alternatives Considered**:
- React: Rejected due to steeper learning curve and larger ecosystem complexity
- Angular: Rejected due to larger bundle size and more opinionated structure
- Vuetify: Rejected in favor of PrimeVue for better enterprise features and documentation

### 3. SQL Server Database Design

**Decision**: Use SQL Server 2019+ with proper indexing and constraints

**Rationale**:
- Robust transaction support (critical for financial data)
- Excellent tooling (SSMS, Azure Data Studio)
- Strong backup/recovery capabilities
- Compatible with Azure SQL for cloud migration

**Best Practices Adopted**:

1. **Indexing Strategy**:
   - Clustered index on primary keys (PetID, ContactPersonID, etc.)
   - Non-clustered indexes on foreign keys for join performance
   - Composite index on `(ReserverDate, Status)` for calendar queries
   - Index on `ContactNumber` for phone number lookups
   - Include columns in indexes for covering queries

2. **Constraints & Validation**:
   - NOT NULL constraints on required fields
   - FOREIGN KEY constraints with ON DELETE CASCADE/RESTRICT
   - CHECK constraints for data integrity (e.g., `StartDate < EndDate`)
   - UNIQUE constraints on business keys (e.g., SystemCode.Code + CodeType)

3. **Audit Trail**:
   - All tables include: CreateUser, CreateTime, ModifyUser, ModifyTime
   - Use DATETIME2 (more precise than DATETIME)
   - Populate via `EntitySaveChangesInterceptor` not triggers

4. **Performance Considerations**:
   - Partition large tables if they exceed 10 million rows
   - Use filtered indexes for frequently queried subsets
   - Enable query store for performance monitoring
   - Regular index maintenance (rebuild/reorganize)

**Alternatives Considered**:
- PostgreSQL: Rejected due to existing SQL Server infrastructure and team expertise
- MySQL: Rejected due to weaker transaction isolation levels
- MongoDB: Rejected because relational model fits business domain better

### 4. JWT Authentication & Security

**Decision**: Use JWT Bearer tokens with ASP.NET Core Identity

**Rationale**:
- Stateless authentication (no server-side session storage)
- Works well with SPA architecture
- Easy to implement role-based access control
- Compatible with mobile apps if needed later

**Best Practices Adopted**:

1. **Token Configuration**:
   - Use HMAC-SHA256 for signing (symmetric key)
   - Token expiration: 30 minutes (balance security and UX)
   - Include claims: UserId, UserName, Roles
   - Store secret key in appsettings (or Azure Key Vault in production)

2. **Security Measures**:
   - HTTPS only in production (HTTP Strict Transport Security header)
   - CORS policy: allow only known frontend origins
   - Rate limiting on authentication endpoints (10 requests/minute)
   - Password hashing with bcrypt (cost factor 12)

3. **Frontend Token Management**:
   - Store tokens in httpOnly cookies (not localStorage to prevent XSS)
   - Axios interceptor to attach token to all requests
   - Automatic token refresh before expiration
   - Redirect to login on 401 Unauthorized

**Alternatives Considered**:
- Session-based auth: Rejected because stateless JWT fits SPA architecture better
- OAuth 2.0: Not needed for internal system, JWT sufficient
- OpenID Connect: Overkill for current requirements, can add later

### 5. Responsive Design & Mobile Performance

**Decision**: Mobile-first CSS with PrimeVue responsive utilities

**Rationale**:
- 60% of staff will access system on tablets/phones
- Improves performance by loading essential styles first
- Forces focus on core functionality

**Best Practices Adopted**:

1. **Breakpoint Strategy**:
   - Mobile: 320px - 767px (touch targets ≥44px)
   - Tablet: 768px - 1023px
   - Desktop: 1024px - 1439px
   - Large: 1440px+

2. **Performance Budget**:
   - First Contentful Paint (FCP): <1.8s on 3G
   - Time to Interactive (TTI): <3.9s on 3G
   - Total bundle size: <300KB gzipped
   - Lighthouse score: >80 on mobile

3. **Touch Interactions**:
   - Minimum touch target: 44px × 44px
   - Adequate spacing between interactive elements (8px)
   - Swipe gestures for mobile navigation
   - Bottom sheet modals for mobile forms

4. **Image Optimization**:
   - Use WebP format with JPG fallback
   - Responsive images with srcset
   - Lazy loading for images below the fold
   - Compress images to <200KB

**Alternatives Considered**:
- Desktop-first: Rejected because mobile is primary use case
- Separate mobile app: Rejected due to development/maintenance cost
- Progressive Web App (PWA): Considered for Phase 2 if offline access needed

### 6. Calendar & Scheduling Patterns

**Decision**: Use FullCalendar-like functionality with PrimeVue FullCalendar

**Rationale**:
- Mature library with 10+ years of development
- Supports day/week/month views
- Drag-and-drop appointment rescheduling
- Touch-friendly for tablets

**Best Practices Adopted**:

1. **Data Structure**:
   - Store appointments with date + time separately for flexibility
   - Use UTC in database, convert to local timezone in UI
   - Index on (ReserverDate, Status) for fast calendar queries

2. **UI Patterns**:
   - Color-code appointments by status
   - Click appointment to open detail modal
   - Drag-and-drop to reschedule (desktop)
   - Swipe gesture to reschedule (mobile)

3. **Performance**:
   - Load only visible month's appointments
   - Implement virtual scrolling for day/week views
   - Cache calendar data in Pinia store

**Alternatives Considered**:
- Custom calendar component: Rejected due to complexity and testing overhead
- Google Calendar API: Not needed for internal system

## Implementation Patterns

### 1. Subscription Package Logic

**Pattern**: Reserved Count System

The subscription system uses a three-state counter to manage usage:
- `TotalUsageLimit`: Maximum number of sessions in the package
- `UsedCount`: Actually consumed sessions (appointment completed)
- `ReservedCount`: Sessions booked but not yet completed
- `RemainingCount`: Calculated as `TotalUsageLimit - UsedCount - ReservedCount`

**Workflow**:
1. **Booking**: Reserve count increments, remaining decrements
2. **Completion**: Used count increments, reserved count decrements
3. **Cancellation**: Reserved count decrements, remaining increments

**Why This Pattern**:
- Prevents overbooking (reserved counts reduce availability)
- Allows multiple future appointments
- Clear audit trail of subscription usage

### 2. File Upload Handling

**Pattern**: Direct File Storage with Path Reference

Pet photos stored in `/wwwroot/uploads/pets/` with naming: `{petId}_{guid}.{ext}`

**Security Measures**:
- Validate file type (whitelist: JPG, PNG, GIF)
- Validate file size (<10MB)
- Sanitize filename (remove path traversal characters)
- Store file path in database, not binary data
- Use antivirus scanning in production

### 3. Audit Trail Implementation

**Pattern**: SaveChangesInterceptor

Automatically populate audit fields without cluttering business logic:

```csharp
public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
{
    foreach (var entry in eventData.Context.ChangeTracker.Entries()
        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
    {
        if (entry.Entity is IAuditableEntity auditable)
        {
            if (entry.State == EntityState.Added)
            {
                auditable.CreatedDate = DateTime.UtcNow;
                auditable.CreatedBy = _userContext.CurrentUserId.ToString();
            }
            else
            {
                auditable.ModifiedDate = DateTime.UtcNow;
                auditable.ModifiedBy = _userContext.CurrentUserId.ToString();
                entry.Property("CreatedDate").IsModified = false;
                entry.Property("CreatedBy").IsModified = false;
            }
        }
    }
    return base.SavingChanges(eventData, result);
}
```

### 4. API Error Handling

**Pattern**: Centralized Exception Filter

Use global exception middleware to provide consistent error responses:

```csharp
app.UseExceptionHandler("/error");

app.MapPost("/error", (HttpContext httpContext) =>
{
    var exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
    return Results.Problem(
        title: "An error occurred",
        statusCode: exception is NotFoundException ? 404 : 500,
        detail: exception?.Message
    );
});
```

Frontend Axios interceptor handles API errors uniformly.

## Performance Targets Validation

Based on research, the following targets are achievable:

| Metric | Target | Strategy |
|--------|--------|----------|
| API Response | <2s | EF Core optimization, caching, indexing |
| Mobile Load | <3s on 3G | Code splitting, image optimization, CDN |
| Concurrent Users | 50+ | Connection pooling, async operations |
| Calendar Render | <1s | Virtual scrolling, query optimization |
| Lighthouse Score | >80 | Bundle optimization, lazy loading |

## Risk Mitigation

### Technical Risks

1. **Risk**: N+1 query problems with EF Core
   - **Mitigation**: Use `.Include()` for related entities, enable query logging in dev

2. **Risk**: Large bundle size affecting mobile performance
   - **Mitigation**: Route-based code splitting, tree shaking, bundle analysis

3. **Risk**: Concurrent edit conflicts
   - **Mitigation**: Optimistic concurrency with `ModifyTime` checks

4. **Risk**: SQL Server performance with large datasets
   - **Mitigation**: Proper indexing, query optimization, pagination

### Business Risks

1. **Risk**: Staff resistance to mobile interface
   - **Mitigation**: User testing, gradual rollout, training materials

2. **Risk**: Data migration from existing system
   - **Mitigation**: SQL import scripts, validation, parallel running period

## Next Steps (Phase 1)

1. Generate data-model.md with entity relationships
2. Create OpenAPI contracts for all endpoints
3. Write quickstart.md for development setup
4. Update agent context files

## References

- [.NET 8 Performance Improvements](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-8/)
- [Vue 3 Composition API Best Practices](https://vuejs.org/guide/extras/composition-api-faq.html)
- [PrimeVue Component Library](https://primevue.org/)
- [SQL Server Indexing Best Practices](https://docs.microsoft.com/en-us/sql/relational-databases/indexes/)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)
- [Mobile-First Design Principles](https://developer.mozilla.org/en-US/docs/Web/Progressive_web_apps/Responsive/Mobile_first)
