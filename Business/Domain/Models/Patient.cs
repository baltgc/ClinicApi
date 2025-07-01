using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Business.Domain.Models;

public class Patient
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [StringLength(10)]
    public string Gender { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? BloodType { get; set; }

    [StringLength(1000)]
    public string? MedicalHistory { get; set; }

    [StringLength(500)]
    public string? Allergies { get; set; }

    [StringLength(500)]
    public string? EmergencyContact { get; set; }

    [StringLength(20)]
    public string? EmergencyContactPhone { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } =
        new List<MedicalRecord>();
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
