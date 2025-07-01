using ClinicApi.Business.Domain.Constants;
using ClinicApi.Business.Domain.Models;
using ClinicApi.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Identity;

namespace ClinicApi.Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures the HTTP request pipeline for development environment
    /// </summary>
    public static WebApplication ConfigureDevelopmentPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic API v1");
                c.RoutePrefix = string.Empty; // Makes Swagger UI available at root
            });
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        return app;
    }

    /// <summary>
    /// Configures security headers middleware
    /// </summary>
    public static WebApplication AddSecurityHeaders(this WebApplication app)
    {
        app.Use(
            async (context, next) =>
            {
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Append("X-Frame-Options", "DENY");
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Append(
                    "Referrer-Policy",
                    "strict-origin-when-cross-origin"
                );
                await next();
            }
        );

        return app;
    }

    /// <summary>
    /// Configures the main middleware pipeline
    /// </summary>
    public static WebApplication ConfigureMiddlewarePipeline(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseCors("ClinicApiPolicy");
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// Maps controllers and configures API endpoints
    /// </summary>
    public static WebApplication ConfigureEndpoints(this WebApplication app)
    {
        app.MapControllers();

        // Health Check Endpoint
        app.MapHealthChecks("/health");

        // API Information Endpoint
        app.MapGet(
                "/api/info",
                () =>
                    new
                    {
                        Title = "Clinic Management API",
                        Version = "1.0.0",
                        Environment = app.Environment.EnvironmentName,
                        Timestamp = DateTime.UtcNow,
                        Endpoints = new
                        {
                            Auth = "/api/auth",
                            Patients = "/api/patients",
                            Appointments = "/api/appointments",
                            Health = "/health",
                            Swagger = app.Environment.IsDevelopment() ? "/" : "/swagger",
                        },
                    }
            )
            .WithName("ApiInfo")
            .WithTags("Info");

        return app;
    }

    /// <summary>
    /// Initializes database and seeds initial data in development environment
    /// </summary>
    public static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ClinicDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Ensure database is created
                context.Database.EnsureCreated();

                // Create default admin user
                await CreateDefaultAdminUser(userManager, roleManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while creating the database.");
            }
        }

        return app;
    }

    /// <summary>
    /// Creates default admin user and roles
    /// </summary>
    private static async Task CreateDefaultAdminUser(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        // Ensure roles exist
        foreach (var role in ClinicRoles.AllRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create default admin user
        const string adminEmail = "admin@clinic.com";
        const string adminPassword = "Admin123!";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, ClinicRoles.Admin);
                Console.WriteLine($"Default admin user created: {adminEmail} / {adminPassword}");
            }
        }
    }
}
