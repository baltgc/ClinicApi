using MediatR;

namespace ClinicApi.Application.Commands.Appointments;

public record DeleteAppointmentCommand(int Id) : IRequest<bool>; 