using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Interfaces;
using ClinicApi.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Infrastructure.Data.Repositories;

public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
{
    public DoctorRepository(ClinicDbContext context)
        : base(context) { }

    public async Task<Doctor?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.Email.ToLower() == email.ToLower());
    }

    public async Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber);
    }

    public async Task<Doctor?> GetWithSchedulesAsync(int id)
    {
        return await _dbSet.Include(d => d.Schedules).FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Doctor>> GetBySpecializationAsync(string specialization)
    {
        return await _dbSet
            .Where(d => d.IsActive && d.Specialization.ToLower() == specialization.ToLower())
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Doctor>> GetActiveAsync()
    {
        return await _dbSet
            .Where(d => d.IsActive)
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var query = _dbSet.Where(d => d.Email.ToLower() == email.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(d => d.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> LicenseNumberExistsAsync(string licenseNumber, int? excludeId = null)
    {
        var query = _dbSet.Where(d => d.LicenseNumber == licenseNumber);

        if (excludeId.HasValue)
        {
            query = query.Where(d => d.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
