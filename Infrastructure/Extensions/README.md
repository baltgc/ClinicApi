# Infrastructure Extensions

This folder contains extension methods that organize and modularize the application configuration, making `Program.cs` cleaner and more maintainable.

## Files

### ServiceCollectionExtensions.cs

Contains extension methods for configuring services in the DI container:

- **`AddDatabaseConfiguration()`** - Configures Entity Framework and database context
- **`AddIdentityConfiguration()`** - Configures ASP.NET Core Identity
- **`AddJwtAuthentication()`** - Configures JWT authentication
- **`AddAuthorizationPolicies()`** - Configures authorization policies
- **`AddRepositories()`** - Registers repositories
- **`AddApplicationServices()`** - Registers application and domain services
- **`AddAutoMapperConfiguration()`** - Configures AutoMapper
- **`AddSwaggerConfiguration()`** - Configures Swagger/OpenAPI documentation
- **`AddCorsConfiguration()`** - Configures CORS policies
- **`AddHealthCheckConfiguration()`** - Configures health checks
- **`AddModelValidationConfiguration()`** - Configures model validation
- **`AddClinicServices()`** - Master method that configures all services at once

### WebApplicationExtensions.cs

Contains extension methods for configuring the middleware pipeline:

- **`ConfigureDevelopmentPipeline()`** - Configures development-specific middleware
- **`AddSecurityHeaders()`** - Adds security headers middleware
- **`ConfigureMiddlewarePipeline()`** - Configures the main middleware pipeline
- **`ConfigureEndpoints()`** - Maps controllers and API endpoints
- **`InitializeDatabaseAsync()`** - Initializes database and seeds data

## Usage

### In Program.cs (Clean version)

```csharp
using ClinicApi.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure all services using extension methods
builder.Services.AddClinicServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline using extension methods
app.ConfigureDevelopmentPipeline()
   .AddSecurityHeaders()
   .ConfigureMiddlewarePipeline()
   .ConfigureEndpoints();

// Initialize database in development
await app.InitializeDatabaseAsync();

app.Run();
```

### Individual Configuration (For granular control)

```csharp
// You can also use individual methods for specific configurations
builder.Services
    .AddControllers()
    .AddDatabaseConfiguration(builder.Configuration)
    .AddIdentityConfiguration()
    .AddJwtAuthentication(builder.Configuration)
    .AddAuthorizationPolicies()
    .AddRepositories()
    .AddApplicationServices();
```

## Benefits

1. **Separation of Concerns** - Each configuration is in its own method
2. **Maintainability** - Easy to modify individual configurations
3. **Testability** - Each configuration can be tested independently
4. **Readability** - Clean and organized Program.cs
5. **Reusability** - Extension methods can be reused across projects
6. **Modularity** - Easy to add/remove features

## Configuration Dependencies

Some extensions depend on configuration sections in `appsettings.json`:

- **JWT Authentication**: Requires `JwtSettings` section
- **CORS**: Requires `Cors:AllowedOrigins` section
- **Swagger**: Uses optional `SwaggerConfig` section
- **Database**: Requires `ConnectionStrings:DefaultConnection`

## Future Improvements

Consider adding these additional extension methods:

- `AddLoggingConfiguration()` - For Serilog/logging setup
- `AddCachingConfiguration()` - For Redis/memory caching
- `AddApiVersioningConfiguration()` - For API versioning
- `AddFluentValidationConfiguration()` - For FluentValidation setup
- `AddBackgroundServicesConfiguration()` - For hosted services
