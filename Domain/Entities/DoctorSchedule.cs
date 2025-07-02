using ClinicApi.Domain.Common;

namespace ClinicApi.Domain.Entities;

public class DoctorSchedule : BaseEntity
{
    public int DoctorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public TimeSpan? BreakStartTime { get; set; }
    public TimeSpan? BreakEndTime { get; set; }
    public bool IsAvailable { get; set; } = true;

    // Navigation Properties
    public virtual Doctor Doctor { get; set; } = null!;

    // Domain methods
    public bool IsWorkingDay() => IsAvailable;

    public TimeSpan GetWorkingHours()
    {
        var totalHours = EndTime - StartTime;
        if (BreakStartTime.HasValue && BreakEndTime.HasValue)
        {
            var breakDuration = BreakEndTime.Value - BreakStartTime.Value;
            totalHours -= breakDuration;
        }
        return totalHours;
    }

    public bool IsWithinWorkingHours(TimeSpan time)
    {
        if (!IsAvailable)
            return false;
        if (time < StartTime || time > EndTime)
            return false;

        if (BreakStartTime.HasValue && BreakEndTime.HasValue)
        {
            return !(time >= BreakStartTime.Value && time < BreakEndTime.Value);
        }

        return true;
    }

    public bool CanScheduleAppointment(TimeSpan startTime, TimeSpan duration)
    {
        if (!IsAvailable)
            return false;

        var endTime = startTime.Add(duration);
        if (startTime < StartTime || endTime > EndTime)
            return false;

        if (BreakStartTime.HasValue && BreakEndTime.HasValue)
        {
            // Check if appointment overlaps with break time
            var breakStart = BreakStartTime.Value;
            var breakEnd = BreakEndTime.Value;

            return !(startTime < breakEnd && endTime > breakStart);
        }

        return true;
    }
}
