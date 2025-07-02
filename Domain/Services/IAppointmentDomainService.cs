using ClinicApi.Domain.Entities;

namespace ClinicApi.Domain.Services;

public interface IAppointmentDomainService
{
    Task<bool> CanScheduleAppointmentAsync(
        int doctorId,
        DateTime appointmentDate,
        TimeSpan duration
    );
    decimal CalculateConsultationFee(
        Doctor doctor,
        TimeSpan duration,
        string appointmentType = "Regular"
    );
    bool CanCancelAppointment(Appointment appointment, DateTime cancellationTime);
    Task<bool> IsDoctorAvailableForEmergencyAsync(int doctorId, DateTime requestedTime);
    Task<DateTime?> GetNextAvailableSlotAsync(int doctorId, TimeSpan duration, DateTime afterDate);
}
