using ClinicApi.Business.Domain.Models;

namespace ClinicApi.Business.Domain.Services;

/// <summary>
/// Domain service for complex appointment business rules
/// </summary>
public interface IAppointmentDomainService
{
    /// <summary>
    /// Validates if an appointment can be scheduled based on business rules
    /// </summary>
    Task<bool> CanScheduleAppointmentAsync(
        int doctorId,
        DateTime appointmentDate,
        TimeSpan duration
    );

    /// <summary>
    /// Calculates consultation fee based on doctor, appointment type, and duration
    /// </summary>
    decimal CalculateConsultationFee(
        Doctor doctor,
        TimeSpan duration,
        string appointmentType = "Regular"
    );

    /// <summary>
    /// Validates if an appointment can be cancelled based on business rules
    /// </summary>
    bool CanCancelAppointment(Appointment appointment, DateTime cancellationTime);

    /// <summary>
    /// Determines if a doctor is available for emergency appointments
    /// </summary>
    Task<bool> IsDoctorAvailableForEmergencyAsync(int doctorId, DateTime requestedTime);

    /// <summary>
    /// Calculates the earliest available slot for a doctor
    /// </summary>
    Task<DateTime?> GetNextAvailableSlotAsync(int doctorId, TimeSpan duration, DateTime afterDate);
}
