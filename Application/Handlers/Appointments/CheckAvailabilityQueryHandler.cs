using ClinicApi.Application.Queries.Appointments;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Appointments;

public class CheckAvailabilityQueryHandler : IRequestHandler<CheckAvailabilityQuery, bool>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public CheckAvailabilityQueryHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<bool> Handle(
        CheckAvailabilityQuery request,
        CancellationToken cancellationToken
    )
    {
        var duration = TimeSpan.FromMinutes(request.DurationMinutes);
        var hasConflict = await _appointmentRepository.HasConflictAsync(
            request.DoctorId,
            request.AppointmentDate,
            duration
        );

        return !hasConflict; // Return true if available (no conflict)
    }
}
