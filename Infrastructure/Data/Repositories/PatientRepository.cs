using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Interfaces;
using ClinicApi.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Infrastructure.Data.Repositories;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(ClinicDbContext context)
        : base(context) { }

    public async Task<Patient?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<Patient>> GetActiveAsync()
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Patient>> SearchAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();

        return await _dbSet
            .Where(p =>
                p.IsActive
                && (
                    p.FirstName.ToLower().Contains(lowerSearchTerm)
                    || p.LastName.ToLower().Contains(lowerSearchTerm)
                    || p.Email.ToLower().Contains(lowerSearchTerm)
                    || p.PhoneNumber.Contains(searchTerm)
                )
            )
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var query = _dbSet.Where(p => p.Email.ToLower() == email.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
