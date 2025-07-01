using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Business.Domain.Models;

public class Prescription
{
    public int Id { get; set; }

    [Required]
    public int PatientId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    public int? MedicalRecordId { get; set; }

    [Required]
    [StringLength(200)]
    public string MedicationName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Dosage { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Frequency { get; set; } = string.Empty;

    [Required]
    public int DurationDays { get; set; }

    [StringLength(500)]
    public string? Instructions { get; set; }

    [StringLength(500)]
    public string? SideEffects { get; set; }

    public DateTime PrescribedDate { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = "Active"; // Active, Completed, Discontinued

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual MedicalRecord? MedicalRecord { get; set; }
}
