using MediatR;

namespace ClinicApi.Application.Queries.Appointments;

public record CheckAvailabilityQuery(int DoctorId, DateTime AppointmentDate, int DurationMinutes)
    : IRequest<bool>;
