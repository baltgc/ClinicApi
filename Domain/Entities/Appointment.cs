using ClinicApi.Domain.Common;
using ClinicApi.Domain.Enums;

namespace ClinicApi.Domain.Entities;

public class Appointment : BaseEntity
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan Duration { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? ReasonForVisit { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public decimal? ConsultationFee { get; set; }

    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } =
        new List<MedicalRecord>();

    // Domain methods
    public DateTime GetEndTime() => AppointmentDate.Add(Duration);

    public bool IsUpcoming() => AppointmentDate > DateTime.UtcNow;

    public bool IsPast() => AppointmentDate < DateTime.UtcNow;

    public bool CanBeCancelled()
    {
        return Status == AppointmentStatus.Scheduled || Status == AppointmentStatus.Confirmed;
    }

    public bool IsOnSameDay(DateTime date)
    {
        return AppointmentDate.Date == date.Date;
    }

    public void Cancel(string reason)
    {
        if (!CanBeCancelled())
            throw new InvalidOperationException(
                "Appointment cannot be cancelled in its current state."
            );

        Status = AppointmentStatus.Cancelled;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled appointments can be confirmed.");

        Status = AppointmentStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }
}
