using ClinicApi.Business.Domain.Models;

namespace ClinicApi.Business.Domain.Interfaces;

public interface IPatientRepository : IGenericRepository<Patient>
{
    Task<Patient?> GetByEmailAsync(string email);
    Task<IEnumerable<Patient>> SearchAsync(string searchTerm);
    Task<IEnumerable<Patient>> GetActiveAsync();
    Task<Patient?> GetWithAppointmentsAsync(int id);
    Task<Patient?> GetWithMedicalRecordsAsync(int id);
}
