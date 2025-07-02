using ClinicApi.Application.Commands.Patients;
using FluentValidation;

namespace ClinicApi.Application.Validators;

public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(100)
            .WithMessage("First name cannot exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(100)
            .WithMessage("Last name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^\+?[\d\s\-\(\)]+$")
            .WithMessage("Invalid phone number format.")
            .MaximumLength(20)
            .WithMessage("Phone number cannot exceed 20 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth is required.")
            .LessThan(DateTime.Today)
            .WithMessage("Date of birth must be in the past.")
            .GreaterThan(DateTime.Today.AddYears(-150))
            .WithMessage("Date of birth cannot be more than 150 years ago.");

        RuleFor(x => x.Gender)
            .NotEmpty()
            .WithMessage("Gender is required.")
            .Must(g => new[] { "Male", "Female", "Other" }.Contains(g))
            .WithMessage("Gender must be Male, Female, or Other.");

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .WithMessage("Address cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Address));

        RuleFor(x => x.BloodType)
            .Must(bt => new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" }.Contains(bt))
            .WithMessage("Invalid blood type.")
            .When(x => !string.IsNullOrEmpty(x.BloodType));

        RuleFor(x => x.MedicalHistory)
            .MaximumLength(1000)
            .WithMessage("Medical history cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.MedicalHistory));

        RuleFor(x => x.Allergies)
            .MaximumLength(500)
            .WithMessage("Allergies cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Allergies));

        RuleFor(x => x.EmergencyContact)
            .MaximumLength(500)
            .WithMessage("Emergency contact cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.EmergencyContact));

        RuleFor(x => x.EmergencyContactPhone)
            .Matches(@"^\+?[\d\s\-\(\)]+$")
            .WithMessage("Invalid emergency contact phone format.")
            .MaximumLength(20)
            .WithMessage("Emergency contact phone cannot exceed 20 characters.")
            .When(x => !string.IsNullOrEmpty(x.EmergencyContactPhone));
    }
}
