# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

PetSalon is a .NET 6 Web API application for managing a pet grooming salon business. The system handles pets, contact persons, reservations, subscriptions, and payment records. It uses Entity Framework Core with SQL Server and includes JWT authentication.

## Architecture

This is a multi-layered .NET solution with the following projects:

- **PetSalon.Web**: Web API layer with controllers and JWT authentication
- **PetSalon.Services**: Business logic layer with service interfaces and implementations
- **PetSalon.Models**: Data models and Entity Framework DbContext
- **PetSalon.Tools**: Utility classes including database interceptors

### Core Entities

The main business entities are:

- **Pet**: Core entity with breed, gender, pricing (normal and subscription), photo upload support
- **ContactPerson**: Pet owners/contacts with phone numbers and relationship types
- **PetRelation**: Links pets to their contact persons with relationship types
- **ReserveRecord**: Appointment bookings with service and addon integration
- **Subscription**: Monthly packages for pets with time period validation
- **PaymentRecord**: Financial transactions with income/expense categorization
- **Service**: Grooming services with pricing and duration
- **ServiceAddon**: Additional services (styling, treatments) with pricing
- **PetServicePrice**: Custom pricing per pet for specific services
- **SystemCode**: Configuration/lookup data (breeds, genders, service types, statuses)

### Database

- Uses Entity Framework Core 6.0 with SQL Server
- Auto-generated context from EF Core Power Tools
- Database-first approach with SQL scripts in `/SQL` folder
- Entity interceptor for audit fields (create/modify user/time)

## Development Commands

### Backend (.NET 6 API)

```bash
# Build the entire solution
dotnet build PetSalon/PetSalon.sln

# Run the web API (from PetSalon.Web directory)
cd PetSalon/PetSalon.Web
dotnet run

# The API will be available at http://localhost:5150 with Swagger UI
```

### Frontend (Vue.js)

```bash
# Install dependencies
cd PetSalon.Frontend
npm install

# Run development server
npm run dev
# Frontend will be available at http://localhost:3000 (or 3001 if 3000 is busy)

# Build for production
npm run build

# Type checking
npm run type-check

# Linting
npm run lint
```


### Initialize Database with System Codes

```sql
-- Run the SystemCode initialization scripts in order:
-- SQL/70-InintialData/SystemCode-Breed.sql
-- SQL/70-InintialData/SystemCode-Gender.sql
-- SQL/70-InintialData/SystemCode-ServiceType.sql
-- SQL/70-InintialData/SystemCode-ReservationStatus.sql
-- SQL/70-InintialData/SystemCode-Relationship.sql
-- SQL/70-InintialData/SystemCode-PaymentType.sql
-- SQL/70-InintialData/SystemCode-AddonType.sql
```

### Database Operations

```bash
# Run Entity Framework migrations (if using migrations)
dotnet ef database update --project PetSalon.Models --startup-project PetSalon.Web

# Generate models from existing database (when schema changes)
# Uses EF Core Power Tools configuration in efpt.config.json
```

### Testing

```bash
# Run tests (if test projects exist)
dotnet test PetSalon/PetSalon.sln
```

## Key Configuration

### Connection Strings

- Database connection configured in `appsettings.json` as "DefaultConnection"
- Uses SQL Server with integrated security

### JWT Settings

- JWT authentication implemented with configurable issuer and signing key
- Token expiration defaults to 30 minutes
- Admin and Users roles assigned by default

### Launch Settings

**Backend:**
- Development profile runs on port 5150
- Swagger UI available at `/swagger` endpoint
- IIS Express profile also configured

**Frontend:**
- Development server runs on port 3000 (or 3001 if busy)
- Automatic proxy to backend API at localhost:5150
- Hot module replacement for instant updates
- Vue DevTools integration

## Development Notes

### Service Registration

Services are registered in `Program.cs`:

- `ICommonService` → `CommonService` (SystemCode management)
- `IPetService` → `PetService` (Pet CRUD and photo upload)
- `IContactPersonService` → `ContactPersonService` (Contact management with relationships)
- `IReservationService` → `ReservationService` (Booking system with subscription integration)
- `JwtHelpers` as singleton

### Database Context

- `PetSalonContext` is auto-generated and should not be manually edited
- Uses `EntitySaveChangesInterceptor` for audit trail
- Connection string builder used for configuration
- New tables: Service, ServiceAddon, PetServicePrice, ReservationService, ReservationAddon
- Complex relationships support subscription-based pricing and service customization

### API Structure

- Controllers inherit from `BaseController`
- JWT authentication middleware configured
- Swagger documentation enabled for development

### Current System Codes

Based on SQL initialization files:
- **Breed**: Various dog breeds (Poodle, Golden Retriever, etc.)
- **Gender**: Male/Female
- **ServiceType**: Bath, Grooming, Nail trimming, Special styling, Monthly packages
- **ReservationStatus**: Pending, Confirmed, In Progress, Completed, Cancelled, No Show
- **Relationship**: Owner, Father, Mother, Brother, Sister, Family, Friend, Caregiver
- **IncomeType**: Grooming, Retail, Addon, Subscription revenue
- **ExpenseType**: Utilities, Phone, Rent, Supplies, Equipment, Marketing
- **AddonType**: Styling, Poodle foot, Flea treatment, Nail painting, Perfume, SPA

## API Endpoints

### Pet Management
- `GET /api/pet` - List all pets
- `GET /api/pet/{id}` - Get pet details
- `POST /api/pet` - Create new pet
- `PUT /api/pet/{id}` - Update pet
- `DELETE /api/pet/{id}` - Delete pet
- `POST /api/pet/{id}/photo` - Upload pet photo
- `GET /api/pet/contact/{contactPersonId}` - Get pets by contact person

### Contact Person Management
- `GET /api/contactperson` - List all contact persons
- `GET /api/contactperson/{id}` - Get contact person details
- `POST /api/contactperson` - Create new contact person
- `PUT /api/contactperson/{id}` - Update contact person
- `DELETE /api/contactperson/{id}` - Delete contact person
- `POST /api/contactperson/{contactId}/pets/{petId}` - Link contact to pet
- `DELETE /api/contactperson/{contactId}/pets/{petId}` - Unlink contact from pet

### System Code Management
- `GET /api/common/systemcodes/{codeType}` - Get codes by type
- `GET /api/common/systemcodes/{codeType}/{code}` - Get specific code
- `GET /api/common/systemcode-types` - Get all code types
- `POST /api/common/systemcodes` - Create new system code
- `PUT /api/common/systemcodes/{id}` - Update system code
- `DELETE /api/common/systemcodes/{id}` - Delete system code

### File Upload Support
- Pet photos uploaded to `/wwwroot/uploads/pets/`
- Supported formats: JPG, PNG, GIF
- File naming: `{petId}_{guid}.{extension}`

## Project Structure

### Backend Structure
```
PetSalon/
├── PetSalon.Web/           # Web API controllers and startup
├── PetSalon.Services/      # Business logic layer
├── PetSalon.Models/        # Data models and DTOs
└── PetSalon.Tools/         # Utility classes
```

### Frontend Structure
```
PetSalon.Frontend/
├── src/
│   ├── views/              # Page components
│   ├── stores/             # Pinia state management
│   ├── api/                # API service layer
│   ├── types/              # TypeScript type definitions
│   ├── utils/              # Utility functions
│   └── router/             # Vue Router configuration
├── index.html              # Entry HTML
└── vite.config.ts          # Vite configuration
```

## Working with the Codebase

### Backend Development
1. **Services layer** (`PetSalon.Services`) contains business logic with clear interfaces
2. **Controllers** (`PetSalon.Web/Controllers`) handle HTTP requests with proper error handling
3. **Models** are auto-generated - modify database schema for changes
4. **DTOs** (`PetSalon.Models/DTOs`) for API data transfer and calculations
5. **SystemCodes** provide configurable lookup data
6. Use dependency injection for service dependencies
7. Follow existing patterns for audit fields and error handling
8. All services support async operations
9. File uploads use proper validation and secure storage

### Frontend Development
1. **Vue 3 Composition API** with TypeScript for type safety
2. **Element Plus** for consistent UI components
3. **Pinia** for centralized state management
4. **Axios** with interceptors for API communication
5. **Vue Router** with navigation guards for authentication
6. **Auto-import** for Vue APIs and Element Plus components
7. **Tailwind CSS v4** for utility-first styling
8. Follow Vue 3 best practices and composition patterns

### Tailwind CSS Integration

#### Setup (Tailwind CSS v4.0 - 2025)

```bash
# Install Tailwind CSS v4 with first-party Vite plugin
npm install -D @tailwindcss/vite

# Add to vite.config.ts
import tailwindcss from '@tailwindcss/vite'

export default {
  plugins: [
    tailwindcss(),
    // other plugins...
  ]
}

# Import in src/assets/main.css
@import "tailwindcss";
```

#### Key Features in v4.0:
- **5x faster builds** with new high-performance engine
- **100x faster incremental builds** (measured in microseconds)
- **Zero configuration** with automatic content detection
- **CSS-first configuration** directly in CSS files
- **Built-in container queries** support
- **Modern CSS features** using cascade layers and @property

#### Best Practices for Vue Integration:

1. **Component-Level Styling:**
```vue
<template>
  <button class="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500">
    Click Me
  </button>
</template>
```

2. **Responsive Design:**
```vue
<template>
  <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
    <!-- Content -->
  </div>
</template>
```

3. **Dynamic Classes with Vue:**
```vue
<script setup>
const isActive = ref(false)
const buttonClass = computed(() => 
  isActive.value 
    ? 'bg-green-600 hover:bg-green-700' 
    : 'bg-gray-600 hover:bg-gray-700'
)
</script>

<template>
  <button :class="['px-4 py-2 text-white rounded', buttonClass]">
    Toggle
  </button>
</template>
```

4. **Theme Configuration in CSS:**
```css
@import "tailwindcss";

@theme {
  --color-primary: #1e40af;
  --color-secondary: #64748b;
  --font-family-custom: "Inter", sans-serif;
}
```

#### Performance Optimization:
- Automatic purging of unused styles in production
- JIT (Just-in-Time) compilation for dynamic class generation
- Minimal CSS bundle size with tree-shaking
- First-party Vite integration for optimal development experience

# AI Rules for {{project-name}}

{{project-description}}

## FRONTEND

### Guidelines for VUE

#### VUE_CODING_STANDARDS

- Use the Composition API instead of the Options API for better type inference and code reuse
- Implement <script setup> for more concise component definitions
- Use Suspense and async components for handling loading states during code-splitting
- Leverage the defineProps and defineEmits macros for type-safe props and events
- Use the new defineOptions for additional component options
- Implement provide/inject for dependency injection instead of prop drilling in deeply nested components
- Use the Teleport component for portal-like functionality to render UI elsewhere in the DOM
- Leverage ref over reactive for primitive values to avoid unintended unwrapping
- Use v-memo for performance optimization in render-heavy list rendering scenarios
- Implement shallow refs for large objects that don't need deep reactivity


## BACKEND

### Guidelines for DOTNET

#### ENTITY_FRAMEWORK

- Use the repository and unit of work patterns to abstract data access logic and simplify testing
- Implement eager loading with Include() to avoid N+1 query problems for {{entity_relationships}}
- Use migrations for database schema changes and version control with proper naming conventions
- Apply appropriate tracking behavior (AsNoTracking() for read-only queries) to optimize performance
- Implement query optimization techniques like compiled queries for frequently executed database operations
- Use value conversions for complex property transformations and proper handling of {{custom_data_types}}

#### ASP_NET

- Use minimal APIs for simple endpoints in .NET 6+ applications to reduce boilerplate code
- Implement the mediator pattern with MediatR for decoupling request handling and simplifying cross-cutting concerns
- Use API controllers with model binding and validation attributes for {{complex_data_models}}
- Apply proper response caching with cache profiles and ETags for improved performance on {{high_traffic_endpoints}}
- Implement proper exception handling with ExceptionFilter or middleware to provide consistent error responses
- Use dependency injection with scoped lifetime for request-specific services and singleton for stateless services


## DATABASE

### Guidelines for SQL

#### SQLSERVER

- Use parameterized queries to prevent SQL injection
- Implement proper indexing strategies based on query patterns
- Use stored procedures for complex business logic that requires database access to {{business_entities}}


