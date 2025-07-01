namespace ClinicApi.Business.Application.DTOs;

public record DoctorResponseDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Specialization,
    string LicenseNumber,
    string? Qualifications,
    int YearsOfExperience,
    string? Bio,
    string? OfficeAddress,
    decimal ConsultationFee,
    string? AvailableHours,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsActive
);

public record CreateDoctorDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Specialization,
    string LicenseNumber,
    string? Qualifications = null,
    int YearsOfExperience = 0,
    string? Bio = null,
    string? OfficeAddress = null,
    decimal ConsultationFee = 0,
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
    int? YearsOfExperience = null,
    string? Bio = null,
    string? OfficeAddress = null,
    decimal? ConsultationFee = null,
    string? AvailableHours = null
);
