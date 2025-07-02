# 🏥 Clinic Management API

A comprehensive RESTful API for medical clinic management built with **ASP.NET Core 9**, **Entity Framework Core**, and **Clean Architecture**.

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Technologies](#-technologies)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [Running the Project](#-running-the-project)
- [Database](#-database)
- [Testing](#-testing)
- [API Endpoints](#-api-endpoints)
- [Project Structure](#-project-structure)
- [Configuration](#-configuration)
- [Security](#-security)
- [Troubleshooting](#-troubleshooting)
- [Useful Commands](#-useful-commands)

## ✨ Features

- 🏗️ **Clean Architecture** with clear layer separation
- 🛡️ **JWT Authentication** and role-based authorization
- 📊 **Entity Framework Core** with SQL Server
- 🧪 **Unit Testing** with NUnit and Moq
- 📝 **Swagger/OpenAPI** documentation
- 🔄 **CQRS Pattern** with MediatR
- 🗂️ **AutoMapper** for object mapping
- 📦 **Dependency Injection** with ASP.NET Core DI
- 🐳 **Docker** support for SQL Server
- 🔐 **HTTPS/SSL** configuration for development and production
- 🌐 **CORS** configuration for cross-origin requests

## 🏗️ Architecture

The project follows **Clean Architecture** principles:

```
┌─────────────────┐    ┌─────────────────┐
│   Presentation  │    │   Application   │
│   (Web API)     │───▶│   (Use Cases)   │
└─────────────────┘    └─────────────────┘
         │                       │
         ▼                       ▼
┌─────────────────┐    ┌─────────────────┐
│ Infrastructure  │    │     Domain      │
│ (Data & Services)│───▶│  (Entities)     │
└─────────────────┘    └─────────────────┘
```

### Project Layers:

- **Domain**: Entities, interfaces, business rules
- **Application**: Use cases, DTOs, commands and queries
- **Infrastructure**: Data implementation, external services
- **Web**: Controllers, middleware, API configuration
- **Tests**: Unit and integration tests

## 🚀 Technologies

- **.NET 9.0**
- **ASP.NET Core 9.0**
- **Entity Framework Core 9.0**
- **SQL Server** (via Docker)
- **JWT Authentication**
- **Swagger/OpenAPI**
- **AutoMapper**
- **FluentValidation**
- **NUnit** + **Moq** (Testing)
- **Docker & Docker Compose**

## 📋 Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### Required CLI Tools:

```powershell
# Install Entity Framework Core CLI
dotnet tool install --global dotnet-ef
```

## ⚙️ Getting Started

### 1. Clone the repository

```powershell
git clone <repository-url>
cd ClinicApi
```

### 2. Start SQL Server with Docker

```powershell
# Start SQL Server in Docker
docker-compose up -d

# Verify container is running
docker ps
```

### 3. Restore dependencies

```powershell
dotnet restore
```

### 4. Configure the database

```powershell
# Create initial migration (if not exists)
dotnet ef migrations add InitialCreate --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj

# Apply migrations
dotnet ef database update --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj
```

## 🚀 Running the Project

### Development

```powershell
# Run the API
dotnet run --project Web/ClinicApi.Web.csproj

# Or from the Web directory
cd Web
dotnet run
```

### Launch Profiles

The project includes multiple launch profiles:

```powershell
# Run with HTTP only (port 5000)
dotnet run --project Web/ClinicApi.Web.csproj --launch-profile http

# Run with HTTPS support (ports 5000 and 5001)
dotnet run --project Web/ClinicApi.Web.csproj --launch-profile https
```

### Production

```powershell
# Build in Release mode
dotnet build --configuration Release

# Run in Production mode
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run --project Web/ClinicApi.Web.csproj --configuration Release
```

### 🌐 Application URLs:

- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger UI**: https://localhost:5001/swagger (or http://localhost:5000/swagger)

## 🗄️ Database

### Configuration

The application uses **SQL Server** running in Docker:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ClinicApiDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

### Migration Commands

```powershell
# Create new migration
dotnet ef migrations add <MigrationName> --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj

# Apply migrations
dotnet ef database update --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj

# Rollback migration
dotnet ef database update <PreviousMigration> --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj

# Remove last migration (if not applied)
dotnet ef migrations remove --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj

# List migrations
dotnet ef migrations list --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj
```

### Docker SQL Server Management

```powershell
# Start container
docker-compose up -d

# Stop container
docker-compose down

# View container logs
docker-compose logs sqlserver

# Connect directly to SQL Server
sqlcmd -S localhost,1433 -U sa -P YourStrong@Passw0rd
```

## 🧪 Testing

```powershell
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run specific tests
dotnet test --filter "FullyQualifiedName~CreatePatient"

# Run by category
dotnet test --filter "Category=Unit"

# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"
```

### Test Types:

- **Unit Tests**: Business logic and handlers
- **Integration Tests**: Repositories with database
- **API Tests**: Complete endpoints

## 📡 API Endpoints

### Authentication

```http
POST /api/auth/register     # Register user
POST /api/auth/login        # Login
GET  /api/auth/profile      # Get profile (requires auth)
```

### Patients

```http
GET    /api/patients              # Get all patients
GET    /api/patients/{id}         # Get patient by ID
GET    /api/patients/active       # Get active patients
GET    /api/patients/search?term= # Search patients
POST   /api/patients              # Create patient
PUT    /api/patients/{id}         # Update patient
DELETE /api/patients/{id}         # Delete patient
PATCH  /api/patients/{id}/deactivate # Deactivate patient
```

### Appointments

```http
GET    /api/appointments                    # Get all appointments
GET    /api/appointments/{id}               # Get appointment by ID
GET    /api/appointments/doctor/{doctorId}  # Appointments by doctor
GET    /api/appointments/patient/{patientId} # Appointments by patient
GET    /api/appointments/availability       # Check availability
POST   /api/appointments                    # Create appointment
PUT    /api/appointments/{id}               # Update appointment
DELETE /api/appointments/{id}               # Delete appointment
PATCH  /api/appointments/{id}/cancel        # Cancel appointment
```

### Authentication and Authorization

The API uses **JWT Bearer tokens** with the following roles:

- `Admin`: Full access
- `Doctor`: Appointment and patient management
- `Nurse`: Limited patient access
- `Receptionist`: Appointment management
- `Manager`: Reports and management
- `Patient`: Access to own data

## 📁 Project Structure

```
ClinicApi/
├── 📁 Domain/                          # 🎯 Domain Layer
│   ├── 📁 Entities/                    # Domain entities
│   │   ├── ApplicationUser.cs
│   │   ├── Patient.cs
│   │   ├── Doctor.cs
│   │   ├── Appointment.cs
│   │   └── ...
│   ├── 📁 Enums/                       # Enumerations
│   ├── 📁 Exceptions/                  # Domain exceptions
│   ├── 📁 Interfaces/                  # Repository contracts
│   └── 📁 Services/                    # Domain services
│
├── 📁 Application/                     # 🎯 Application Layer
│   ├── 📁 Commands/                    # CQRS Commands
│   │   ├── 📁 Patients/
│   │   ├── 📁 Appointments/
│   │   └── 📁 Auth/
│   ├── 📁 Queries/                     # CQRS Queries
│   ├── 📁 Handlers/                    # CQRS Handlers
│   ├── 📁 DTOs/                        # Data Transfer Objects
│   ├── 📁 Validators/                  # FluentValidation validators
│   └── 📁 Mapping/                     # AutoMapper profiles
│
├── 📁 Infrastructure/                  # 🎯 Infrastructure Layer
│   ├── 📁 Data/
│   │   ├── 📁 Context/                 # DbContext
│   │   ├── 📁 Repositories/            # Repository implementations
│   │   └── 📁 Migrations/              # EF Core migrations
│   ├── 📁 Services/                    # External services
│   └── 📁 Configuration/               # DI configuration
│
├── 📁 Web/                             # 🎯 Presentation Layer
│   ├── 📁 Controllers/                 # API controllers
│   ├── 📁 Middleware/                  # Custom middleware
│   ├── 📁 Configuration/               # Web configuration
│   ├── 📁 Properties/                  # Launch settings
│   ├── Program.cs                      # Entry point
│   └── appsettings.json               # Configuration
│
├── 📁 ClinicApi.Tests/                 # 🎯 Tests
│   ├── 📁 Unit/                        # Unit tests
│   │   ├── 📁 Application/
│   │   ├── 📁 Domain/
│   │   └── 📁 Infrastructure/
│   └── 📁 Integration/                 # Integration tests
│
├── docker-compose.yml                  # 🐳 Docker configuration
├── ClinicApi.sln                      # 📋 Solution file
└── README.md                          # 📖 Documentation
```

## ⚙️ Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ClinicApiDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "SecretKey": "MyVerySecretKeyForJwtTokenGeneration123456789!",
    "Issuer": "ClinicApi",
    "Audience": "ClinicApiUsers",
    "ExpirationInHours": 24
  },
  "SwaggerConfig": {
    "Title": "Clinic Management API",
    "Version": "v1"
  },
  "AllowedOrigins": [
    "http://localhost:3000",
    "https://localhost:3001"
  ],
  "HttpsPort": 5001
}
```

### Environment Variables

```powershell
# Set development environment
$env:ASPNETCORE_ENVIRONMENT="Development"

# Set production environment
$env:ASPNETCORE_ENVIRONMENT="Production"

# Set custom connection string
$env:ConnectionStrings__DefaultConnection="Server=...;Database=...;"
```

### HTTPS Configuration

The project includes proper HTTPS configuration:

- **Development**: HTTPS redirection is disabled to prevent port conflicts
- **Production**: HTTPS redirection is enabled for security
- **Launch Settings**: Both HTTP (5000) and HTTPS (5001) profiles available

## 🔒 Security

### ⚠️ **Sensitive Files**

**NEVER** commit these files to the repository:
- `appsettings.json` (contains JWT secret and passwords)
- `docker-compose.yml` (contains SA password)
- `.env` (environment variables with secrets)

### 📋 **Secure Setup**

1. **Use template files:**
   ```powershell
   copy appsettings.example.json Web/appsettings.json
   copy docker-compose.example.yml docker-compose.yml
   ```

2. **Configure your own values:**
   - Generate secure JWT secret
   - Use unique passwords for development
   - Never use production credentials locally

3. **For production use:**
   - Azure Key Vault
   - User Secrets (.NET)
   - Secure environment variables

## 🐛 Troubleshooting

### Common Issues

#### HTTPS Redirection Error

If you encounter `Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware[3]` errors:

1. **Check your launch profile**: Use the `http` profile for HTTP-only development
2. **Verify ports**: Ensure ports 5000 and 5001 are available
3. **Development mode**: HTTPS redirection is disabled in development by default

#### SQL Server Connection Issues

```powershell
# Check if Docker is running
docker ps

# Restart SQL Server container
docker-compose restart sqlserver

# Check container logs
docker-compose logs sqlserver
```

#### Port Already in Use

```powershell
# Find process using port 5000
netstat -ano | findstr :5000

# Kill process (replace PID with actual process ID)
taskkill /PID <PID> /F
```

#### Migration Issues

```powershell
# Reset database (WARNING: This will delete all data)
dotnet ef database drop --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj
dotnet ef database update --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj
```

## 🛠️ Useful Commands

### Daily Development

```powershell
# Check Docker status
docker ps

# Run application
dotnet run --project Web/ClinicApi.Web.csproj

# Run tests
dotnet test

# Clean and rebuild
dotnet clean
dotnet build
```

### Database Management

```powershell
# Create migration
dotnet ef migrations add <Name> --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj

# Apply migration
dotnet ef database update --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj

# Generate SQL script
dotnet ef migrations script --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj
```

### Docker Management

```powershell
# Start all services
docker-compose up -d

# Stop all services
docker-compose down

# View logs
docker-compose logs -f

# Rebuild and start
docker-compose up -d --build
```

## 📊 Health Checks

The API includes health checks accessible at:

- **Health Check Endpoint**: `/health`
- **Detailed Health**: Available in development mode

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For support and questions:
- Create an issue in the repository
- Check the troubleshooting section
- Review the API documentation at `/swagger`

---

**Built with ❤️ using ASP.NET Core 9 and Clean Architecture principles**
