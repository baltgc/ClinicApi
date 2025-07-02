using ClinicApi.Application.Commands.Appointments;
using FluentValidation;

namespace ClinicApi.Application.Validators;

public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0)
            .WithMessage("Patient ID must be a positive number.");

        RuleFor(x => x.DoctorId).GreaterThan(0).WithMessage("Doctor ID must be a positive number.");

        RuleFor(x => x.AppointmentDate)
            .NotEmpty()
            .WithMessage("Appointment date is required.")
            .GreaterThan(DateTime.UtcNow.AddMinutes(30))
            .WithMessage("Appointment must be scheduled at least 30 minutes in advance.");

        RuleFor(x => x.Duration)
            .NotEmpty()
            .WithMessage("Duration is required.")
            .Must(d => d.TotalMinutes >= 15)
            .WithMessage("Appointment duration must be at least 15 minutes.")
            .Must(d => d.TotalMinutes <= 480)
            .WithMessage("Appointment duration cannot exceed 8 hours.")
            .Must(d => d.TotalMinutes % 15 == 0)
            .WithMessage("Appointment duration must be in 15-minute increments.");

        RuleFor(x => x.ReasonForVisit)
            .MaximumLength(100)
            .WithMessage("Reason for visit cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.ReasonForVisit));

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}
