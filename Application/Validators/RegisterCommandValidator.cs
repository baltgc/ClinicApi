using ClinicApi.Application.Commands.Auth;
using ClinicApi.Domain.Enums;
using FluentValidation;

namespace ClinicApi.Application.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
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

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(128)
            .WithMessage("Password cannot exceed 128 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
            .WithMessage(
                "Password must contain at least one lowercase letter, one uppercase letter, and one number."
            );

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^\+?[\d\s\-\(\)]+$")
            .WithMessage("Invalid phone number format.")
            .MaximumLength(20)
            .WithMessage("Phone number cannot exceed 20 characters.");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required.")
            .Must(role =>
                new[]
                {
                    ClinicRoles.Admin,
                    ClinicRoles.Doctor,
                    ClinicRoles.Patient,
                    ClinicRoles.Nurse,
                    ClinicRoles.Receptionist,
                    ClinicRoles.Manager,
                }.Contains(role)
            )
            .WithMessage("Invalid role specified.");

        RuleFor(x => x.Gender)
            .Must(g => string.IsNullOrEmpty(g) || new[] { "Male", "Female", "Other" }.Contains(g))
            .WithMessage("Gender must be Male, Female, or Other.")
            .When(x => !string.IsNullOrEmpty(x.Gender));

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .WithMessage("Address cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Address));

        RuleFor(x => x.Department)
            .MaximumLength(100)
            .WithMessage("Department cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Department));

        RuleFor(x => x.EmployeeId)
            .MaximumLength(50)
            .WithMessage("Employee ID cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.EmployeeId));
    }
}
