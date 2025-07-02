using ClinicApi.Infrastructure.Configuration;
using ClinicApi.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure all services using extension methods
builder.Services.AddClinicServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline using extension methods
app.ConfigureDevelopmentPipeline()
    .AddSecurityHeaders()
    .ConfigureMiddlewarePipeline()
    .ConfigureEndpoints()
    .ConfigureRecurringJobs();

// Initialize database in development
await app.InitializeDatabaseAsync();

app.Run();
