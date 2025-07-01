using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Business.Domain.Models;

public class DoctorSchedule
{
    public int Id { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    public TimeSpan? BreakStartTime { get; set; }

    public TimeSpan? BreakEndTime { get; set; }

    public int SlotDurationMinutes { get; set; } = 30;

    public bool IsAvailable { get; set; } = true;

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    public virtual Doctor Doctor { get; set; } = null!;
}
