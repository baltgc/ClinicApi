using ClinicApi.Infrastructure.Data.Context;
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
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic API v1");
                c.RoutePrefix = string.Empty; // Serve Swagger at root
            });
        }

        return app;
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
        app.UseHttpsRedirection();
        app.UseCors("AllowSpecificOrigins");
        app.UseAuthentication();
        app.UseAuthorization();

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

        if (app.Environment.IsDevelopment())
        {
            await context.Database.MigrateAsync();
        }

        return app;
    }
}
