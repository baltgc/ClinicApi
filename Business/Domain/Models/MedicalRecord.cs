using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Business.Domain.Models;

public class MedicalRecord
{
    public int Id { get; set; }

    [Required]
    public int PatientId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    public int? AppointmentId { get; set; }

    [Required]
    public DateTime VisitDate { get; set; }

    [StringLength(100)]
    public string? ChiefComplaint { get; set; }

    [StringLength(1000)]
    public string? Symptoms { get; set; }

    [StringLength(1000)]
    public string? Diagnosis { get; set; }

    [StringLength(1000)]
    public string? Treatment { get; set; }

    [StringLength(500)]
    public string? VitalSigns { get; set; }

    [StringLength(1000)]
    public string? PhysicalExamination { get; set; }

    [StringLength(1000)]
    public string? LabResults { get; set; }

    [StringLength(1000)]
    public string? Recommendations { get; set; }

    [StringLength(500)]
    public string? FollowUpInstructions { get; set; }

    public DateTime? NextVisitDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual Appointment? Appointment { get; set; }
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
