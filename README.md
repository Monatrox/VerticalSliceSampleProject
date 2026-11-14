# VerticalSliceSampleProject

> [!NOTE]
> While the code in this project wasn't entirely generated with AI, nearly this entire readme WAS generated with AI. Take it all with a grain of salt. I don't recommend using this as a proper template, so read that into all of the advice and information given below. This project was used as a playground to mess around with the idea of demonstrating a vertical slice architecture. I don't recommend using it in production as it is now. 

## Overview

This project uses a somewhat weak **Vertical Slice Architecture** where each feature module (e.g., Weather, Todo) is organized into four distinct layers. Each module is self-contained and focused on a specific business domain, allowing teams to work independently and deploy features incrementally. Overall, it's a very quick and dirty modular monolith architecture mainly for demonstration purposes. Real production applications would require some real thought, you wouldn't want to copy this structure. 

## Module Layers

Each module follows a consistent four-layer structure:

```
[Module].Domain/          # Domain entities and business rules
[Module].Public/          # Public contracts and DTOs
[Module].Infrastructure/  # Data access and external services
[Module].Application/     # API endpoints and use cases
```

### Example: Weather Module

```
Weather.Domain/
  └── Objects/
      ├── Location.cs
      └── WeatherForecast.cs

Weather.Public/
  └── DTO/
      └── WeatherForecastDto.cs
  └── Abstractions/
      └── IWeatherForecastProvider.cs

Weather.Infrastructure/
  └── Persistence/
      ├── WeatherDbContext.cs
      └── WeatherDbContextFactory.cs
  └── Services/
      ├── LocationService.cs
      └── WeatherForecastService.cs
  └── Extensions/
      └── WeatherBuilderExtensions.cs

Weather.Application/
  └── Endpoints/
      ├── GetWeatherForecast/
      ├── CreateWeatherForecast/
      └── CreateLocation/
  └── Extensions/
      └── WeatherServiceExtensions.cs
```

---

## Layer Breakdown

### 1. **[Module].Domain** - Domain Layer

**Purpose:** Contains pure domain entities, value objects, and domain logic. No dependencies on infrastructure or frameworks.

**Contents:**
- `Objects/` - Domain entities (e.g., `TodoItem`, `Location`, `WeatherForecast`)
- `Enums/` - Domain enumerations
- `Abstractions/` - Domain interfaces (repository patterns, domain services)

**Dependencies:**
- `Microsoft.EntityFrameworkCore` (for annotations only)

**Example:**
```csharp
public class TodoItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }
    
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Priority { get; set; }
}
```

**Key Principles:**
- ✅ Contains business entities and their invariants
- ✅ Minimal dependencies (only EF Core attributes)
- ✅ Represents the "ubiquitous language" of the domain
- ❌ No database context or queries
- ❌ No API-specific logic

---

### 2. **[Module].Public** - Public Contracts Layer

**Purpose:** Defines the public API surface that other modules can consume. This creates a stable contract layer between modules.

**Contents:**
- `DTO/` - Data Transfer Objects (public representations)
- `Abstractions/` - Public interfaces (e.g., `ITodoItemProvider`, `IWeatherForecastProvider`)

**Dependencies:**
- `Microsoft.EntityFrameworkCore` (minimal)

**Example:**
```csharp
// DTO
public sealed record TodoItemDto(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime CreatedAt,
    int Priority);

// Provider Interface
public interface ITodoItemProvider
{
    ValueTask<IReadOnlyList<TodoItemDto>> GetAllAsync(CancellationToken ct = default);
    ValueTask<TodoItemDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
```

**Key Principles:**
- ✅ Stable public contracts for inter-module communication
- ✅ DTOs prevent exposing internal domain structure
- ✅ Interfaces allow for testing and mocking
- ❌ No implementation details
- ❌ No framework-specific types (e.g., no `IActionResult`)

---

### 3. **[Module].Infrastructure** - Infrastructure Layer

**Purpose:** Handles all external concerns like database access, third-party APIs, file systems, etc.

**Contents:**
- `Persistence/` - DbContext, migrations, database configuration
- `Services/` - Service implementations (CRUD operations, business logic)
- `Extensions/` - Dependency injection setup for Aspire/hosting

**Dependencies:**
- `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL`
- `Microsoft.EntityFrameworkCore.Design`
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- Project reference to `[Module].Domain`

**Example:**
```csharp
// DbContext
public class TodoDbContext : DbContext
{
    public DbSet<TodoItem> TodoItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.ToTable("TodoItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
        });
        
        SeedData(modelBuilder);
    }
}

// Service
public interface ITodoItemService
{
    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TodoItem> CreateAsync(TodoItem item, CancellationToken ct = default);
}

public class TodoItemService : ITodoItemService
{
    private readonly TodoDbContext _context;
    
    public async Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.TodoItems.FirstOrDefaultAsync(t => t.Id == id, ct);
}

// Extension for registration
public static class TodoBuilderExtensions
{
    public static IServiceCollection AddTodoServices(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<TodoDbContext>("tododb");
        builder.Services.AddScoped<ITodoItemService, TodoItemService>();
        return builder.Services;
    }
}
```

**Key Principles:**
- ✅ Owns database migrations and schema
- ✅ Implements data access patterns
- ✅ Handles connection strings and Aspire integration
- ✅ Contains seed data for development
- ❌ No HTTP/API concerns
- ❌ No endpoint routing

---

### 4. **[Module].Application** - Application Layer

**Purpose:** Defines the API endpoints and orchestrates use cases using FastEndpoints.

**Contents:**
- `Endpoints/` - FastEndpoints organized by feature (one folder per endpoint)
    - `[Feature]/[Feature]Endpoint.cs`
    - `[Feature]/[Feature]Request.cs`
    - `[Feature]/[Feature]Response.cs`
- `Extensions/` - FastEndpoints registration
- `Handlers/` - Event handlers, MediatR handlers (if applicable)
- `Events/` - Application events

**Dependencies:**
- `FastEndpoints`
- `Microsoft.EntityFrameworkCore`
- Project references to `VerticalSlice.Shared`, `[Module].Domain`, `[Module].Infrastructure`

**Example:**
```csharp
// Request
public class GetTodoItemRequest
{
    public Guid Id { get; set; }
}

// Response
public class GetTodoItemResponse : ApiResponse<TodoItem>
{
    public GetTodoItemResponse(TodoItem todoItem) : base(todoItem) { }
    public GetTodoItemResponse(HttpStatusCode code, string? message = null) 
        : base(code, message) { }
}

// Endpoint
public class GetTodoItemEndpoint : Endpoint<GetTodoItemRequest, GetTodoItemResponse>
{
    private readonly ITodoItemService _service;

    public GetTodoItemEndpoint(ITodoItemService service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Get("/todos/{id}");
        AllowAnonymous();
    }

    public override async Task<GetTodoItemResponse> ExecuteAsync(
        GetTodoItemRequest request, CancellationToken ct)
    {
        var data = await _service.GetByIdAsync(request.Id, ct);
        return data is null 
            ? new GetTodoItemResponse(HttpStatusCode.NotFound)
            : new GetTodoItemResponse(data);
    }
}

// Extension for registration
public static class TodoServiceExtensions
{
    public static IServiceCollection AddTodoEndpoints(this IServiceCollection services)
    {
        services.AddFastEndpoints();
        return services;
    }
}
```

**Key Principles:**
- ✅ One endpoint per feature/use case
- ✅ Co-located request/response/endpoint
- ✅ Thin layer that orchestrates services
- ✅ Uses `ApiResponse<T>` from shared layer for consistency
- ❌ Minimal business logic (delegate to services)
- ❌ No direct database access

---

## Integration with Main Application

### 1. **Aspire AppHost Setup**

In `VerticalSlice.Aspire/Program.cs`, define your databases:

```csharp
var weatherDb = builder
    .AddPostgres("weatherpostgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("weatherdb");

var todoDb = builder
    .AddPostgres("todopostgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("tododb");
```

### 2. **API Project Setup**

In `VerticalSlice.Api/Program.cs`:

```csharp
// Register infrastructure services
builder.AddWeatherServices();
builder.AddTodoServices();

// Register endpoints
builder.Services.AddWeatherEndpoints();
builder.Services.AddTodoEndpoints();

var app = builder.Build();

// Use FastEndpoints
app.UseHttpsRedirection();
app.UseFastEndpoints();

app.Run();
```

### 3. **Migration Service Setup**

In `VerticalSlice.MigrationService/MigrationWorker.cs`:

```csharp
public class MigrationWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        
        var weatherDb = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
        await weatherDb.Database.MigrateAsync(ct);
        
        var todoDb = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        await todoDb.Database.MigrateAsync(ct);
    }
}
```

---

## Creating a New Module

### Step-by-Step Guide

#### 1. Create Domain Layer
```bash
dotnet new classlib -n [Module].Domain
dotnet add [Module].Domain package Microsoft.EntityFrameworkCore
```

- Create `Objects/` folder with domain entities
- Add annotations (`[Key]`, `[Required]`, etc.)

#### 2. Create Public Layer
```bash
dotnet new classlib -n [Module].Public
```

- Create `DTO/` folder with public DTOs (use records)
- Create `Abstractions/` folder with provider interfaces

#### 3. Create Infrastructure Layer
```bash
dotnet new classlib -n [Module].Infrastructure
dotnet add [Module].Infrastructure package Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add [Module].Infrastructure package Microsoft.EntityFrameworkCore.Design
dotnet add [Module].Infrastructure reference [Module].Domain
```

- Create `Persistence/[Module]DbContext.cs`
- Create `Persistence/[Module]DbContextFactory.cs` (for migrations)
- Create `Services/` with service interfaces and implementations
- Create `Extensions/[Module]BuilderExtensions.cs` with `Add[Module]Services()` method

#### 4. Create Application Layer
```bash
dotnet new classlib -n [Module].Application
dotnet add [Module].Application package FastEndpoints
dotnet add [Module].Application reference VerticalSlice.Shared
dotnet add [Module].Application reference [Module].Domain
dotnet add [Module].Application reference [Module].Infrastructure
```

- Create `Endpoints/[Feature]/` folders
- Each feature gets: `[Feature]Endpoint.cs`, `[Feature]Request.cs`, `[Feature]Response.cs`
- Create `Extensions/[Module]ServiceExtensions.cs` with `Add[Module]Endpoints()` method

#### 5. Create and Apply Migrations
```bash
dotnet ef migrations add InitialCreate --project [Module].Infrastructure --startup-project VerticalSlice.Api
dotnet ef database update --project [Module].Infrastructure --startup-project VerticalSlice.Api
```

#### 6. Register in Aspire and API

Add to `VerticalSlice.Aspire/Program.cs`:
```csharp
var moduleDb = builder
    .AddPostgres("[module]postgres")
    .AddDatabase("[module]db");
```

Add to `VerticalSlice.Api/Program.cs`:
```csharp
builder.Add[Module]Services();
builder.Services.Add[Module]Endpoints();
```

---

## Architecture Rationale

### Why This Structure?

#### ✅ **Pros**

1. **Independent Module Development**
    - Teams can work on different modules without conflicts
    - Each module has its own database, migrations, and endpoints
    - Reduced merge conflicts

2. **Clear Separation of Concerns**
    - Domain: Purely domain objects
    - Public: Stable contracts for inter-module communication
    - Infrastructure: All technical concerns
    - Application: Thin HTTP layer

3. **Testability**
    - Each layer can be tested independently
    - Services can be mocked via interfaces
    - Domain logic is pure and easy to test

4. **Scalability**
    - Modules can be extracted into microservices later
    - Database per module allows independent scaling
    - Public layer provides clear service boundaries

5. **Technology Flexibility**
    - Different modules can use different databases
    - Can swap FastEndpoints for MediatR or minimal APIs
    - Infrastructure is isolated from business logic

6. **Maintainability**
    - Feature changes are localized to one module
    - Clear ownership boundaries
    - Easy to onboard new developers (focus on one module)

7. **Aspire Integration**
    - Each module can get its own database via Aspire
    - Service discovery and telemetry built-in
    - Container orchestration handled automatically

#### ❌ **Cons**

1. **Increased Project Count**
    - 4+ projects per module can feel heavy for small features
    - More solution management overhead
    - Longer build times

2. **Boilerplate Code**
    - Each endpoint requires 3 files (Endpoint, Request, Response)
    - Repetitive service interfaces and implementations
    - Extension methods for each module can seem like an unneeded abstraction

3. **Cross-Module Communication Complexity**
    - Modules should communicate via Public contracts
    - Can be tempting to reference Infrastructure directly
    - Requires discipline or strict tests to maintain boundaries

4. **Learning Curve for Cross Domain Features**
    - Developers need to understand the layering
    - More structure to learn compared to simple CRUD
    - Requires understanding of DI, EF Core, FastEndpoints, Aspire

5. **Over-Engineering for Simple CRUD**
    - Small applications may not need this much structure
    - Can feel like overkill for prototypes
    - More files to navigate

6. **Database Per Module Overhead**
    - Might have multiple databases to manage
    - Foreign keys must effectively be managed with code, not DB constraints
    - Cross module query execution can be complex

---

## Alternative Architectures

One benefit of this architecture is its flexibility. Any module can adopt a different internal structure if needed. Below are some alternative architectures you might consider based on your project's needs.

### 1. **Strict Domain-Driven Design (DDD)**

```
[Module].Domain/
  ├── Entities/
  ├── ValueObjects/
  ├── Aggregates/
  ├── DomainEvents/
  ├── Repositories/  (interfaces only)
  └── Services/      (domain services)

[Module].Application/
  ├── Commands/
  ├── Queries/
  ├── DTOs/
  ├── Validators/
  └── Handlers/      (MediatR)

[Module].Infrastructure/
  ├── Persistence/
  ├── Repositories/  (implementations)
  └── ExternalServices/

[Module].Api/
  └── Controllers/   (or Endpoints)
```

**Key Differences:**
- Stronger focus on Aggregates and bounded contexts
- Application layer uses CQRS (Commands/Queries) with MediatR
- Domain layer has rich behavior and domain events
- Value objects enforce invariants
- Repository pattern with aggregate roots

**When to Use:**
- Complex business domains with rich behavior
- Need for domain events and eventual consistency
- Team experienced with DDD tactical patterns
- Long-term strategic projects

**Pros:**
- Rich domain model with encapsulated behavior
- Clear separation between commands and queries
- Domain events enable loose coupling
- Aggregate roots enforce consistency boundaries

**Cons:**
- Steeper learning curve
- More abstraction and indirection
- Can be over-engineered for CRUD scenarios
- Requires team buy-in on DDD concepts

---

### 2. **Simplified Vertical Slice (Combined Application/Public)**

```
[Module].Domain/
  └── Objects/

[Module].Application/  (combines Application + Public)
  ├── GetTodo/
  │   ├── Endpoint.cs
  │   ├── Query.cs
  │   └── Result.cs
  ├── CreateTodo/
  │   ├── Endpoint.cs
  │   ├── Command.cs
  │   └── Result.cs
  └── Shared/
      └── DTOs/

[Module].Infrastructure/
  ├── Data/
  └── Services/
```

**Key Differences:**
- Application and Public layers merged into just "Application"
- DTOs live alongside their endpoints
- Each feature is a complete slice (endpoint + logic + DTO)

**When to Use:**
- Small to medium applications
- No need for cross-module communication
- Teams want to move fast with less ceremony

**Pros:**
- Fewer projects to manage (3 instead of 4)
- Feature folders contain everything for that feature
- Less navigating between projects
- Faster to scaffold new features

**Cons:**
- No clear public contract layer
- Harder to extract features to separate services
- Cross-module dependencies less controlled
- Public API surface not well-defined
- Other modules will need to reference Application directly

---

### 3. **Data-Focused Architecture (Combined Infrastructure/Domain)**

```
[Module].Data/  (combines Infrastructure + Domain)
  ├── Objects/
  ├── Persistence/
  │   ├── Migrations/
  │   ├── ModuleDbContext.cs
  │   └── ModuleDbContextFactory.cs
  └── Services/

[Module].Application/
  └── Endpoints/

[Module].Public/
  └── Contracts/
```

**Key Differences:**
- Domain entities live with DbContext
- No separation between domain and persistence
- Services directly access DbContext

**When to Use:**
- Data-centric CRUD applications
- Anemic domain model (little business logic)
- Rapid prototyping or MVPs

**Pros:**
- Less project overhead (3 projects)
- Entities and DbContext co-located
- Simpler mental model
- Faster initial development

**Cons:**
- Domain logic couples to EF Core
- Hard to test domain without database
- Persistence concerns leak into domain
- Difficult to change ORM later

---

### 4. **Minimal Vertical Slice (2 Projects)**

```
[Module].Core/
  ├── Domain/
  ├── Features/
  │   ├── GetTodo/
  │   │   ├── Endpoint.cs
  │   │   ├── Handler.cs
  │   │   └── Validator.cs
  │   └── CreateTodo/
  └── Data/
      └── DbContext.cs

[Module].Api/
  └── Program.cs
```

**Key Differences:**
- Everything in one "Core" project
- Features contain endpoint + handler + validation
- Minimal layering

**When to Use:**
- Prototypes and MVPs
- Very small applications
- Solo developers or small teams
- Learning projects

**Pros:**
- Absolute minimum ceremony
- Fast to navigate
- Easy to understand
- Quick to scaffold

**Cons:**
- No separation of concerns
- Difficult to test in isolation
- Hard to scale as application grows
- Everything couples to everything

---

### 5. **Modular Monolith (Hexagonal/Ports & Adapters)**

```
[Module].Core/
  ├── Domain/
  ├── Ports/
  │   ├── ITodoRepository.cs
  │   ├── ITodoService.cs
  │   └── DTOs/
  └── UseCases/

[Module].Adapters/
  ├── Persistence/
  │   └── PostgresTodoRepository.cs
  ├── Api/
  │   └── TodoController.cs
  └── ExternalServices/

[Module].Tests/
  └── ...
```

**Key Differences:**
- Core defines "ports" (interfaces)
- Adapters implement ports (database, API, external services)
- Use cases orchestrate domain and ports
- Dependency inversion at module boundaries

**When to Use:**
- Need to support multiple adapters (REST + gRPC + GraphQL)
- Want to easily swap infrastructure (SQL → NoSQL)
- Team experienced with hexagonal architecture
- Testing is a top priority

**Pros:**
- Highly testable (mock all ports)
- Framework-independent core
- Easy to add new adapters
- Clear dependency flow (inward)

**Cons:**
- More abstraction layers
- Interface explosion
- Learning curve for team
- Can be overkill for CRUD

---

## Comparison Matrix

| Architecture | Projects per Module | Complexity | Flexibility | Boilerplate | Best For |
|-------------|---------------------|------------|-------------|-------------|----------|
| **Current (4-Layer)** | 4 | Medium-High | High | High | Modular monoliths, microservices prep |
| **Strict DDD** | 4 | High | Very High | Very High | Complex domains, large teams |
| **Simplified (3-Layer)** | 3 | Medium | Medium | Medium | Most applications, balanced approach |
| **Data-Focused** | 3 | Low-Medium | Low | Low | CRUD apps, anemic domains |
| **Minimal Slice** | 1-2 | Low | Low | Very Low | Prototypes, MVPs, small apps |
| **Hexagonal** | 2-3 | High | Very High | High | Multi-channel apps, testing focus |

---

## Recommendations

### ✅ **Stick with Current Architecture If:**
- Building a modular monolith with potential microservices migration
- Multiple teams working on different modules
- Need clear module boundaries and contracts
- Using Aspire for orchestration
- Plan to scale and evolve over time

### 🤔 **Consider Simplifying To 3-Layer If:**
- Cross-module communication is rare
- Public contracts add little value
- Team finds 4 projects excessive
- **Combine:** Application + Public → `[Module].Features`

### 🚀 **Consider Strict DDD If:**
- Complex business domain with rich behavior
- Need domain events and CQRS
- Team is experienced with DDD
- Long-term strategic project

### ⚡ **Consider Minimal Slice If:**
- Prototyping or MVP
- Solo developer or very small team
- Simple CRUD application
- Speed over structure

---

## Conclusion

The current **4-layer vertical slice architecture** provides a strong balance between modularity, testability, and scalability. It's well-suited for teams building modular monoliths with Aspire that may eventually evolve into microservices.

The key is **consistency** - once you choose an architecture, try to stick to that for all modules. Very rarely should we choose to change our architecture for some particular module.

**Start here, adjust as needed, but always be intentional about your structure.**

