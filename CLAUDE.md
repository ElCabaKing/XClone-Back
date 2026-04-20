# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an **XClone** (Twitter/X clone) backend API built with **.NET 10** using Clean Architecture principles. The project is organized as a modular monolith with vertical slice architecture patterns.

## Architecture

### Project Structure (Clean Architecture)

The solution follows Clean Architecture with 6 projects:

```
AppWeb.slnx
├── Domain/           # Core entities, interfaces, enums, exceptions (no dependencies)
├── Application/      # Use cases, handlers, commands (depends on Domain)
├── Infrastructure/   # External services, repositories, EF Core, auth (depends on Application, Domain)
├── AppWeb/           # API controllers, middleware, DI registration (depends on all except Test)
├── Shared/           # Shared helpers, constants, generic types
└── Test/             # xUnit tests with Moq (depends on Application, Domain)
```

### Key Architectural Patterns

**Vertical Slice Architecture**: Features are organized as self-contained modules with Command/Handler pairs:

```
Application/Modules/{Feature}/{Action}/
├── {Action}Command.cs      # Input DTO (record-based with primary constructor)
├── {Action}Handler.cs      # Business logic handler
└── {Action}Response.cs     # Output DTO
```

**Repository + Unit of Work Pattern**:
- `IGenericRepository<T>`: Base CRUD operations in `Domain/Interfaces/`
- `IUOW`: Aggregates repositories and provides `SaveChangesAsync()`
- Repositories are instantiated lazily in `Infrastructure/Unity/UOW.cs`

**Custom Handler Pattern (not MediatR)**: Handlers are registered explicitly in `Application/DependencyInjection.cs`:

```csharp
services.AddScoped<CreateUserHandler>();
services.AddScoped<UpdateUserHandler>();
```

Handlers are injected directly into controllers and called via `await handler.Handle(command)`.

## Development Commands

### Build
```bash
dotnet build AppWeb.slnx
```

### Run
```bash
cd AppWeb && dotnet run
# or from root:
dotnet run --project AppWeb/AppWeb.csproj
```

The API runs with Scalar OpenAPI documentation at `/scalar/v1` (development mode only).

### Tests
```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~CreateUserTest"

# Run with verbosity
dotnet test --verbosity normal
```

### Database (EF Core)
```bash
# Add migration
dotnet ef migrations add {MigrationName} --project Infrastructure --startup-project AppWeb

# Update database
dotnet ef database update --project Infrastructure --startup-project AppWeb
```

## Technology Stack

- **.NET 10** - Target framework
- **EF Core 10** - SQL Server database access (scaffolded from existing DB)
- **JWT Bearer Authentication** - Cookie-based token auth
- **BCrypt.Net** - Password hashing
- **Cloudinary** - Image/file storage
- **Mailjet** - Email service
- **Serilog + MongoDB** - Structured logging to MongoDB
- **Scalar.AspNetCore** - OpenAPI documentation UI
- **xUnit + Moq** - Testing framework

## Configuration

Configuration is loaded from **environment variables** via `.env` file (at repo root):

```bash
# Required environment variables
DefaultConnection="Server=localhost,1433;..."
JWT__Key="..."
JWT__Issuer="..."
JWT__Audience="..."
Cloudinary__CloudName="..."
Cloudinary__ApiKey="..."
Cloudinary__ApiSecret="..."
MongoDb__ConnectionString="..."
Mailjet__ApiKey="..."
Mailjet__ApiSecret="..."
```

## Key Conventions

**Commands**: Use primary constructors for immutable input data:
```csharp
public class CreateUserCommand(string username, string email, ...) { ... }
```

**Handlers**: Injected dependencies via primary constructor, single public `Handle` method:
```csharp
public class CreateUserHandler(IEmailService email, IPasswordService password, IUOW uow, ICloudStorage cloud)
{
    public async Task<GenericResponse<CreateUserResponse>> Handle(CreateUserCommand command) { ... }
}
```

**Controllers**: Map `Request` objects to `Command` objects, return `GenericResponse<T>`:
```csharp
var command = new CreateUserCommand(request.Username, ...);
var result = await handler.Handle(command);
return Created("/users/" + result.Data.Id, result);
```

**Error Handling**: Custom domain exceptions are caught by `ErrorHandlerMiddleware`:
- `NotFoundException` → 404
- `BadRequestException` → 400
- `AlreadyExistsException` → 409 (mapped from middleware logic)
- `ServiceErrorException` → 500
- Other exceptions → 500 with trace ID

**Rate Limiting**: Fixed window limiter configured at 5 requests/minute per client.

## Testing Patterns

Tests use **Moq** for mocking with repository pattern:

```csharp
var uowMock = new Mock<IUOW>();
var userRepositoryMock = new Mock<IUserRepository>();
uowMock.Setup(uow => uow.UserRepository).Returns(userRepositoryMock.Object);
```

Test naming convention: `{Method}_{Scenario}_{ExpectedResult}`

## Important Notes

- **No MediatR**: Handlers are manually registered and injected (not using MediatR library)
- **Cookie-based JWT**: Tokens are read from `access_token` cookie, not Authorization header
- **Spanish error messages**: Response constants are in Spanish
- **Hardcoded connection string**: `XDbContext` has a hardcoded fallback connection string (line 57)
- **CORS**: Configured for `http://localhost:3000` only
