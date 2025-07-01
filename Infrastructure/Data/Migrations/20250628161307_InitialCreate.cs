using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(
                        type: "nvarchar(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    NormalizedName = table.Column<string>(
                        type: "nvarchar(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    LastName = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Email = table.Column<string>(
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    PhoneNumber = table.Column<string>(
                        type: "nvarchar(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    Specialization = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    LicenseNumber = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    Qualifications = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: true
                    ),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    Bio = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    OfficeAddress = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    ConsultationFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AvailableHours = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    UpdatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    LastName = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Email = table.Column<string>(
                        type: "nvarchar(255)",
                        maxLength: 255,
                        nullable: false
                    ),
                    PhoneNumber = table.Column<string>(
                        type: "nvarchar(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(
                        type: "nvarchar(10)",
                        maxLength: 10,
                        nullable: false
                    ),
                    Address = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    BloodType = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    MedicalHistory = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    Allergies = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    EmergencyContact = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    EmergencyContactPhone = table.Column<string>(
                        type: "nvarchar(20)",
                        maxLength: 20,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    UpdatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "DoctorSchedules",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    BreakStartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    BreakEndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    SlotDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    UpdatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorSchedules_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    Status = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    ReasonForVisit = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    Notes = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    CancellationReason = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    ConsultationFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    UpdatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Appointments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    LastName = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    PhoneNumberSecondary = table.Column<string>(
                        type: "nvarchar(20)",
                        maxLength: 20,
                        nullable: true
                    ),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(
                        type: "nvarchar(10)",
                        maxLength: 10,
                        nullable: true
                    ),
                    Address = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    Department = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    EmployeeId = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: true
                    ),
                    CreatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    UpdatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    DoctorId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(
                        type: "nvarchar(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    NormalizedUserName = table.Column<string>(
                        type: "nvarchar(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    Email = table.Column<string>(
                        type: "nvarchar(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    NormalizedEmail = table.Column<string>(
                        type: "nvarchar(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(
                        type: "datetimeoffset",
                        nullable: true
                    ),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    AppointmentId = table.Column<int>(type: "int", nullable: true),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChiefComplaint = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    Symptoms = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    Diagnosis = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    Treatment = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    VitalSigns = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    PhysicalExamination = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    LabResults = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    Recommendations = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),
                    FollowUpInstructions = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    NextVisitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    UpdatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: true
                    ),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_AspNetUserLogins",
                        x => new { x.LoginProvider, x.ProviderKey }
                    );
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_AspNetUserTokens",
                        x => new
                        {
                            x.UserId,
                            x.LoginProvider,
                            x.Name,
                        }
                    );
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    MedicalRecordId = table.Column<int>(type: "int", nullable: true),
                    MedicationName = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    Dosage = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Frequency = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    Instructions = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    SideEffects = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    PrescribedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    CreatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                    UpdatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETUTCDATE()"
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescriptions_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_Prescriptions_MedicalRecords_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_Prescriptions_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    {
                        "6f039d16-48a3-4133-a6b6-dd2d87ecca98",
                        "3be58f68-77b7-4a7a-9749-c7c05b3b2bb5",
                        "Nurse",
                        "NURSE",
                    },
                    {
                        "733a9d44-9869-4047-8056-4f1abe21df84",
                        "e5faeac0-997c-4aa2-8ef1-21011a9f764f",
                        "Manager",
                        "MANAGER",
                    },
                    {
                        "7ebd34bd-675f-4eb4-9a92-c82f5446cba8",
                        "18fd3dc8-9bc5-4dc2-b2d4-6f3603518a11",
                        "Patient",
                        "PATIENT",
                    },
                    {
                        "8ab5d31b-c48f-410a-a215-c9cf42f150dc",
                        "8f2be1eb-2c7c-4c06-826d-a9fef5cd0b86",
                        "Receptionist",
                        "RECEPTIONIST",
                    },
                    {
                        "d746c8e3-3c30-42fb-8230-c734689de285",
                        "aebf4639-b727-45bc-982c-7f4b977dbc68",
                        "Doctor",
                        "DOCTOR",
                    },
                    {
                        "ebdf28e7-fbc3-4aff-b53e-ba55feb7d05f",
                        "5062b3d2-9d66-4e43-be3d-eb9603d1bb24",
                        "Admin",
                        "ADMIN",
                    },
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments",
                column: "DoctorId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId"
            );

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId"
            );

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DoctorId",
                table: "AspNetUsers",
                column: "DoctorId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmployeeId",
                table: "AspNetUsers",
                column: "EmployeeId",
                unique: true,
                filter: "[EmployeeId] IS NOT NULL"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PatientId",
                table: "AspNetUsers",
                column: "PatientId"
            );

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_Email",
                table: "Doctors",
                column: "Email",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_LicenseNumber",
                table: "Doctors",
                column: "LicenseNumber",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSchedules_DoctorId",
                table: "DoctorSchedules",
                column: "DoctorId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_AppointmentId",
                table: "MedicalRecords",
                column: "AppointmentId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_DoctorId",
                table: "MedicalRecords",
                column: "DoctorId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_PatientId",
                table: "MedicalRecords",
                column: "PatientId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Email",
                table: "Patients",
                column: "Email",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_DoctorId",
                table: "Prescriptions",
                column: "DoctorId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_MedicalRecordId",
                table: "Prescriptions",
                column: "MedicalRecordId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_PatientId",
                table: "Prescriptions",
                column: "PatientId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AspNetRoleClaims");

            migrationBuilder.DropTable(name: "AspNetUserClaims");

            migrationBuilder.DropTable(name: "AspNetUserLogins");

            migrationBuilder.DropTable(name: "AspNetUserRoles");

            migrationBuilder.DropTable(name: "AspNetUserTokens");

            migrationBuilder.DropTable(name: "DoctorSchedules");

            migrationBuilder.DropTable(name: "Prescriptions");

            migrationBuilder.DropTable(name: "AspNetRoles");

            migrationBuilder.DropTable(name: "AspNetUsers");

            migrationBuilder.DropTable(name: "MedicalRecords");

            migrationBuilder.DropTable(name: "Appointments");

            migrationBuilder.DropTable(name: "Doctors");

            migrationBuilder.DropTable(name: "Patients");
        }
    }
}
