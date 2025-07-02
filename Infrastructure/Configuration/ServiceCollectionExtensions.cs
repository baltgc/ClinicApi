using System.Reflection;
using System.Text;
using ClinicApi.Application.Mapping;
using ClinicApi.Application.Services;
using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Enums;
using ClinicApi.Domain.Interfaces;
using ClinicApi.Domain.Services;
using ClinicApi.Infrastructure.Data.Context;
using ClinicApi.Infrastructure.Data.Repositories;
using ClinicApi.Infrastructure.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ClinicApi.Infrastructure.Configuration;

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
        // Domain Services
        services.AddScoped<IAppointmentDomainService, AppointmentDomainService>();
        services.AddScoped<IPatientDomainService, PatientDomainService>();

        // Application Services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IBackgroundJobService, BackgroundJobService>();

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
    /// Configures AsyncAPI documentation services
    /// </summary>
    public static IServiceCollection AddAsyncApiConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<
            Infrastructure.Services.IAsyncApiService,
            Infrastructure.Services.AsyncApiService
        >();
        return services;
    }

    /// <summary>
    /// Configures CORS
    /// </summary>
    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowSpecificOrigins",
                builder =>
                {
                    builder
                        .WithOrigins(
                            configuration.GetSection("AllowedOrigins").Get<string[]>()
                                ?? new[] { "http://localhost:3000" }
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
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
        services.AddHealthChecks();

        return services;
    }

    /// <summary>
    /// Configures HTTPS redirection options
    /// </summary>
    public static IServiceCollection AddHttpsRedirectionConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<HttpsRedirectionOptions>(options =>
        {
            options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            options.HttpsPort = configuration.GetValue<int?>("HttpsPort") ?? 5001;
        });

        return services;
    }

    /// <summary>
    /// Configures Hangfire for background job processing
    /// </summary>
    public static IServiceCollection AddHangfireConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHangfire(config =>
        {
            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(
                    configuration.GetConnectionString("DefaultConnection"),
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true,
                    }
                );
        });

        // Add Hangfire server
        services.AddHangfireServer();

        return services;
    }

    /// <summary>
    /// Main service registration method
    /// </summary>
    public static IServiceCollection AddClinicServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddControllers();
        services.AddDatabaseConfiguration(configuration);
        services.AddIdentityConfiguration();
        services.AddJwtAuthentication(configuration);
        services.AddAuthorizationPolicies();
        services.AddRepositories();
        services.AddApplicationServices();
        services.AddAutoMapperConfiguration();
        services.AddAsyncApiConfiguration(configuration);
        services.AddCorsConfiguration(configuration);
        services.AddHealthCheckConfiguration();
        services.AddHttpsRedirectionConfiguration(configuration);
        services.AddHangfireConfiguration(configuration);

        // Add MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(MappingProfile).Assembly)
        );

        return services;
    }
}
