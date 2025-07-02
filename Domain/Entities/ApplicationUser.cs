using Microsoft.AspNetCore.Identity;

namespace ClinicApi.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumberSecondary { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? Department { get; set; }
    public string? EmployeeId { get; set; }
    public int? PatientId { get; set; }
    public int? DoctorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public virtual Patient? Patient { get; set; }
    public virtual Doctor? Doctor { get; set; }

    // Domain methods
    public string GetFullName() => $"{FirstName} {LastName}";

    public bool IsStaff() => !string.IsNullOrEmpty(EmployeeId);

    public bool IsPatientUser() => PatientId.HasValue;

    public bool IsDoctorUser() => DoctorId.HasValue;
}
