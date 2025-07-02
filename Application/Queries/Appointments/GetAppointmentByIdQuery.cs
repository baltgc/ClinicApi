using ClinicApi.Application.DTOs;
using MediatR;

namespace ClinicApi.Application.Queries.Appointments;

public record GetAppointmentByIdQuery(int Id) : IRequest<AppointmentResponseDto?>;
