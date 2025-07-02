using ClinicApi.Infrastructure.Data.Context;
using ClinicApi.Infrastructure.Services;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Web.Configuration;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures the application pipeline for development environment
    /// </summary>
    public static WebApplication ConfigureDevelopmentPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            // Serve AsyncAPI documentation
            app.UseAsyncApiDocumentation();
        }

        return app;
    }

    /// <summary>
    /// Configures AsyncAPI documentation middleware
    /// </summary>
    public static WebApplication UseAsyncApiDocumentation(this WebApplication app)
    {
        // Serve AsyncAPI specification
        app.MapGet(
            "/asyncapi.yaml",
            async (IAsyncApiService asyncApiService) =>
            {
                var filePath = Path.Combine(
                    app.Environment.ContentRootPath,
                    asyncApiService.GetSpecificationPath()
                );
                if (File.Exists(filePath))
                {
                    var content = await File.ReadAllTextAsync(filePath);
                    return Results.Text(content, "application/x-yaml");
                }
                return Results.NotFound("AsyncAPI specification not found");
            }
        );

        // Serve AsyncAPI documentation UI
        app.MapGet(
            "/",
            (IAsyncApiService asyncApiService) =>
            {
                var config = asyncApiService.GetConfiguration();
                var html = GetAsyncApiHtml(config);
                return Results.Text(html, "text/html");
            }
        );

        return app;
    }

    /// <summary>
    /// Configures recurring background jobs
    /// </summary>
    public static WebApplication ConfigureRecurringJobs(this WebApplication app)
    {
        // Set up recurring jobs
        RecurringJob.AddOrUpdate<ClinicApi.Infrastructure.Services.BackgroundJobService>(
            "daily-appointment-summary",
            service => service.SendDailyAppointmentSummaryAsync(),
            Cron.Daily(8, 0)
        ); // Run daily at 8:00 AM

        RecurringJob.AddOrUpdate<ClinicApi.Infrastructure.Services.BackgroundJobService>(
            "patient-data-cleanup",
            service => service.ProcessPatientDataCleanupAsync(),
            Cron.Weekly(DayOfWeek.Sunday, 2, 0)
        ); // Run weekly on Sunday at 2:00 AM

        return app;
    }

    private static string GetAsyncApiHtml(AsyncApiConfig config)
    {
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{config.Title} - AsyncAPI Documentation</title>
    <style>
        body {{
            margin: 0;
            padding: 0;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            background-color: #f8f9fa;
            line-height: 1.6;
        }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 2rem;
            text-align: center;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }}
        .header h1 {{
            margin: 0 0 0.5rem 0;
            font-size: 2.5rem;
            font-weight: 300;
        }}
        .header p {{
            margin: 0;
            opacity: 0.9;
            font-size: 1.1rem;
        }}
        .container {{
            max-width: 1200px;
            margin: 2rem auto;
            padding: 0 2rem;
        }}
        .card {{
            background: white;
            border-radius: 8px;
            padding: 2rem;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            margin-bottom: 2rem;
        }}
        .card h2 {{
            margin-top: 0;
            color: #333;
            border-bottom: 2px solid #eee;
            padding-bottom: 0.5rem;
        }}
        .endpoints {{
            display: grid;
            gap: 1rem;
            margin-top: 2rem;
        }}
        .endpoint {{
            padding: 1rem;
            border: 1px solid #e9ecef;
            border-radius: 6px;
            background: #f8f9fa;
            transition: box-shadow 0.2s;
        }}
        .endpoint:hover {{
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }}
        .method {{
            display: inline-block;
            padding: 0.25rem 0.75rem;
            border-radius: 4px;
            color: white;
            font-weight: bold;
            font-size: 0.8rem;
            margin-right: 1rem;
            text-transform: uppercase;
        }}
        .get {{ background-color: #61affe; }}
        .post {{ background-color: #49cc90; }}
        .put {{ background-color: #fca130; }}
        .delete {{ background-color: #f93e3e; }}
        .patch {{ background-color: #50e3c2; }}
        .spec-link {{
            display: inline-block;
            background: #007bff;
            color: white;
            padding: 0.75rem 1.5rem;
            text-decoration: none;
            border-radius: 6px;
            font-weight: 500;
            transition: background-color 0.2s;
            margin: 1rem 0;
        }}
        .spec-link:hover {{
            background: #0056b3;
        }}
        .info {{
            background: #e7f3ff;
            border: 1px solid #b3d9ff;
            border-radius: 6px;
            padding: 1rem;
            margin: 1rem 0;
        }}
        .auth-example {{
            background: #f8f9fa;
            padding: 1rem;
            border-radius: 4px;
            font-family: 'Courier New', monospace;
            border-left: 4px solid #007bff;
            margin: 1rem 0;
        }}
        .footer {{
            text-align: center;
            padding: 2rem;
            color: #6c757d;
            font-size: 0.9rem;
        }}
        .server-info {{
            background: #f1f3f4;
            padding: 1rem;
            border-radius: 6px;
            margin: 1rem 0;
        }}
        .server-info strong {{
            color: #333;
        }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>🏥 {config.Title}</h1>
        <p>AsyncAPI Documentation - Version {config.Version}</p>
    </div>

    <div class=""container"">
        <div class=""card"">
            <h2>📋 API Overview</h2>
            <p>{config.Description}</p>
            
            <div class=""info"">
                <strong>ℹ️ Information:</strong> This API uses AsyncAPI specification format to document REST endpoints. 
                For the complete machine-readable specification, download the YAML file below.
            </div>

            <a href=""/asyncapi.yaml"" class=""spec-link"" download>📄 Download AsyncAPI Specification (YAML)</a>
        </div>

        <div class=""card"">
            <h2>🔐 Authentication</h2>
            <p>This API uses <strong>JWT Bearer Authentication</strong>. Include the following header in your requests:</p>
            <div class=""auth-example"">
                Authorization: Bearer &lt;your-jwt-token&gt;
            </div>
            <p><strong>Available Roles:</strong> Admin, Doctor, Nurse, Receptionist, Manager, Patient</p>
        </div>

        <div class=""card"">
            <h2>🌐 Server Information</h2>
            <div class=""server-info"">
                <p><strong>Development HTTP:</strong> http://localhost:5000</p>
                <p><strong>Development HTTPS:</strong> https://localhost:5001</p>
            </div>
        </div>

        <div class=""card"">
            <h2>🔗 Available Endpoints</h2>
            
            <h3>Authentication Endpoints</h3>
            <div class=""endpoints"">
                <div class=""endpoint"">
                    <span class=""method post"">POST</span>
                    <strong>/api/auth/login</strong>
                    <p>Authenticate user with email and password. Returns JWT token for subsequent requests.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method post"">POST</span>
                    <strong>/api/auth/register</strong>
                    <p>Register a new user in the system with role assignment.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method get"">GET</span>
                    <strong>/api/auth/profile</strong>
                    <p>Get authenticated user's profile information (requires JWT token).</p>
                </div>
            </div>

            <h3>Patient Management</h3>
            <div class=""endpoints"">
                <div class=""endpoint"">
                    <span class=""method get"">GET</span>
                    <strong>/api/patients</strong>
                    <p>Retrieve all patients in the system.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method post"">POST</span>
                    <strong>/api/patients</strong>
                    <p>Create a new patient record with personal and contact information.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method get"">GET</span>
                    <strong>/api/patients/{{id}}</strong>
                    <p>Retrieve a specific patient by their unique identifier.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method put"">PUT</span>
                    <strong>/api/patients/{{id}}</strong>
                    <p>Update an existing patient's information.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method delete"">DELETE</span>
                    <strong>/api/patients/{{id}}</strong>
                    <p>Remove a patient record from the system.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method get"">GET</span>
                    <strong>/api/patients/active</strong>
                    <p>Get all currently active patients.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method get"">GET</span>
                    <strong>/api/patients/search</strong>
                    <p>Search patients by name, email, or other criteria.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method patch"">PATCH</span>
                    <strong>/api/patients/{{id}}/deactivate</strong>
                    <p>Deactivate a patient without removing their record.</p>
                </div>
            </div>

            <h3>Appointment Management</h3>
            <div class=""endpoints"">
                <div class=""endpoint"">
                    <span class=""method get"">GET</span>
                    <strong>/api/appointments</strong>
                    <p>Retrieve all appointments in the system.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method post"">POST</span>
                    <strong>/api/appointments</strong>
                    <p>Schedule a new appointment between patient and doctor.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method get"">GET</span>
                    <strong>/api/appointments/{{id}}</strong>
                    <p>Get details of a specific appointment.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method put"">PUT</span>
                    <strong>/api/appointments/{{id}}</strong>
                    <p>Update appointment details (date, time, notes, etc.).</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method delete"">DELETE</span>
                    <strong>/api/appointments/{{id}}</strong>
                    <p>Remove an appointment from the system.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method patch"">PATCH</span>
                    <strong>/api/appointments/{{id}}/cancel</strong>
                    <p>Cancel an appointment while keeping the record.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method get"">GET</span>
                    <strong>/api/appointments/doctor/{{doctorId}}</strong>
                    <p>Get all appointments for a specific doctor.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method get"">GET</span>
                    <strong>/api/appointments/patient/{{patientId}}</strong>
                    <p>Get all appointments for a specific patient.</p>
                </div>
                
                <div class=""endpoint"">
                    <span class=""method get"">GET</span>
                    <strong>/api/appointments/availability</strong>
                    <p>Check doctor availability for appointment scheduling.</p>
                </div>
            </div>
        </div>

        <div class=""card"">
            <h2>🏥 System Health</h2>
            <div class=""endpoint"">
                <span class=""method get"">GET</span>
                <strong>/health</strong>
                <p>Check the overall health status of the API and database connectivity.</p>
            </div>
        </div>
    </div>

    <div class=""footer"">
        <p>Built with ❤️ using ASP.NET Core 9, Clean Architecture, and AsyncAPI</p>
        <p>AsyncAPI Specification Version: {config.Version}</p>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Adds security headers to responses
    /// </summary>
    public static WebApplication AddSecurityHeaders(this WebApplication app)
    {
        app.Use(
            async (context, next) =>
            {
                context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
                context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
                context.Response.Headers.TryAdd("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.TryAdd(
                    "Referrer-Policy",
                    "strict-origin-when-cross-origin"
                );

                await next();
            }
        );

        return app;
    }

    /// <summary>
    /// Configures the middleware pipeline
    /// </summary>
    public static WebApplication ConfigureMiddlewarePipeline(this WebApplication app)
    {
        // Only use HTTPS redirection in production
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.UseCors("AllowSpecificOrigins");
        app.UseAuthentication();
        app.UseAuthorization();

        // Add Hangfire Dashboard middleware after authentication
        if (app.Environment.IsDevelopment())
        {
            app.UseHangfireDashboard(
                "/hangfire",
                new DashboardOptions { Authorization = new[] { new HangfireAuthorizationFilter() } }
            );
        }

        return app;
    }

    /// <summary>
    /// Configures endpoints
    /// </summary>
    public static WebApplication ConfigureEndpoints(this WebApplication app)
    {
        app.MapControllers();
        app.MapHealthChecks("/health");

        return app;
    }

    /// <summary>
    /// Initializes the database
    /// </summary>
    public static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();

        // Run migrations in Development and Production
        if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
        {
            await context.Database.MigrateAsync();
        }

        return app;
    }
}
