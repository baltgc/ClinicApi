using ClinicApi.Application.DTOs;
using ClinicApi.Domain.Enums;
using MediatR;

namespace ClinicApi.Application.Commands.Appointments;

public record UpdateAppointmentCommand(
    int Id,
    DateTime? AppointmentDate = null,
    TimeSpan? Duration = null,
    AppointmentStatus? Status = null,
    string? ReasonForVisit = null,
    string? Notes = null,
    string? CancellationReason = null,
    decimal? ConsultationFee = null
) : IRequest<AppointmentResponseDto?>; 