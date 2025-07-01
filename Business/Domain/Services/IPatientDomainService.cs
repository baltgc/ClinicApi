using ClinicApi.Business.Domain.Models;

namespace ClinicApi.Business.Domain.Services;

/// <summary>
/// Domain service for complex patient business rules
/// </summary>
public interface IPatientDomainService
{
    /// <summary>
    /// Validates if a patient is eligible for a specific type of appointment
    /// </summary>
    bool IsEligibleForAppointmentType(Patient patient, string appointmentType);

    /// <summary>
    /// Calculates patient risk level based on medical history and age
    /// </summary>
    string CalculateRiskLevel(Patient patient);

    /// <summary>
    /// Determines if patient requires special accommodations
    /// </summary>
    List<string> GetRequiredAccommodations(Patient patient);

    /// <summary>
    /// Validates if patient can have appointments with multiple doctors on same day
    /// </summary>
    bool CanHaveMultipleAppointmentsSameDay(
        Patient patient,
        List<Appointment> existingAppointments
    );

    /// <summary>
    /// Calculates recommended follow-up interval based on patient condition
    /// </summary>
    TimeSpan GetRecommendedFollowUpInterval(Patient patient, string diagnosisType);
}
