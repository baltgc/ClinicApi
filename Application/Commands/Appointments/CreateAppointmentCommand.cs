using ClinicApi.Application.DTOs;
using MediatR;

namespace ClinicApi.Application.Commands.Appointments;

public record CreateAppointmentCommand(
    int PatientId,
    int DoctorId,
    DateTime AppointmentDate,
    TimeSpan Duration,
    string? ReasonForVisit = null,
    string? Notes = null
) : IRequest<AppointmentResponseDto>;
