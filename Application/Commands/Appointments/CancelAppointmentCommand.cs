using MediatR;

namespace ClinicApi.Application.Commands.Appointments;

public record CancelAppointmentCommand(
    int Id,
    string Reason
) : IRequest<bool>; 