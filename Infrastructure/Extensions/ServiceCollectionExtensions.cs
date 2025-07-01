using System.Reflection;
using System.Text;
using ClinicApi.Business.Application.Interfaces;
using ClinicApi.Business.Application.Mapping;
using ClinicApi.Business.Application.Services;
using ClinicApi.Business.Domain.Constants;
using ClinicApi.Business.Domain.Interfaces;
using ClinicApi.Business.Domain.Models;
using ClinicApi.Business.Domain.Services;
using ClinicApi.Infrastructure.Data.Context;
using ClinicApi.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ClinicApi.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures Entity Framework and database context
    /// </summary>
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<ClinicDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );

        return services;
    }

    /// <summary>
    /// Configures ASP.NET Core Identity
    /// </summary>
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                // Sign in settings
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<ClinicDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    /// <summary>
    /// Configures JWT authentication
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey =
            jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey not configured");

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero,
                };
            });

        return services;
    }

    /// <summary>
    /// Configures authorization policies
    /// </summary>
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole(ClinicRoles.Admin));
            options.AddPolicy("DoctorOnly", policy => policy.RequireRole(ClinicRoles.Doctor));
            options.AddPolicy("PatientOnly", policy => policy.RequireRole(ClinicRoles.Patient));
            options.AddPolicy(
                "MedicalStaff",
                policy => policy.RequireRole(ClinicRoles.Doctor, ClinicRoles.Nurse)
            );
            options.AddPolicy(
                "AdminOrManager",
                policy => policy.RequireRole(ClinicRoles.Admin, ClinicRoles.Manager)
            );
            options.AddPolicy(
                "StaffOnly",
                policy =>
                    policy.RequireRole(
                        ClinicRoles.Doctor,
                        ClinicRoles.Nurse,
                        ClinicRoles.Receptionist,
                        ClinicRoles.Manager
                    )
            );
        });

        return services;
    }

    /// <summary>
    /// Registers repositories using dependency injection
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        return services;
    }

    /// <summary>
    /// Registers application services using dependency injection
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application Services
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IAuthService, AuthService>();

        // Domain Services
        services.AddScoped<IAppointmentDomainService, AppointmentDomainService>();
        services.AddScoped<IPatientDomainService, PatientDomainService>();

        return services;
    }

    /// <summary>
    /// Configures AutoMapper
    /// </summary>
    public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        return services;
    }

    /// <summary>
    /// Configures Swagger/OpenAPI documentation
    /// </summary>
    public static IServiceCollection AddSwaggerConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = configuration["SwaggerConfig:Title"] ?? "Clinic API",
                    Version = configuration["SwaggerConfig:Version"] ?? "v1",
                    Description =
                        configuration["SwaggerConfig:Description"] ?? "Clinic Management API",
                    Contact = new OpenApiContact
                    {
                        Name = "Clinic API Support",
                        Email = "support@clinicapi.com",
                    },
                }
            );

            // Include XML comments for better API documentation
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // Add JWT authentication to Swagger
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                }
            );

            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        Array.Empty<string>()
                    },
                }
            );
        });

        return services;
    }

    /// <summary>
    /// Configures CORS policies
    /// </summary>
    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                "ClinicApiPolicy",
                policy =>
                {
                    var allowedOrigins =
                        configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                        ?? new[] { "*" };

                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
            );
        });

        return services;
    }

    /// <summary>
    /// Configures health checks
    /// </summary>
    public static IServiceCollection AddHealthCheckConfiguration(this IServiceCollection services)
    {
        services.AddHealthChecks().AddDbContextCheck<ClinicDbContext>();
        return services;
    }

    /// <summary>
    /// Configures model validation
    /// </summary>
    public static IServiceCollection AddModelValidationConfiguration(
        this IServiceCollection services
    )
    {
        services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context
                    .ModelState.Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage);

                var response = new { Message = "Validation failed", Errors = errors };

                return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
            };
        });

        return services;
    }

    /// <summary>
    /// Configures all clinic application services in one call
    /// </summary>
    public static IServiceCollection AddClinicServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Add core services
        services.AddControllers();

        // Add all configurations
        services
            .AddDatabaseConfiguration(configuration)
            .AddIdentityConfiguration()
            .AddJwtAuthentication(configuration)
            .AddAuthorizationPolicies()
            .AddRepositories()
            .AddApplicationServices()
            .AddAutoMapperConfiguration()
            .AddSwaggerConfiguration(configuration)
            .AddCorsConfiguration(configuration)
            .AddHealthCheckConfiguration()
            .AddModelValidationConfiguration();

        return services;
    }
}
