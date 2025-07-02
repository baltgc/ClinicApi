using ClinicApi.Domain.Entities;

namespace ClinicApi.Domain.Services;

public class PatientDomainService : IPatientDomainService
{
    private static readonly Random _random = new();

    public bool IsEligibleForDiscount(Patient patient)
    {
        // Business rules for discounts
        var age = CalculateAge(patient);

        // Senior citizen discount (65+)
        if (age >= 65)
            return true;

        // Student discount (18-25)
        if (age >= 18 && age <= 25)
            return true;

        // Child discount (under 12)
        if (age < 12)
            return true;

        return false;
    }

    public decimal CalculateAge(Patient patient)
    {
        var today = DateTime.Today;
        var age = today.Year - patient.DateOfBirth.Year;
        if (patient.DateOfBirth.Date > today.AddYears(-age))
            age--;
        return age;
    }

    public bool RequiresGuardianConsent(Patient patient)
    {
        return CalculateAge(patient) < 18;
    }

    public string GeneratePatientNumber()
    {
        var year = DateTime.Now.Year.ToString();
        var randomNumber = _random.Next(1000, 9999);
        return $"PT{year}{randomNumber}";
    }
}
