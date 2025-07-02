using ClinicApi.Application.DTOs;
using MediatR;

namespace ClinicApi.Application.Queries.Appointments;

public record GetAllAppointmentsQuery : IRequest<IEnumerable<AppointmentResponseDto>>;
