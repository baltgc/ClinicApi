using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Business.Domain.Models;

public class Appointment
{
    public int Id { get; set; }

    [Required]
    public int PatientId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    public TimeSpan Duration { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Scheduled"; // Scheduled, Confirmed, InProgress, Completed, Cancelled

    [StringLength(100)]
    public string? ReasonForVisit { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [StringLength(500)]
    public string? CancellationReason { get; set; }

    public decimal? ConsultationFee { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } =
        new List<MedicalRecord>();
}
