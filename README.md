# Clinic Management API

A comprehensive RESTful API for managing clinic operations including patients, doctors, appointments, medical records, and prescriptions, built with **Clean Architecture** and **CQRS** patterns.

## 🏗️ Clean Architecture Overview

This API follows **Robert C. Martin's Clean Architecture** principles with clear separation of concerns, dependency inversion, and the **CQRS (Command Query Responsibility Segregation)** pattern using **MediatR**.

### 📁 Project Structure

```
ClinicApi/
├── Domain/                            # 🧠 Core Business Logic (Inner Circle)
│   ├── Entities/                      # Domain Entities
│   │   ├── ApplicationUser.cs         # Identity User Extension
│   │   ├── Patient.cs                 # Patient Entity with Business Logic
│   │   ├── Doctor.cs                  # Doctor Entity with Business Logic
│   │   ├── Appointment.cs             # Appointment Entity with Business Logic
│   │   ├── MedicalRecord.cs           # Medical Record Entity
│   │   ├── Prescription.cs            # Prescription Entity
│   │   └── DoctorSchedule.cs          # Doctor Schedule Entity
│   ├── Common/                        # Base Classes
│   │   └── BaseEntity.cs              # Base Entity with Audit Properties
│   ├── Enums/                         # Domain Enumerations
│   │   ├── AppointmentStatus.cs       # Appointment Status Enum
│   │   ├── PrescriptionStatus.cs      # Prescription Status Enum
│   │   └── ClinicRoles.cs             # User Roles Constants
│   ├── Interfaces/                    # Repository Contracts (Dependency Inversion)
│   │   ├── IGenericRepository.cs      # Generic Repository Interface
│   │   ├── IPatientRepository.cs      # Patient Repository Interface
│   │   ├── IDoctorRepository.cs       # Doctor Repository Interface
│   │   └── IAppointmentRepository.cs  # Appointment Repository Interface
│   ├── Services/                      # 🎯 Complex Business Rules
│   │   ├── IAppointmentDomainService.cs     # Appointment Business Rules Interface
│   │   ├── AppointmentDomainService.cs      # Intelligent Scheduling & Pricing
│   │   ├── IPatientDomainService.cs         # Patient Business Rules Interface
│   │   └── PatientDomainService.cs          # Risk Assessment & Eligibility
│   └── Exceptions/                    # Domain-Specific Exceptions
│       ├── DomainException.cs         # Base Domain Exception
│       ├── AppointmentConflictException.cs  # Appointment Conflicts
│       └── InvalidAppointmentStatusException.cs # Status Validation
├── Application/                       # 📋 Application Business Rules (CQRS)
│   ├── DTOs/                          # Data Transfer Objects
│   │   ├── AppointmentDto.cs          # Appointment Data Transfer Objects
│   │   ├── PatientDto.cs              # Patient Data Transfer Objects
│   │   ├── DoctorDto.cs               # Doctor Data Transfer Objects
│   │   └── AuthDto.cs                 # Authentication Data Transfer Objects
│   ├── Commands/                      # CQRS Commands (Write Operations)
│   │   ├── Appointments/              # Appointment Commands
│   │   │   ├── CreateAppointmentCommand.cs
│   │   │   ├── UpdateAppointmentCommand.cs
│   │   │   ├── CancelAppointmentCommand.cs
│   │   │   └── DeleteAppointmentCommand.cs
│   │   └── Patients/                  # Patient Commands
│   │       ├── CreatePatientCommand.cs
│   │       └── UpdatePatientCommand.cs
│   ├── Queries/                       # CQRS Queries (Read Operations)
│   │   ├── Appointments/              # Appointment Queries
│   │   │   ├── GetAppointmentByIdQuery.cs
│   │   │   ├── GetAppointmentsByDoctorQuery.cs
│   │   │   └── GetAppointmentsByPatientQuery.cs
│   │   └── Patients/                  # Patient Queries
│   │       └── GetPatientByIdQuery.cs
│   ├── Handlers/                      # Command/Query Handlers (TODO)
│   ├── Interfaces/                    # Application Service Contracts
│   │   ├── IAppointmentService.cs     # Appointment Service Interface
│   │   ├── IPatientService.cs         # Patient Service Interface
│   │   └── IAuthService.cs            # Authentication Service Interface
│   ├── Mapping/                       # AutoMapper Profiles
│   │   └── MappingProfile.cs          # Object-to-Object Mapping
│   ├── Validators/                    # Input Validation Rules (FluentValidation)
│   ├── Services/                      # Application Services (TODO)
│   └── Common/                        # Common Application Utilities
├── Infrastructure/                    # 🔧 External Concerns & Data Access
│   ├── Data/                          # Data Access Layer
│   │   ├── Context/                   # Entity Framework DbContext
│   │   │   └── ClinicDbContext.cs     # Main Database Context
│   │   ├── Repositories/              # Repository Implementations
│   │   │   ├── GenericRepository.cs   # Generic Repository Implementation
│   │   │   ├── PatientRepository.cs   # Patient Repository Implementation
│   │   │   └── DoctorRepository.cs    # Doctor Repository Implementation
│   │   └── Migrations/                # Database Migrations (Auto-generated)
│   ├── Configuration/                 # Dependency Injection & Setup
│   │   └── ServiceCollectionExtensions.cs  # Service Registration
│   └── Services/                      # External Service Implementations
├── Web/                               # 🌐 Presentation Layer (API)
│   ├── Controllers/                   # RESTful API Endpoints
│   │   ├── AppointmentsController.cs  # Appointment API Endpoints
│   │   ├── PatientsController.cs      # Patient API Endpoints
│   │   └── AuthController.cs          # Authentication API (TODO: CQRS)
│   ├── Configuration/                 # Web Configuration
│   │   └── WebApplicationExtensions.cs # Pipeline Configuration
│   ├── Middleware/                    # Cross-cutting Concerns
│   ├── Program.cs                     # 🚀 Application Entry Point
│   ├── appsettings.json               # Production Configuration
│   └── appsettings.Development.json   # Development Configuration
└── ClinicApi.sln                      # Solution File
```

### 🎯 Clean Architecture Benefits

- **🔄 Dependency Inversion**: Infrastructure depends on Domain, not vice versa
- **📋 CQRS Pattern**: Clear separation between read and write operations
- **🧪 Testability**: Business logic isolated and easily unit testable
- **📦 Separation of Concerns**: Each layer has a single responsibility
- **🔧 Maintainability**: Changes in one layer don't affect others
- **📈 Scalability**: Easy to add new features following established patterns
- **🏗️ Enterprise Ready**: Follows industry-standard architectural patterns
- **⚡ Performance**: Optimized queries and commands for specific use cases

### 🎯 CQRS Implementation

- **Commands**: Handle write operations (Create, Update, Delete)
- **Queries**: Handle read operations with optimized data transfer objects
- **MediatR**: Mediator pattern for decoupling controllers from business logic
- **Handlers**: Process commands and queries with single responsibility

### 🔧 Technology Stack

- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core** - Web API framework  
- **Entity Framework Core 9.0** - ORM for database operations
- **SQL Server** - Database (LocalDB for development)
- **MediatR** - CQRS implementation and mediator pattern
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Input validation framework
- **Swagger/OpenAPI** - API documentation
- **ASP.NET Core Identity** - Authentication and authorization
- **JWT Bearer** - Token-based authentication
- **Serilog** - Structured logging

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server (LocalDB included with Visual Studio)
- Visual Studio 2022 or VS Code

### Installation & Setup

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd ClinicApi
   ```

2. **Restore NuGet packages**

   ```bash
   dotnet restore
   ```

3. **Update database connection** (optional)

   - Edit `Web/appsettings.json` or `Web/appsettings.Development.json`
   - Modify the `DefaultConnection` string if needed

4. **Build the solution**

   ```bash
   dotnet build
   ```

5. **Run the application**

   ```bash
   dotnet run --project Web
   ```

6. **Access the API**
   - API Documentation: `https://localhost:5001/swagger`
   - Health Check: `https://localhost:5001/health`

## 📚 API Endpoints

### 🔐 Authentication (`/api/auth`) - *TODO: CQRS Implementation*

- `POST /api/auth/login` - User login (TODO: Implement with MediatR Command)
- `POST /api/auth/register` - Register new user (TODO: Implement with MediatR Command)
- `GET /api/auth/profile` - Get user profile (TODO: Implement with MediatR Query)

### 👥 Patients (`/api/patients`)

- `GET /api/patients` - Get all patients
- `GET /api/patients/{id}` - Get patient by ID
- `GET /api/patients/search?searchTerm=` - Search patients
- `GET /api/patients/active` - Get active patients
- `GET /api/patients/{id}/appointments` - Get patient with appointments
- `GET /api/patients/{id}/medical-records` - Get patient with medical records
- `GET /api/patients/{id}/risk-assessment` - **🆕 Get patient risk profile**
- `POST /api/patients` - Create new patient
- `PUT /api/patients/{id}` - Update patient
- `DELETE /api/patients/{id}` - Delete patient
- `PATCH /api/patients/{id}/deactivate` - Deactivate patient

### 📅 Appointments (`/api/appointments`)

- `GET /api/appointments` - Get all appointments
- `GET /api/appointments/{id}` - Get appointment by ID
- `GET /api/appointments/patient/{patientId}` - Get appointments by patient
- `GET /api/appointments/doctor/{doctorId}` - Get appointments by doctor
- `GET /api/appointments/date-range?startDate=&endDate=` - Get appointments by date range
- `GET /api/appointments/status/{status}` - Get appointments by status
- `GET /api/appointments/upcoming?days=7` - Get upcoming appointments
- `GET /api/appointments/{id}/details` - Get appointment with full details
- `GET /api/appointments/check-availability` - Check doctor availability
- `GET /api/appointments/{doctorId}/next-available` - **🆕 Find next available slot**
- `GET /api/appointments/{id}/consultation-fee` - **🆕 Calculate dynamic pricing**
- `POST /api/appointments` - Create new appointment
- `PUT /api/appointments/{id}` - Update appointment
- `PATCH /api/appointments/{id}/cancel` - Cancel appointment (with policy validation)
- `DELETE /api/appointments/{id}` - Delete appointment

## 💾 Database Schema

### Core Entities

- **Patient** - Patient information and demographics with business logic
- **Doctor** - Doctor profiles and specializations with business logic
- **Appointment** - Appointment scheduling with business logic
- **MedicalRecord** - Patient visit records and diagnoses
- **Prescription** - Medication prescriptions
- **DoctorSchedule** - Doctor availability schedules
- **ApplicationUser** - Extended Identity user with clinic-specific properties

### Key Relationships

- Patient → Appointments (One-to-Many)
- Doctor → Appointments (One-to-Many)
- Patient → MedicalRecords (One-to-Many)
- Doctor → MedicalRecords (One-to-Many)
- MedicalRecord → Prescriptions (One-to-Many)
- Appointment → MedicalRecords (One-to-Many)
- ApplicationUser → Patient (One-to-One, Optional)
- ApplicationUser → Doctor (One-to-One, Optional)

## 🎯 Key Features

### ✅ Core Implementation

- **Clean Architecture** with proper dependency inversion
- **CQRS Pattern** with MediatR (Commands/Queries structure in place)
- **RESTful API Design** with proper HTTP methods and status codes
- **Repository Pattern** with domain-owned interfaces (Dependency Inversion)
- **Dependency Injection** for loose coupling and testability
- **AutoMapper** for object mapping
- **Swagger Documentation** with comprehensive XML comments
- **Health Checks** for monitoring and diagnostics
- **CORS Configuration** for cross-origin requests
- **Model Validation** with FluentValidation (structure ready)
- **Entity Framework Core 9.0** with proper relationships and migrations
- **Automatic Timestamps** (CreatedAt, UpdatedAt) via BaseEntity

### 🧠 Domain Services & Business Rules

#### 🏥 **Appointment Domain Service**

- **📅 Intelligent Scheduling**: Validates working hours, break times, and availability
- **💰 Dynamic Pricing**: Consultation fees based on specialization, duration, and type
  - Cardiology: 1.5x multiplier | Emergency: 2.0x multiplier
  - Duration-based pricing (15min = 0.5x, 90min = 2.0x)
- **⏰ Cancellation Policy**: 24h notice for regular, 2h for emergency appointments
- **🚨 Emergency Slots**: 2-hour availability windows for urgent care
- **📊 Next Available**: Smart algorithm finds earliest slot within 30 days

#### 👤 **Patient Domain Service**

- **⚠️ Risk Assessment**: Age + medical history + allergies = risk level
- **♿ Accessibility**: Auto-generates accommodation requirements
  - Wheelchair access, interpreters, pediatric equipment
- **📋 Eligibility Rules**: Age-based restrictions (pediatric <18, geriatric 65+)
- **📅 Appointment Limits**: Risk-based daily appointment restrictions
- **🔄 Follow-up Intervals**: Condition-specific scheduling recommendations

### 🔄 Enhanced Business Logic

- **Patient Management** with comprehensive risk profiling
- **Appointment Scheduling** with intelligent conflict resolution
- **Doctor Availability** with break time and working hour validation
- **Search Functionality** across multiple fields with performance optimization
- **Soft Delete** support with audit trails via BaseEntity
- **Business Rule Enforcement** through dedicated domain services
- **Domain Exception Handling** for business rule violations

## 🛡️ Security & Authentication

### 🔐 Authentication & Authorization

- **JWT Bearer Authentication** with secure token validation
- **ASP.NET Core Identity** integration with role-based access
- **Role-Based Authorization** (Admin, Doctor, Patient, Nurse, Receptionist, Manager)
- **Password Policy** enforcement with complexity requirements
- **Account Lockout** protection (5 attempts, 5-minute lockout)
- **Policy-Based Authorization** for fine-grained access control

### 🛡️ Security Features

- **Security Headers** (XSS Protection, Content Type Options, Frame Options)
- **HTTPS Redirection** for secure communication
- **CORS Policy** configuration with allowed origins
- **Input Validation** and sanitization with FluentValidation
- **Authentication Policies** for different user roles and operations

## 🧪 Testing the API

### Sample Requests

**Create a Patient (with Risk Assessment):**

```json
POST /api/patients
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@email.com",
  "phoneNumber": "+1234567890",
  "dateOfBirth": "1990-05-15T00:00:00",
  "gender": "Male",
  "address": "123 Main St, City, State 12345",
  "medicalHistory": "Diabetes, Hypertension",
  "allergies": "Penicillin",
  "emergencyContactName": "Jane Doe",
  "emergencyContactPhone": "+1234567891"
}
```

**Create an Appointment (with Smart Scheduling):**

```json
POST /api/appointments
{
  "patientId": 1,
  "doctorId": 1,
  "appointmentDate": "2024-01-15T10:00:00",
  "duration": "00:30:00",
  "reasonForVisit": "Regular checkup",
  "appointmentType": "Regular",
  "consultationFee": 150.00,
  "notes": "Patient requests morning appointment"
}
```

**Sample Domain Service Response (Risk Assessment):**

```json
GET /api/patients/1/risk-assessment
{
  "patientId": 1,
  "riskLevel": "Medium",
  "riskScore": 7.5,
  "factors": [
    "Age: 34 years (Low risk)",
    "Medical History: Diabetes, Hypertension (High risk)",
    "Allergies: Present (Medium risk)"
  ],
  "recommendedFollowupDays": 30,
  "maxAppointmentsPerDay": 2,
  "specialAccommodations": ["Monitor blood pressure", "Dietary consultation"]
}
```

## 🔧 Configuration

### Database Connection

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ClinicApiDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

### JWT Settings

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyHereMustBe32Characters!",
    "Issuer": "ClinicApi",
    "Audience": "ClinicApiUsers",
    "ExpirationInHours": 24
  }
}
```

### CORS Settings

```json
{
  "AllowedOrigins": [
    "http://localhost:3000",
    "http://localhost:5173",
    "http://localhost:8080"
  ]
}
```

## 📈 Current Status & Roadmap

### ✅ Implemented Features

- **Clean Architecture Structure** with proper layer separation
- **Domain Layer** with entities, services, and business logic
- **Repository Pattern** with generic and specific implementations
- **Database Context** with Entity Framework Core 9.0
- **Domain Services** for complex business rules (Appointments, Patients)
- **Controllers** with RESTful endpoints
- **Dependency Injection** configuration
- **Authentication/Authorization** infrastructure
- **Swagger Documentation** with comprehensive API specs
- **CQRS Structure** ready for implementation

### 🚧 In Progress (TODO)

- **CQRS Handlers** - Command and Query handlers with MediatR
- **Authentication Service** - Complete Auth implementation with CQRS
- **FluentValidation** - Input validation rules for commands/queries
- **Application Services** - Use case implementations
- **Unit Tests** - Comprehensive test coverage
- **Integration Tests** - End-to-end API testing

### 🔮 Planned Features

- **Medical Records Management** with patient history tracking
- **Prescription Management** with drug interaction checking
- **Doctor Schedule Management** with dynamic availability
- **Appointment Notifications** (Email/SMS/Push)
- **File Upload** for medical documents and reports
- **Reporting & Analytics Dashboard** with business insights
- **Audit Logging** for compliance and security
- **Rate Limiting** to prevent API abuse
- **Caching** (Redis) for performance optimization
- **Background Jobs** (Hangfire) for async processing
- **Payment Integration** for billing and invoicing
- **Insurance Claims** processing and verification

### 🏗️ Technical Improvements

- **MediatR Implementation** - Complete CQRS pattern
- **Docker Support** with containerization
- **CI/CD Pipeline** with GitHub Actions
- **Logging** with Serilog enhancements
- **Monitoring** with Application Insights
- **API Versioning** for backward compatibility
- **Response Caching** for performance optimization

## 📞 Support

- **Swagger Documentation**: Available at `/swagger` when running the application
- **Health Status**: Available at `/health`
- **Architecture**: Follows Clean Architecture and CQRS patterns for enterprise-grade scalability

---

**Note**: This API is built with Clean Architecture principles and CQRS pattern. The authentication endpoints are currently placeholder implementations and will be completed with proper MediatR command/query handlers.
