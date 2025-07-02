namespace ClinicApi.Application.DTOs;

public record DoctorResponseDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Specialization,
    string LicenseNumber,
    string? Qualifications,
    string? Bio,
    string? OfficeAddress,
    decimal ConsultationFee,
    int ExperienceYears,
    string? AvailableHours,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateDoctorDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Specialization,
    string LicenseNumber,
    string? Qualifications = null,
    string? Bio = null,
    string? OfficeAddress = null,
    decimal ConsultationFee = 0,
    int ExperienceYears = 0,
    string? AvailableHours = null
);

public record UpdateDoctorDto(
    string? FirstName = null,
    string? LastName = null,
    string? Email = null,
    string? PhoneNumber = null,
    string? Specialization = null,
    string? LicenseNumber = null,
    string? Qualifications = null,
    string? Bio = null,
    string? OfficeAddress = null,
    decimal? ConsultationFee = null,
    int? ExperienceYears = null,
    string? AvailableHours = null,
    bool? IsActive = null
);
