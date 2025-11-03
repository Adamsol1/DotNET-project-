# AI Agent Instructions for DotNET-project

This document provides essential context for AI agents working in this codebase.

## Project Architecture

This is a full-stack application with:
- .NET 8.0 Web API backend (`/backend`)
- React frontend with TypeScript (`/frontend`)

### Backend Structure (Clean Architecture)

```
backend/
├── Controllers/         # API endpoints, no business logic
├── Application/         # Business logic and services
│   ├── Interfaces/     # Contracts for services and repositories
│   ├── Services/       # Core business logic implementations
│   └── Dtos/          # Data transfer objects for API
├── Domain/             # Domain models and business rules
└── Infrastructure/     # External concerns (DB, etc)
    ├── Data/          # Entity Framework context
    └── Repositories/  # Data access implementations
```

Key architectural principles:
- Controllers use DTOs for all API responses
- Business logic belongs in Application/Services
- Repository pattern with Unit of Work for data access
- Dependency injection for services and repositories
- Strict separation of concerns between layers

## Development Workflow

### Backend Development

1. **Database Migrations**
   - Migrations are managed with Entity Framework Core
   - New models require migrations: `dotnet ef migrations add MigrationName`
   - Apply migrations: `dotnet ef database update`

2. **Data Seeding**
   - Seed data is stored in `wwwroot/seedData/*.json`
   - JSON files are loaded during app initialization
   - Example seed files: `Character.json`, `StoryNode.json`

3. **Logging**
   - Serilog is configured for structured logging
   - Logs are written to `Logs/` directory
   - Use ILogger<T> in services for consistent logging

### Frontend Development

1. **Local Development**
   - Start dev server: `npm start` in `/frontend`
   - Runs on http://localhost:3000

## Common Patterns

1. **Error Handling**
   ```csharp
   try {
       // Business logic
   } catch (Exception ex) {
       _logger.LogError(ex, "Context: Operation failed");
       throw; // or return appropriate error response
   }
   ```

2. **Service Implementation**
   ```csharp
   public class SomeService : ISomeService
   {
       private readonly IUnitOfWork _uow;
       private readonly ILogger<SomeService> _logger;

       public SomeService(IUnitOfWork uow, ILogger<SomeService> logger)
       {
           _uow = uow;
           _logger = logger;
       }
   }
   ```

## Integration Points

1. **API Endpoints**
   - All API routes prefixed with `/api/[controller]`
   - Controllers use DTOs for request/response
   - Standard REST patterns for CRUD operations

2. **Database**
   - SQLite for development (configured in appsettings.json)
   - Entity Framework Core for data access
   - Repository pattern abstracts data access

## Testing

1. **Backend Tests**
   - Located in `/tests`
   - Uses xUnit testing framework
   - `DatabaseTestHelper.cs` for DB test setup