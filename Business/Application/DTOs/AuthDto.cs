using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Business.Application.DTOs;

public record LoginDto([Required] [EmailAddress] string Email, [Required] string Password);

public record RegisterDto(
    [Required] [EmailAddress] string Email,
    [Required] [StringLength(100, MinimumLength = 6)] string Password,
    [Required] string FirstName,
    [Required] string LastName,
    string? PhoneNumber = null,
    DateTime? DateOfBirth = null,
    string? Gender = null,
    string? Address = null,
    string Role = "Patient"
);

public record ChangePasswordDto(
    [Required] string CurrentPassword,
    [Required] [StringLength(100, MinimumLength = 6)] string NewPassword
);

public record ForgotPasswordDto([Required] [EmailAddress] string Email);

public record ResetPasswordDto(
    [Required] [EmailAddress] string Email,
    [Required] string Token,
    [Required] [StringLength(100, MinimumLength = 6)] string NewPassword
);

public record AuthResponseDto(
    string Token,
    DateTime Expiration,
    string UserId,
    string Email,
    string FirstName,
    string LastName,
    string[] Roles,
    int? PatientId = null,
    int? DoctorId = null
);

public record UserResponseDto(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateTime? DateOfBirth,
    string? Gender,
    string? Address,
    string? Department,
    string? EmployeeId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsActive,
    string[] Roles,
    int? PatientId,
    int? DoctorId
);

public record UpdateUserDto(
    string? FirstName = null,
    string? LastName = null,
    string? PhoneNumber = null,
    DateTime? DateOfBirth = null,
    string? Gender = null,
    string? Address = null,
    string? Department = null
);

public record AssignRoleDto([Required] string UserId, [Required] string Role);

public record UserFilterDto(
    string? Role = null,
    string? SearchTerm = null,
    bool? IsActive = null,
    int PageNumber = 1,
    int PageSize = 10
);
