using ClinicApi.Domain.Entities;

namespace ClinicApi.Application.Services;

public interface IJwtTokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user, IList<string> roles);
    string? GetUserIdFromToken(string token);
    bool ValidateToken(string token);
}
