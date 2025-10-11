# PetSalon Quickstart Guide

**Feature**: 001-petsalon-system
**Date**: 2025-10-11
**Audience**: Developers setting up the PetSalon system for the first time

## Prerequisites

Before you begin, ensure you have the following installed:

### Required Software

- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** - for backend development
- **[Node.js 18+](https://nodejs.org/)** - for frontend development
- **[SQL Server 2019+](https://www.microsoft.com/sql-server)** - for database
  - Alternative: Docker with SQL Server image (see Docker Setup below)
- **[Git](https://git-scm.com/)** - for version control
- **Code Editor**: Visual Studio 2022, VS Code, or JetBrains Rider

### Optional Tools

- **[Docker Desktop](https://www.docker.com/products/docker-desktop)** - for containerized SQL Server
- **[SQL Server Management Studio (SSMS)](https://aka.ms/ssmsfullsetup)** - for database management
- **[Azure Data Studio](https://aka.ms/azuredatastudio)** - cross-platform database tool
- **[Postman](https://www.postman.com/)** - for API testing

## Quick Start (5 Minutes)

### 1. Clone the Repository

```bash
git clone <repository-url>
cd PetSalon
git checkout 001-petsalon-system
```

### 2. Database Setup (Choose One Option)

#### Option A: Docker SQL Server (Recommended for Mac/Linux)

```bash
# Copy environment template
cp .env.example .env

# Edit .env and set a strong SA password
nano .env  # or use your preferred editor

# Start SQL Server container
docker-compose up -d

# Verify container is running
docker ps
```

The SQL Server will be available at:
- **Host**: localhost
- **Port**: 1433
- **Username**: sa
- **Password**: (from your .env file)

#### Option B: Local SQL Server (Windows)

1. Open SQL Server Management Studio
2. Connect to your local instance
3. Create a new database named `PetSalon`

### 3. Initialize Database Schema

```bash
# Navigate to SQL scripts directory
cd SQL

# Execute table creation scripts (in order)
# Using SSMS:
# 1. Open each file in 10-Table/ directory
# 2. Execute them in alphabetical order

# Using sqlcmd (command line):
cd 10-Table
for %f in (*.sql) do sqlcmd -S localhost -U sa -P YourPassword -d PetSalon -i %f

# Initialize system codes
cd ../70-InintialData
for %f in (*.sql) do sqlcmd -S localhost -U sa -P YourPassword -d PetSalon -i %f
```

**Tables Created**:
- Pet, ContactPerson, PetRelation
- ReserveRecord, ReservationService, ReservationAddon
- Subscription, SubscriptionType
- Service, ServiceAddon, PetServicePrice
- PaymentRecord, SystemCode

**System Codes Loaded**:
- Breed (dog breeds)
- Gender (Male, Female)
- ServiceType (Bath, Grooming, etc.)
- ReservationStatus (Pending, Confirmed, etc.)
- Relationship (Owner, Family, etc.)
- IncomeType, ExpenseType

### 4. Configure Backend

```bash
cd ../PetSalon.Backend/PetSalon.Web

# Edit appsettings.json with your database connection
nano appsettings.json
```

Update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PetSalon;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Issuer": "PetSalonAPI",
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "ExpirationMinutes": 30
  }
}
```

### 5. Run Backend

```bash
# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build

# Run the API server
dotnet run

# API will start at: http://localhost:5150
# Swagger UI: http://localhost:5150/swagger
```

You should see:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5150
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### 6. Configure Frontend

Open a new terminal:

```bash
cd PetSalon.Frontend

# Install NPM packages
npm install

# Create environment file
cp .env.example .env.local

# Edit .env.local (should point to your backend)
echo "VITE_API_BASE_URL=http://localhost:5150/api" > .env.local
```

### 7. Run Frontend

```bash
# Start development server
npm run dev

# Frontend will start at: http://localhost:3000
```

You should see:
```
VITE v5.3.0  ready in 500 ms

➜  Local:   http://localhost:3000/
➜  Network: use --host to expose
➜  press h + enter to show help
```

### 8. Verify Installation

#### Test Backend API

Open browser to: http://localhost:5150/swagger

Try these endpoints:
1. **GET /api/common/systemcode-types** - Should return list of code types
2. **GET /api/common/systemcodes/Breed** - Should return dog breeds
3. **GET /api/pet** - Should return empty array (no pets yet)

#### Test Frontend

Open browser to: http://localhost:3000

You should see the PetSalon dashboard. Try:
1. Navigate to "Pets" page
2. Click "Add New Pet"
3. Fill in the form and submit

## Development Workflow

### Backend Development

```bash
cd PetSalon.Backend/PetSalon.Web

# Watch mode (auto-restart on changes)
dotnet watch run

# Run tests (if available)
cd ..
dotnet test

# Generate EF Core models from database
# After modifying database schema:
# 1. Update SQL scripts
# 2. Execute scripts against database
# 3. Use EF Core Power Tools in Visual Studio to regenerate models
```

### Frontend Development

```bash
cd PetSalon.Frontend

# Development server with hot reload
npm run dev

# Type checking
npm run type-check

# Linting
npm run lint

# Build for production
npm run build

# Preview production build
npm run preview
```

### Database Changes

1. **Modify Schema**:
   ```bash
   cd SQL/10-Table
   # Edit or create new SQL script
   nano Pet.sql
   ```

2. **Execute Against Database**:
   ```bash
   sqlcmd -S localhost -U sa -P YourPassword -d PetSalon -i Pet.sql
   ```

3. **Regenerate EF Models**:
   - Open Visual Studio
   - Right-click on PetSalon.Models project
   - Select "EF Core Power Tools" → "Reverse Engineer"
   - Select tables and generate

4. **Update DTOs if Needed**:
   ```bash
   cd PetSalon.Models/DTOs
   # Update or create DTO classes
   ```

## Common Tasks

### Adding a New System Code Type

1. **Insert into database**:
   ```sql
   INSERT INTO SystemCode (CodeType, Code, Name, CodeName, Sort, IsActive)
   VALUES ('NewType', 'VALUE1', 'Display Name', 'Display Name', 1, 1);
   ```

2. **Use in frontend**:
   ```typescript
   // Will be automatically available via /api/common/systemcodes/NewType
   const codes = await systemCodeApi.getByType('NewType');
   ```

### Creating a New API Endpoint

1. **Add method to service interface** (PetSalon.Services/Interfaces):
   ```csharp
   public interface IPetService {
       Task<IList<Pet>> SearchPets(string searchTerm);
   }
   ```

2. **Implement in service** (PetSalon.Services/Implementations):
   ```csharp
   public async Task<IList<Pet>> SearchPets(string searchTerm) {
       return await _context.Pets
           .Where(p => p.PetName.Contains(searchTerm))
           .ToListAsync();
   }
   ```

3. **Add controller action** (PetSalon.Web/Controllers):
   ```csharp
   [HttpGet("search")]
   public async Task<ActionResult<IList<PetDto>>> SearchPets([FromQuery] string q) {
       var pets = await _petService.SearchPets(q);
       return Ok(pets.Select(p => MapToDto(p)));
   }
   ```

4. **Add frontend API call** (PetSalon.Frontend/src/api):
   ```typescript
   export const petApi = {
       searchPets: async (searchTerm: string): Promise<PetDto[]> => {
           const response = await axios.get(`/pet/search?q=${searchTerm}`);
           return response.data;
       }
   };
   ```

### Adding a New Vue Component

1. **Create component file** (PetSalon.Frontend/src/components):
   ```vue
   <script setup lang="ts">
   import { ref } from 'vue';

   const props = defineProps<{
       petId: number;
   }>();

   const pet = ref<Pet | null>(null);

   // Component logic
   </script>

   <template>
       <div class="pet-card">
           <!-- Template -->
       </div>
   </template>

   <style scoped>
   .pet-card {
       /* Styles */
   }
   </style>
   ```

2. **Use in parent component**:
   ```vue
   <script setup lang="ts">
   import PetCard from '@/components/PetCard.vue';
   </script>

   <template>
       <PetCard :pet-id="123" />
   </template>
   ```

## Testing

### Manual Testing

#### Backend (Swagger UI)

1. Navigate to http://localhost:5150/swagger
2. Expand endpoint (e.g., GET /api/pet)
3. Click "Try it out"
4. Click "Execute"
5. Verify response

#### Frontend (Browser DevTools)

1. Open browser DevTools (F12)
2. Go to Network tab
3. Interact with UI
4. Verify API calls and responses
5. Check Console for errors

### Responsive Design Testing

#### Desktop Testing
- Chrome DevTools: F12 → Toggle Device Toolbar (Ctrl+Shift+M)
- Test breakpoints: 1024px, 1440px

#### Mobile Testing
- Chrome DevTools: Select iPhone 12 Pro, Samsung Galaxy S20
- Test touch interactions
- Verify 44px minimum touch targets
- Test portrait and landscape

#### Real Device Testing
1. Find your local IP: `ipconfig` (Windows) or `ifconfig` (Mac/Linux)
2. Update .env.local: `VITE_API_BASE_URL=http://192.168.1.100:5150/api`
3. Restart frontend: `npm run dev -- --host`
4. Access from mobile: http://192.168.1.100:3000

## Troubleshooting

### Backend Issues

**Issue**: Cannot connect to database
```bash
# Check SQL Server is running
docker ps  # if using Docker

# Test connection
sqlcmd -S localhost -U sa -P YourPassword -Q "SELECT @@VERSION"

# Check connection string in appsettings.json
```

**Issue**: Port 5150 already in use
```bash
# Find process using port
netstat -ano | findstr :5150  # Windows
lsof -i :5150  # Mac/Linux

# Kill process or change port in launchSettings.json
```

**Issue**: EF Core errors
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# Rebuild
dotnet clean
dotnet build
```

### Frontend Issues

**Issue**: Cannot connect to backend API
```bash
# Check backend is running
curl http://localhost:5150/api/common/systemcode-types

# Verify VITE_API_BASE_URL in .env.local
cat .env.local

# Check browser Console for CORS errors
```

**Issue**: Port 3000 already in use
```bash
# Change port in vite.config.ts
server: {
  port: 3001
}
```

**Issue**: TypeScript errors
```bash
# Check TypeScript version
npm list typescript

# Run type checker
npm run type-check

# Rebuild node_modules if needed
rm -rf node_modules package-lock.json
npm install
```

### Database Issues

**Issue**: SystemCode data not loaded
```bash
# Re-run initial data scripts
cd SQL/70-InintialData
sqlcmd -S localhost -U sa -P YourPassword -d PetSalon -i SystemCode-Breed.sql
# Repeat for other SystemCode files
```

**Issue**: Schema out of sync
```bash
# Drop and recreate database (⚠️ WARNING: Deletes all data)
sqlcmd -S localhost -U sa -P YourPassword -Q "DROP DATABASE PetSalon"
sqlcmd -S localhost -U sa -P YourPassword -Q "CREATE DATABASE PetSalon"

# Re-run all table scripts
cd SQL/10-Table
for %f in (*.sql) do sqlcmd -S localhost -U sa -P YourPassword -d PetSalon -i %f
```

## Production Deployment

See [Deployment Guide](../../../docs/DEPLOYMENT.md) for production deployment instructions.

Quick checklist:
- [ ] Set strong JWT secret key
- [ ] Enable HTTPS (SSL certificate)
- [ ] Configure CORS for production domain
- [ ] Set up database backups
- [ ] Configure logging (Serilog, Application Insights)
- [ ] Set up monitoring and alerts
- [ ] Use environment variables for secrets
- [ ] Enable rate limiting
- [ ] Configure CDN for static assets
- [ ] Run security audit

## Next Steps

1. **Explore the API**: Open Swagger UI and test all endpoints
2. **Read the Spec**: Review [spec.md](./spec.md) for user stories and requirements
3. **Review Data Model**: Check [data-model.md](./data-model.md) for entity relationships
4. **Check API Contracts**: See [contracts/openapi.yaml](./contracts/openapi.yaml) for full API documentation
5. **Read the Constitution**: Understand governance principles in [.specify/memory/constitution.md](../../../.specify/memory/constitution.md)

## Support

- **Technical Issues**: Check [CLAUDE.md](../../../CLAUDE.md) for development guidance
- **Feature Requests**: Create an issue in the project repository
- **Questions**: Contact the development team

## Resources

- [.NET 8 Documentation](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-8)
- [Vue 3 Documentation](https://vuejs.org/guide/introduction.html)
- [PrimeVue Components](https://primevue.org/installation)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [SQL Server Documentation](https://learn.microsoft.com/sql/sql-server/)
