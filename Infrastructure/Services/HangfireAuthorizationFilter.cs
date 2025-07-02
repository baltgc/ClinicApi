using Hangfire.Dashboard;

namespace ClinicApi.Infrastructure.Services;

/// <summary>
/// Authorization filter for Hangfire Dashboard
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // In development, allow all access
        // In production, you should implement proper authorization logic
        return true;
    }
}
