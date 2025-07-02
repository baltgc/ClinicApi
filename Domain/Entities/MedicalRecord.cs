using ClinicApi.Domain.Common;

namespace ClinicApi.Domain.Entities;

public class MedicalRecord : BaseEntity
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int? AppointmentId { get; set; }
    public DateTime VisitDate { get; set; }
    public string? ChiefComplaint { get; set; }
    public string? Symptoms { get; set; }
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    public string? VitalSigns { get; set; }
    public string? PhysicalExamination { get; set; }
    public string? LabResults { get; set; }
    public string? Recommendations { get; set; }
    public string? FollowUpInstructions { get; set; }

    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual Appointment? Appointment { get; set; }
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    // Domain methods
    public bool HasDiagnosis() => !string.IsNullOrWhiteSpace(Diagnosis);

    public bool HasPrescriptions() => Prescriptions.Any();

    public bool RequiresFollowUp() => !string.IsNullOrWhiteSpace(FollowUpInstructions);
}
