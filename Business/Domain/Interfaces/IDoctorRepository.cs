using ClinicApi.Business.Domain.Models;

namespace ClinicApi.Business.Domain.Interfaces;

public interface IDoctorRepository : IGenericRepository<Doctor>
{
    Task<Doctor?> GetByEmailAsync(string email);
    Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber);
    Task<IEnumerable<Doctor>> GetBySpecializationAsync(string specialization);
    Task<IEnumerable<Doctor>> SearchAsync(string searchTerm);
    Task<IEnumerable<Doctor>> GetActiveAsync();
    Task<Doctor?> GetWithSchedulesAsync(int id);
    Task<Doctor?> GetWithAppointmentsAsync(int id);
}
