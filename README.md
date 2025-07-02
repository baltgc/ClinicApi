# 🏥 Clinic Management API

Una API RESTful completa para gestión de clínicas médicas construida con **ASP.NET Core 9**, **Entity Framework Core**, y **Clean Architecture**.

## 📋 Tabla de Contenidos

- [Características](#-características)
- [Arquitectura](#-arquitectura)
- [Tecnologías](#-tecnologías)
- [Prerrequisitos](#-prerrequisitos)
- [Configuración Inicial](#-configuración-inicial)
- [Ejecutar el Proyecto](#-ejecutar-el-proyecto)
- [Base de Datos](#-base-de-datos)
- [Ejecutar Tests](#-ejecutar-tests)
- [Endpoints de API](#-endpoints-de-api)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Configuración](#-configuración)
- [Seguridad](#-seguridad)
- [Comandos Útiles](#-comandos-útiles)

## ✨ Características

- 🏗️ **Clean Architecture** con separación de capas
- 🛡️ **Autenticación JWT** y autorización basada en roles
- 📊 **Entity Framework Core** con SQL Server
- 🧪 **Testing** con NUnit y Moq
- 📝 **Swagger/OpenAPI** para documentación
- 🔄 **CQRS Pattern** con MediatR
- 🗂️ **AutoMapper** para mapeo de objetos
- 📦 **Dependency Injection** nativo de ASP.NET Core
- 🐳 **Docker** para SQL Server

## 🏗️ Arquitectura

El proyecto sigue los principios de **Clean Architecture**:

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

### Capas del Proyecto:

- **Domain**: Entidades, interfaces, reglas de negocio
- **Application**: Casos de uso, DTOs, comandos y consultas
- **Infrastructure**: Implementación de datos, servicios externos
- **Web**: Controllers, middleware, configuración de API
- **Tests**: Pruebas unitarias e integración

## 🚀 Tecnologías

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

## 📋 Prerrequisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### Herramientas CLI requeridas:

```powershell
# Instalar Entity Framework Core CLI
dotnet tool install --global dotnet-ef
```

## ⚙️ Configuración Inicial

### 1. Clonar el repositorio

```powershell
git clone <repository-url>
cd ClinicApi
```

### 2. Iniciar SQL Server con Docker

```powershell
# Iniciar SQL Server en Docker
docker-compose up -d

# Verificar que el contenedor esté ejecutándose
docker ps
```

### 3. Restaurar dependencias

```powershell
dotnet restore
```

### 4. Configurar la base de datos

```powershell
# Crear migración inicial
dotnet ef migrations add InitialCreate --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj

# Aplicar migraciones
dotnet ef database update --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj
```

## 🚀 Ejecutar el Proyecto

### Desarrollo

```powershell
# Ejecutar la API
dotnet run --project Web/ClinicApi.Web.csproj

# O desde el directorio Web
cd Web
dotnet run
```

### Producción

```powershell
# Compilar en modo Release
dotnet build --configuration Release

# Ejecutar en modo Production
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run --project Web/ClinicApi.Web.csproj --configuration Release
```

### 🌐 URLs de la aplicación:

- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger UI**: https://localhost:5001/swagger

## 🗄️ Base de Datos

### Configuración

La aplicación utiliza **SQL Server** ejecutándose en Docker:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ClinicApiDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

### Comandos de migración

```powershell
# Crear nueva migración
dotnet ef migrations add <NombreMigracion> --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj

# Aplicar migraciones
dotnet ef database update --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj

# Revertir migración
dotnet ef database update <MigracionAnterior> --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj

# Eliminar última migración (si no se aplicó)
dotnet ef migrations remove --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj

# Ver historial de migraciones
dotnet ef migrations list --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj
```

### Gestión de Docker SQL Server

```powershell
# Iniciar contenedor
docker-compose up -d

# Detener contenedor
docker-compose down

# Ver logs del contenedor
docker-compose logs sqlserver

# Conectar directamente a SQL Server
sqlcmd -S localhost,1433 -U sa -P YourStrong@Passw0rd
```

## 🧪 Ejecutar Tests

```powershell
# Ejecutar todas las pruebas
dotnet test

# Ejecutar con información detallada
dotnet test --verbosity normal

# Ejecutar pruebas específicas
dotnet test --filter "FullyQualifiedName~CreatePatient"

# Ejecutar por categoría
dotnet test --filter "Category=Unit"

# Generar reporte de cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Tipos de Tests:

- **Unit Tests**: Lógica de negocio y handlers
- **Integration Tests**: Repositorios con base de datos
- **API Tests**: Endpoints completos

## 📡 Endpoints de API

### Autenticación

```http
POST /api/auth/register     # Registrar usuario
POST /api/auth/login        # Iniciar sesión
GET  /api/auth/profile      # Obtener perfil (requiere auth)
```

### Pacientes

```http
GET    /api/patients              # Obtener todos los pacientes
GET    /api/patients/{id}         # Obtener paciente por ID
GET    /api/patients/active       # Obtener pacientes activos
GET    /api/patients/search?term= # Buscar pacientes
POST   /api/patients              # Crear paciente
PUT    /api/patients/{id}         # Actualizar paciente
DELETE /api/patients/{id}         # Eliminar paciente
PATCH  /api/patients/{id}/deactivate # Desactivar paciente
```

### Citas

```http
GET    /api/appointments                    # Obtener todas las citas
GET    /api/appointments/{id}               # Obtener cita por ID
GET    /api/appointments/doctor/{doctorId}  # Citas por doctor
GET    /api/appointments/patient/{patientId} # Citas por paciente
GET    /api/appointments/availability       # Verificar disponibilidad
POST   /api/appointments                    # Crear cita
PUT    /api/appointments/{id}               # Actualizar cita
DELETE /api/appointments/{id}               # Eliminar cita
PATCH  /api/appointments/{id}/cancel        # Cancelar cita
```

### Autenticación y Autorización

La API utiliza **JWT Bearer tokens** con los siguientes roles:

- `Admin`: Acceso completo
- `Doctor`: Gestión de citas y pacientes
- `Nurse`: Acceso limitado a pacientes
- `Receptionist`: Gestión de citas
- `Manager`: Reportes y gestión
- `Patient`: Acceso a sus propios datos

## 📁 Estructura del Proyecto

```
ClinicApi/
├── 📁 Domain/                          # 🎯 Capa de Dominio
│   ├── 📁 Entities/                    # Entidades del dominio
│   │   ├── ApplicationUser.cs
│   │   ├── Patient.cs
│   │   ├── Doctor.cs
│   │   ├── Appointment.cs
│   │   └── ...
│   ├── 📁 Enums/                       # Enumeraciones
│   ├── 📁 Exceptions/                  # Excepciones del dominio
│   ├── 📁 Interfaces/                  # Contratos de repositorios
│   └── 📁 Services/                    # Servicios del dominio
│
├── 📁 Application/                     # 🎯 Capa de Aplicación
│   ├── 📁 Commands/                    # Comandos CQRS
│   │   ├── 📁 Patients/
│   │   ├── 📁 Appointments/
│   │   └── 📁 Auth/
│   ├── 📁 Queries/                     # Consultas CQRS
│   ├── 📁 Handlers/                    # Manejadores CQRS
│   ├── 📁 DTOs/                        # Objetos de transferencia
│   ├── 📁 Validators/                  # Validadores FluentValidation
│   └── 📁 Mapping/                     # Perfiles AutoMapper
│
├── 📁 Infrastructure/                  # 🎯 Capa de Infraestructura
│   ├── 📁 Data/
│   │   ├── 📁 Context/                 # DbContext
│   │   ├── 📁 Repositories/            # Implementaciones de repositorios
│   │   └── 📁 Migrations/              # Migraciones EF Core
│   ├── 📁 Services/                    # Servicios externos
│   └── 📁 Configuration/               # Configuración DI
│
├── 📁 Web/                             # 🎯 Capa de Presentación
│   ├── 📁 Controllers/                 # Controladores API
│   ├── 📁 Middleware/                  # Middleware personalizado
│   ├── 📁 Configuration/               # Configuración web
│   ├── Program.cs                      # Punto de entrada
│   └── appsettings.json               # Configuración
│
├── 📁 ClinicApi.Tests/                 # 🎯 Pruebas
│   ├── 📁 Unit/                        # Pruebas unitarias
│   │   ├── 📁 Application/
│   │   ├── 📁 Domain/
│   │   └── 📁 Infrastructure/
│   └── 📁 Integration/                 # Pruebas de integración
│
├── docker-compose.yml                  # 🐳 Configuración Docker
├── ClinicApi.sln                      # 📋 Archivo de solución
└── README.md                          # 📖 Documentación
```

## ⚙️ Configuración

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
  ]
}
```

### Variables de Entorno

```powershell
# Configurar entorno de desarrollo
$env:ASPNETCORE_ENVIRONMENT="Development"

# Configurar entorno de producción
$env:ASPNETCORE_ENVIRONMENT="Production"

# Configurar cadena de conexión personalizada
$env:ConnectionStrings__DefaultConnection="Server=...;Database=...;"
```

## 🔒 Seguridad

### ⚠️ **Archivos Sensibles**

**NUNCA** subas al repositorio estos archivos:
- `appsettings.json` (contiene JWT secret y passwords)
- `docker-compose.yml` (contiene SA password)
- `.env` (variables de entorno con secrets)

### 📋 **Setup Seguro**

1. **Usa archivos template:**
   ```powershell
   copy appsettings.Template.json Web/appsettings.json
   copy docker-compose.Template.yml docker-compose.yml
   ```

2. **Configura tus propios valores:**
   - Genera JWT secret seguro
   - Usa passwords únicos para desarrollo
   - Nunca uses credenciales de producción localmente

3. **Para producción usa:**
   - Azure Key Vault
   - User Secrets (.NET)
   - Variables de entorno seguras

### 📖 **Guía Completa**

Ver [SECURITY-SETUP.md](SECURITY-SETUP.md) para instrucciones detalladas de configuración segura.

## 🛠️ Comandos Útiles

### Desarrollo diario

```powershell
# Verificar estado de Docker
docker ps

# Ejecutar aplicación
dotnet run --project Web/ClinicApi.Web.csproj

# Ejecutar tests
dotnet test

# Limpiar y reconstruir
dotnet clean
dotnet build
```

### Base de datos

```powershell
# Crear migración
dotnet ef migrations add <Nombre> --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj

# Aplicar migración
dotnet ef database update --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj

# Generar script SQL
dotnet ef migrations script --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj
```

### Docker

```powershell
# Iniciar todos los servicios
docker-compose up -d

# Detener todos los servicios
docker-compose down

# Ver logs
docker-compose logs -f

# Reconstruir y iniciar
docker-compose up -d --build
```

## 🐛 Solución de Problemas

### Error de conexión a SQL Server

```powershell
# Verificar que Docker esté ejecutándose
docker ps

# Reiniciar contenedor SQL Server
docker-compose restart sqlserver

# Verificar conexión
sqlcmd -S localhost,1433 -U sa -P YourStrong@Passw0rd -Q "SELECT @@VERSION"
```

### Error de migración

```powershell
# Eliminar base de datos y recrear
dotnet ef database drop --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj
dotnet ef database update --project Infrastructure/ClinicApi.Infrastructure.csproj --startup-project Web/ClinicApi.Web.csproj
```

### Problemas de certificado HTTPS

```powershell
# Instalar certificado de desarrollo
dotnet dev-certs https --trust
```

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📞 Soporte

Para soporte o preguntas, por favor abre un issue en el repositorio de GitHub.

---

**¡Happy Coding!** 🚀
