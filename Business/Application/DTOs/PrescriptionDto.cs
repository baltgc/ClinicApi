namespace ClinicApi.Business.Application.DTOs;

public record PrescriptionResponseDto(
    int Id,
    int PatientId,
    int DoctorId,
    int? MedicalRecordId,
    string MedicationName,
    string Dosage,
    string Frequency,
    int DurationDays,
    string? Instructions,
    string? SideEffects,
    DateTime PrescribedDate,
    DateTime? StartDate,
    DateTime? EndDate,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    PatientResponseDto? Patient = null,
    DoctorResponseDto? Doctor = null
);

public record CreatePrescriptionDto(
    int PatientId,
    int DoctorId,
    int? MedicalRecordId,
    string MedicationName,
    string Dosage,
    string Frequency,
    int DurationDays,
    string? Instructions = null,
    string? SideEffects = null,
    DateTime? StartDate = null
);

public record UpdatePrescriptionDto(
    string? MedicationName = null,
    string? Dosage = null,
    string? Frequency = null,
    int? DurationDays = null,
    string? Instructions = null,
    string? SideEffects = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string? Status = null
);
