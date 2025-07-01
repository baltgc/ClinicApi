namespace ClinicApi.Business.Application.DTOs;

public record PatientResponseDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth,
    string Gender,
    string? Address,
    string? BloodType,
    string? MedicalHistory,
    string? Allergies,
    string? EmergencyContact,
    string? EmergencyContactPhone,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsActive
);

public record CreatePatientDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth,
    string Gender,
    string? Address = null,
    string? BloodType = null,
    string? MedicalHistory = null,
    string? Allergies = null,
    string? EmergencyContact = null,
    string? EmergencyContactPhone = null
);

public record UpdatePatientDto(
    string? FirstName = null,
    string? LastName = null,
    string? Email = null,
    string? PhoneNumber = null,
    DateTime? DateOfBirth = null,
    string? Gender = null,
    string? Address = null,
    string? BloodType = null,
    string? MedicalHistory = null,
    string? Allergies = null,
    string? EmergencyContact = null,
    string? EmergencyContactPhone = null
);
