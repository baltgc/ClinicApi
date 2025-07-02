using ClinicApi.Domain.Common;
using ClinicApi.Domain.Enums;

namespace ClinicApi.Domain.Entities;

public class Prescription : BaseEntity
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int? MedicalRecordId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public DateTime PrescribedDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Instructions { get; set; }
    public string? SideEffects { get; set; }
    public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Active;

    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual MedicalRecord? MedicalRecord { get; set; }

    // Domain methods
    public bool IsActive() => Status == PrescriptionStatus.Active && EndDate >= DateTime.UtcNow;

    public bool IsExpired() => EndDate < DateTime.UtcNow;

    public int GetRemainingDays()
    {
        if (IsExpired())
            return 0;
        return (int)(EndDate - DateTime.UtcNow).TotalDays;
    }

    public void Complete()
    {
        Status = PrescriptionStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        Status = PrescriptionStatus.Cancelled;
        // Note: Add cancellation reason property if needed
        UpdatedAt = DateTime.UtcNow;
    }
}
