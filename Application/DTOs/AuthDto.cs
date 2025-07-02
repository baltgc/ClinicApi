namespace ClinicApi.Application.DTOs;

public record LoginDto(
    string Email,
    string Password
);

public record RegisterDto(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Role,
    DateTime? DateOfBirth = null,
    string? Gender = null,
    string? Address = null,
    string? Department = null,
    string? EmployeeId = null,
    int? PatientId = null,
    int? DoctorId = null
);

public record AuthResponseDto(
    string Token,
    DateTime Expiration,
    UserResponseDto User
);

public record UserResponseDto(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? Department,
    string? EmployeeId,
    bool IsActive,
    IEnumerable<string> Roles
);

public record RefreshTokenDto(
    string RefreshToken
);

public record ChangePasswordDto(
    string CurrentPassword,
    string NewPassword
);

public record ForgotPasswordDto(
    string Email
);

public record ResetPasswordDto(
    string Email,
    string Token,
    string NewPassword
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