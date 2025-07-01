using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ClinicApi.Business.Domain.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PhoneNumberSecondary { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? Department { get; set; }

    [StringLength(50)]
    public string? EmployeeId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties for clinic entities
    public int? PatientId { get; set; }
    public virtual Patient? Patient { get; set; }

    public int? DoctorId { get; set; }
    public virtual Doctor? Doctor { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
