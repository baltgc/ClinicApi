using ClinicApi.Domain.Enums;

namespace ClinicApi.Domain.Exceptions;

public class InvalidAppointmentStatusException : DomainException
{
    public AppointmentStatus CurrentStatus { get; }
    public AppointmentStatus AttemptedStatus { get; }

    public InvalidAppointmentStatusException(
        AppointmentStatus currentStatus,
        AppointmentStatus attemptedStatus
    )
        : base($"Cannot transition appointment from {currentStatus} to {attemptedStatus}")
    {
        CurrentStatus = currentStatus;
        AttemptedStatus = attemptedStatus;
    }
}
