using ClinicApi.Application.DTOs;
using MediatR;

namespace ClinicApi.Application.Commands.Appointments;

public record UpdateAppointmentCommand(
    int Id,
    DateTime? AppointmentDate = null,
    TimeSpan? Duration = null,
    string? ReasonForVisit = null,
    string? Notes = null
) : IRequest<AppointmentResponseDto?>;
