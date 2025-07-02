using ClinicApi.Application.Commands.Appointments;
using ClinicApi.Domain.Enums;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Appointments;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, bool>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public CancelAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<bool> Handle(
        CancelAppointmentCommand request,
        CancellationToken cancellationToken
    )
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.Id);
        if (appointment == null || appointment.Status == AppointmentStatus.Cancelled)
            return false;

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.CancellationReason = request.Reason;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _appointmentRepository.UpdateAsync(appointment);
        return true;
    }
}
