using ClinicApi.Domain.Entities;

namespace ClinicApi.Domain.Services;

public interface IPatientDomainService
{
    bool IsEligibleForDiscount(Patient patient);
    decimal CalculateAge(Patient patient);
    bool RequiresGuardianConsent(Patient patient);
    string GeneratePatientNumber();
}
