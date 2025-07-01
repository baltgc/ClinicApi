using ClinicApi.Business.Application.DTOs;
using ClinicApi.Business.Domain.Models;

namespace ClinicApi.Business.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
    Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    Task<string> GenerateJwtToken(ApplicationUser user, IList<string> roles);
    Task<UserResponseDto?> GetUserByIdAsync(string userId);
    Task<IEnumerable<UserResponseDto>> GetUsersAsync(
        UserFilterDto filter,
        int page,
        int pageSize,
        string sortBy,
        bool isAscending
    );
    Task<UserResponseDto?> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
    Task<bool> AssignRoleAsync(AssignRoleDto assignRoleDto);
    Task<bool> RemoveRoleAsync(string userId, string role);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
    Task<bool> DeactivateUserAsync(string userId);
    Task<bool> ActivateUserAsync(string userId);
}
