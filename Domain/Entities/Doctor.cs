using ClinicApi.Domain.Common;

namespace ClinicApi.Domain.Entities;

public class Doctor : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Qualifications { get; set; }
    public string? Bio { get; set; }
    public string? OfficeAddress { get; set; }
    public decimal ConsultationFee { get; set; }
    public int ExperienceYears { get; set; }
    public string? AvailableHours { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } =
        new List<MedicalRecord>();
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public virtual ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();

    // Domain methods
    public string GetFullName() => $"{FirstName} {LastName}";

    public string GetDisplayName() => $"Dr. {GetFullName()}";

    public bool IsAvailableOnDay(DayOfWeek dayOfWeek)
    {
        return Schedules.Any(s => s.DayOfWeek == dayOfWeek && s.IsAvailable);
    }
}
