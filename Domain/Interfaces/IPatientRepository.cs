using ClinicApi.Domain.Entities;

namespace ClinicApi.Domain.Interfaces;

public interface IPatientRepository : IGenericRepository<Patient>
{
    Task<Patient?> GetByEmailAsync(string email);
    Task<IEnumerable<Patient>> GetActiveAsync();
    Task<IEnumerable<Patient>> SearchAsync(string searchTerm);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    Task<IEnumerable<Patient>> GetActivePatientsAsync();
    Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm);
}
