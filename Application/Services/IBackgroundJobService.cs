namespace ClinicApi.Application.Services;

/// <summary>
/// Interface for background job services
/// </summary>
public interface IBackgroundJobService
{
    /// <summary>
    /// Schedules an appointment reminder job
    /// </summary>
    /// <param name="appointmentId">The appointment ID</param>
    /// <param name="scheduledTime">When to send the reminder</param>
    /// <returns>Job ID</returns>
    string ScheduleAppointmentReminder(int appointmentId, DateTime scheduledTime);

    /// <summary>
    /// Sends appointment confirmation email
    /// </summary>
    /// <param name="appointmentId">The appointment ID</param>
    Task SendAppointmentConfirmationAsync(int appointmentId);

    /// <summary>
    /// Processes patient data cleanup (recurring job)
    /// </summary>
    Task ProcessPatientDataCleanupAsync();

    /// <summary>
    /// Sends daily appointment summary to doctors
    /// </summary>
    Task SendDailyAppointmentSummaryAsync();
}
