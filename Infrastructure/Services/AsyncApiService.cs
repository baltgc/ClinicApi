using Microsoft.Extensions.Configuration;

namespace ClinicApi.Infrastructure.Services;

/// <summary>
/// Service for AsyncAPI configuration and utilities
/// </summary>
public class AsyncApiService : IAsyncApiService
{
    private readonly IConfiguration _configuration;

    public AsyncApiService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the AsyncAPI specification file path
    /// </summary>
    public string GetSpecificationPath()
    {
        return "asyncapi.yaml";
    }

    /// <summary>
    /// Gets AsyncAPI configuration from appsettings
    /// </summary>
    public AsyncApiConfig GetConfiguration()
    {
        var config = _configuration.GetSection("AsyncApiConfig");
        return new AsyncApiConfig
        {
            Title = config["Title"] ?? "Clinic Management API",
            Version = config["Version"] ?? "1.0.0",
            Description = config["Description"] ?? "A comprehensive clinic management system API",
        };
    }

    /// <summary>
    /// Validates that the AsyncAPI specification file exists
    /// </summary>
    public bool ValidateSpecificationExists(string rootPath)
    {
        var filePath = Path.Combine(rootPath, GetSpecificationPath());
        return File.Exists(filePath);
    }
}

/// <summary>
/// Interface for AsyncAPI service
/// </summary>
public interface IAsyncApiService
{
    string GetSpecificationPath();
    AsyncApiConfig GetConfiguration();
    bool ValidateSpecificationExists(string rootPath);
}

/// <summary>
/// Configuration model for AsyncAPI
/// </summary>
public class AsyncApiConfig
{
    public string Title { get; set; } = "API";
    public string Version { get; set; } = "1.0.0";
    public string Description { get; set; } = "API Documentation";
}
