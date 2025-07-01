namespace ClinicApi.Business.Application.DTOs;

public record MedicalRecordResponseDto(
    int Id,
    int PatientId,
    int DoctorId,
    int? AppointmentId,
    DateTime VisitDate,
    string? ChiefComplaint,
    string? Symptoms,
    string? Diagnosis,
    string? Treatment,
    string? VitalSigns,
    string? PhysicalExamination,
    string? LabResults,
    string? Recommendations,
    string? FollowUpInstructions,
    DateTime? NextVisitDate,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    PatientResponseDto? Patient = null,
    DoctorResponseDto? Doctor = null
);

public record CreateMedicalRecordDto(
    int PatientId,
    int DoctorId,
    int? AppointmentId = null,
    DateTime? VisitDate = null,
    string? ChiefComplaint = null,
    string? Symptoms = null,
    string? Diagnosis = null,
    string? Treatment = null,
    string? VitalSigns = null,
    string? PhysicalExamination = null,
    string? LabResults = null,
    string? Recommendations = null,
    string? FollowUpInstructions = null,
    DateTime? NextVisitDate = null
);

public record UpdateMedicalRecordDto(
    string? ChiefComplaint = null,
    string? Symptoms = null,
    string? Diagnosis = null,
    string? Treatment = null,
    string? VitalSigns = null,
    string? PhysicalExamination = null,
    string? LabResults = null,
    string? Recommendations = null,
    string? FollowUpInstructions = null,
    DateTime? NextVisitDate = null
);
