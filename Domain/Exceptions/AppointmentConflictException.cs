namespace ClinicApi.Domain.Exceptions;

public class AppointmentConflictException : DomainException
{
    public int DoctorId { get; }
    public DateTime AppointmentDate { get; }

    public AppointmentConflictException(int doctorId, DateTime appointmentDate)
        : base($"Doctor {doctorId} is not available at {appointmentDate}")
    {
        DoctorId = doctorId;
        AppointmentDate = appointmentDate;
    }

    public AppointmentConflictException(int doctorId, DateTime appointmentDate, string message)
        : base(message)
    {
        DoctorId = doctorId;
        AppointmentDate = appointmentDate;
    }
}
