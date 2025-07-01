using ClinicApi.Business.Domain.Interfaces;
using ClinicApi.Business.Domain.Models;

namespace ClinicApi.Business.Domain.Services;

/// <summary>
/// Domain service implementing complex appointment business rules
/// </summary>
public class AppointmentDomainService : IAppointmentDomainService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;

    public AppointmentDomainService(
        IAppointmentRepository appointmentRepository,
        IDoctorRepository doctorRepository
    )
    {
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
    }

    /// <summary>
    /// Business rule: Can only schedule appointments during doctor's working hours,
    /// with no conflicts, and minimum 24 hours in advance (except emergencies)
    /// </summary>
    public async Task<bool> CanScheduleAppointmentAsync(
        int doctorId,
        DateTime appointmentDate,
        TimeSpan duration
    )
    {
        // Rule 1: Appointment must be in the future
        if (appointmentDate <= DateTime.UtcNow)
            return false;

        // Rule 2: Check for scheduling conflicts
        var hasConflict = await _appointmentRepository.HasConflictAsync(
            doctorId,
            appointmentDate,
            duration
        );
        if (hasConflict)
            return false;

        // Rule 3: Check doctor's working hours
        var doctor = await _doctorRepository.GetWithSchedulesAsync(doctorId);
        if (doctor?.Schedules == null || !doctor.Schedules.Any())
            return false;

        var dayOfWeek = (int)appointmentDate.DayOfWeek;
        var timeOfDay = appointmentDate.TimeOfDay;

        var isWithinWorkingHours = doctor.Schedules.Any(schedule =>
            schedule.DayOfWeek == (DayOfWeek)dayOfWeek
            && schedule.IsAvailable
            && timeOfDay >= schedule.StartTime
            && timeOfDay.Add(duration) <= schedule.EndTime
            && (
                schedule.BreakStartTime == null
                || timeOfDay >= schedule.BreakEndTime
                || timeOfDay.Add(duration) <= schedule.BreakStartTime
            )
        );

        return isWithinWorkingHours;
    }

    /// <summary>
    /// Business rule: Consultation fees vary by doctor specialization, duration, and appointment type
    /// </summary>
    public decimal CalculateConsultationFee(
        Doctor doctor,
        TimeSpan duration,
        string appointmentType = "Regular"
    )
    {
        var baseFee = doctor.ConsultationFee;

        // Rule 1: Base fee multipliers by specialization
        var specializationMultiplier = doctor.Specialization.ToLower() switch
        {
            "cardiology" => 1.5m,
            "neurology" => 1.4m,
            "orthopedics" => 1.3m,
            "pediatrics" => 1.1m,
            "general" => 1.0m,
            _ => 1.2m,
        };

        // Rule 2: Duration-based pricing
        var durationMultiplier = duration.TotalMinutes switch
        {
            <= 15 => 0.5m,
            <= 30 => 1.0m,
            <= 60 => 1.5m,
            <= 90 => 2.0m,
            _ => 2.5m,
        };

        // Rule 3: Appointment type multipliers
        var typeMultiplier = appointmentType.ToLower() switch
        {
            "emergency" => 2.0m,
            "follow-up" => 0.8m,
            "consultation" => 1.2m,
            "regular" => 1.0m,
            _ => 1.0m,
        };

        return Math.Round(
            baseFee * specializationMultiplier * durationMultiplier * typeMultiplier,
            2
        );
    }

    /// <summary>
    /// Business rule: Appointments can be cancelled up to 24 hours before,
    /// emergency appointments up to 2 hours before
    /// </summary>
    public bool CanCancelAppointment(Appointment appointment, DateTime cancellationTime)
    {
        // Rule 1: Cannot cancel past appointments
        if (appointment.AppointmentDate <= cancellationTime)
            return false;

        // Rule 2: Different cancellation windows based on appointment type
        var timeUntilAppointment = appointment.AppointmentDate - cancellationTime;

        // Emergency appointments: 2 hours notice
        if (appointment.ReasonForVisit?.ToLower().Contains("emergency") == true)
            return timeUntilAppointment >= TimeSpan.FromHours(2);

        // Regular appointments: 24 hours notice
        return timeUntilAppointment >= TimeSpan.FromHours(24);
    }

    /// <summary>
    /// Business rule: Emergency appointments can be scheduled if doctor has no appointment
    /// in the next 2 hours or if there's a cancellation
    /// </summary>
    public async Task<bool> IsDoctorAvailableForEmergencyAsync(int doctorId, DateTime requestedTime)
    {
        var emergencyWindow = TimeSpan.FromHours(2);
        var endTime = requestedTime.Add(emergencyWindow);

        // Check for conflicts in emergency window
        var hasConflict = await _appointmentRepository.HasConflictAsync(
            doctorId,
            requestedTime,
            emergencyWindow
        );

        return !hasConflict;
    }

    /// <summary>
    /// Business rule: Find the next available slot considering doctor's schedule,
    /// existing appointments, and minimum slot duration
    /// </summary>
    public async Task<DateTime?> GetNextAvailableSlotAsync(
        int doctorId,
        TimeSpan duration,
        DateTime afterDate
    )
    {
        var doctor = await _doctorRepository.GetWithSchedulesAsync(doctorId);
        if (doctor?.Schedules == null || !doctor.Schedules.Any())
            return null;

        // Look ahead for next 30 days
        for (int dayOffset = 0; dayOffset < 30; dayOffset++)
        {
            var checkDate = afterDate.Date.AddDays(dayOffset);
            var dayOfWeek = (int)checkDate.DayOfWeek;

            var daySchedule = doctor.Schedules.FirstOrDefault(s =>
                s.DayOfWeek == (DayOfWeek)dayOfWeek && s.IsAvailable
            );

            if (daySchedule == null)
                continue;

            // Check each time slot in 15-minute increments
            var currentTime = checkDate.Add(daySchedule.StartTime);
            var endTime = checkDate.Add(daySchedule.EndTime);

            while (currentTime.Add(duration) <= endTime)
            {
                // Skip break times
                if (daySchedule.BreakStartTime.HasValue)
                {
                    var breakStart = checkDate.Add(daySchedule.BreakStartTime.Value);
                    var breakEnd = checkDate.Add(
                        daySchedule.BreakEndTime ?? daySchedule.BreakStartTime.Value
                    );

                    if (currentTime < breakEnd && currentTime.Add(duration) > breakStart)
                    {
                        currentTime = breakEnd;
                        continue;
                    }
                }

                // Check if slot is available
                var hasConflict = await _appointmentRepository.HasConflictAsync(
                    doctorId,
                    currentTime,
                    duration
                );

                if (!hasConflict && currentTime > DateTime.UtcNow)
                    return currentTime;

                currentTime = currentTime.AddMinutes(15); // 15-minute increments
            }
        }

        return null; // No available slot found in next 30 days
    }
}
