using ClinicApi.Domain.Enums;

namespace ClinicApi.Application.DTOs;

public record AppointmentResponseDto(
    int Id,
    int PatientId,
    int DoctorId,
    DateTime AppointmentDate,
    TimeSpan Duration,
    AppointmentStatus Status,
    string? ReasonForVisit,
    string? Notes,
    string? CancellationReason,
    decimal? ConsultationFee,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    PatientResponseDto? Patient = null,
    DoctorResponseDto? Doctor = null
);

public record CreateAppointmentDto(
    int PatientId,
    int DoctorId,
    DateTime AppointmentDate,
    TimeSpan Duration,
    string? ReasonForVisit = null,
    string? Notes = null
);

public record UpdateAppointmentDto(
    DateTime? AppointmentDate = null,
    TimeSpan? Duration = null,
    AppointmentStatus? Status = null,
    string? ReasonForVisit = null,
    string? Notes = null,
    string? CancellationReason = null,
    decimal? ConsultationFee = null
);

public record AppointmentFilterDto(
    int? PatientId = null,
    int? DoctorId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    AppointmentStatus? Status = null
); 