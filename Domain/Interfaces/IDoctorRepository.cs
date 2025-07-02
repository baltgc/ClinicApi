using ClinicApi.Domain.Entities;

namespace ClinicApi.Domain.Interfaces;

public interface IDoctorRepository : IGenericRepository<Doctor>
{
    Task<Doctor?> GetByEmailAsync(string email);
    Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber);
    Task<Doctor?> GetWithSchedulesAsync(int id);
    Task<IEnumerable<Doctor>> GetBySpecializationAsync(string specialization);
    Task<IEnumerable<Doctor>> GetActiveAsync();
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    Task<bool> LicenseNumberExistsAsync(string licenseNumber, int? excludeId = null);
}
