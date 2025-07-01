using System.ComponentModel.DataAnnotations;

namespace ClinicApi.Business.Domain.Models;

public class Doctor
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
    [StringLength(100)]
    public string Specialization { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Qualifications { get; set; }

    public int YearsOfExperience { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    [StringLength(500)]
    public string? OfficeAddress { get; set; }

    public decimal ConsultationFee { get; set; }

    [StringLength(100)]
    public string? AvailableHours { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } =
        new List<MedicalRecord>();
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public virtual ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
}
