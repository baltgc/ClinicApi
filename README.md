# Clinic Management API

A comprehensive RESTful API for managing clinic operations including patients, doctors, appointments, medical records, and prescriptions.

## 🏗️ Clean Architecture Overview

This API follows **Robert C. Martin's Clean Architecture** principles with clear separation of concerns and dependency inversion.

### 📁 Project Structure

```
ClinicApi/
├── Business/                          # 🧠 Pure Business Logic
│   ├── Domain/                        # Enterprise Business Rules
│   │   ├── Models/                    # Domain Entities (Patient, Doctor, Appointment)
│   │   ├── Constants/                 # Domain Constants (ClinicRoles)
│   │   ├── Interfaces/                # Repository Contracts (Dependency Inversion)
│   │   │   ├── IGenericRepository.cs
│   │   │   ├── IPatientRepository.cs
│   │   │   ├── IDoctorRepository.cs
│   │   │   └── IAppointmentRepository.cs
│   │   └── Services/                  # 🎯 Complex Business Rules
│   │       ├── IAppointmentDomainService.cs
│   │       ├── AppointmentDomainService.cs
│   │       ├── IPatientDomainService.cs
│   │       └── PatientDomainService.cs
│   └── Application/                   # Application Business Rules
│       ├── Interfaces/                # Application Service Contracts
│       ├── Services/                  # Use Cases & Application Logic
│       ├── DTOs/                      # Data Transfer Objects
│       ├── Mapping/                   # AutoMapper Profiles
│       └── Validators/                # Input Validation Rules
├── Infrastructure/                    # 🔧 External Concerns
│   ├── Api/                          # Presentation Layer
│   │   ├── Controllers/              # RESTful API Endpoints
│   │   └── Middleware/               # Cross-cutting Concerns
│   └── Data/                         # Data Access Layer
│       ├── Context/                  # Entity Framework DbContext
│       ├── Repositories/             # Repository Implementations
│       └── Migrations/               # Database Migrations
├── Program.cs                        # 🚀 Application Entry Point
└── appsettings.json                  # Configuration
```

### 🎯 Clean Architecture Benefits

- **🔄 Dependency Inversion**: Infrastructure depends on Business, not vice versa
- **🧪 Testability**: Business logic isolated and easily unit testable
- **📦 Separation of Concerns**: Each layer has a single responsibility
- **🔧 Maintainability**: Changes in one layer don't affect others
- **📈 Scalability**: Easy to add new features following established patterns
- **🏗️ Enterprise Ready**: Follows industry-standard architectural patterns

### 🔧 Technology Stack

- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database (LocalDB for development)
- **AutoMapper** - Object-to-object mapping
- **Swagger/OpenAPI** - API documentation
- **FluentValidation** - Input validation
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

   - Edit `appsettings.json` or `appsettings.Development.json`
   - Modify the `DefaultConnection` string if needed

4. **Run the application**

   ```bash
   dotnet run
   ```

5. **Access the API**
   - API Documentation: `https://localhost:5001/` (Swagger UI)
   - Health Check: `https://localhost:5001/health`
   - API Info: `https://localhost:5001/api/info`

## 📚 API Endpoints

### 🔐 Authentication (`/api/auth`)

- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh JWT token
- `POST /api/auth/logout` - User logout

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

- **Patient** - Patient information and demographics
- **Doctor** - Doctor profiles and specializations
- **Appointment** - Appointment scheduling
- **MedicalRecord** - Patient visit records and diagnoses
- **Prescription** - Medication prescriptions
- **DoctorSchedule** - Doctor availability schedules

### Key Relationships

- Patient → Appointments (One-to-Many)
- Doctor → Appointments (One-to-Many)
- Patient → MedicalRecords (One-to-Many)
- Doctor → MedicalRecords (One-to-Many)
- MedicalRecord → Prescriptions (One-to-Many)
- Appointment → MedicalRecords (One-to-Many)

## 🎯 Key Features

### ✅ Core Implementation

- **RESTful API Design** with proper HTTP methods and status codes
- **Clean Architecture** with complete separation of concerns
- **Repository Pattern** with domain-owned interfaces (Dependency Inversion)
- **Dependency Injection** for loose coupling and testability
- **AutoMapper** for object mapping
- **Swagger Documentation** with comprehensive XML comments
- **Health Checks** for monitoring and diagnostics
- **CORS Configuration** for cross-origin requests
- **Model Validation** with custom error responses
- **Entity Framework** with proper relationships and migrations
- **Automatic Timestamps** (CreatedAt, UpdatedAt)

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
- **Soft Delete** support with audit trails
- **Business Rule Enforcement** through dedicated domain services

## 🛡️ Security & Authentication

### 🔐 Authentication & Authorization

- **JWT Bearer Authentication** with secure token validation
- **ASP.NET Core Identity** integration with role-based access
- **Role-Based Authorization** (Admin, Doctor, Patient, Nurse, Receptionist, Manager)
- **Password Policy** enforcement with complexity requirements
- **Account Lockout** protection (5 attempts, 5-minute lockout)
- **Default Admin Account** auto-creation for development

### 🛡️ Security Features

- **Security Headers** (XSS Protection, Content Type Options, Frame Options)
- **HTTPS Redirection** for secure communication
- **CORS Policy** configuration with allowed origins
- **Input Validation** and sanitization
- **Referrer Policy** for privacy protection

## 🧪 Testing the API

### Sample Requests

**Register User:**

```json
POST /api/auth/register
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@email.com",
  "password": "SecurePass123!",
  "phoneNumber": "+1234567890",
  "role": "Patient"
}
```

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

### CORS Settings

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173",
      "http://localhost:8080"
    ]
  }
}
```

## 📈 Future Enhancements

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

- **Unit Tests** with xUnit
- **Integration Tests**
- **Docker Support**
- **CI/CD Pipeline**
- **Logging** with Serilog
- **Monitoring** with Application Insights
- **API Versioning**
- **Response Caching**

## 📞 Support

- Documentation: Available at `/index.html` when running the application
- Health Status: Available at `/health`
