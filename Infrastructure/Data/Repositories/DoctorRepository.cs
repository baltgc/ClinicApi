using ClinicApi.Business.Domain.Interfaces;
using ClinicApi.Business.Domain.Models;
using ClinicApi.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Infrastructure.Data.Repositories;

public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
{
    public DoctorRepository(ClinicDbContext context)
        : base(context) { }

    public async Task<Doctor?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.Email == email);
    }

    public async Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber);
    }

    public async Task<IEnumerable<Doctor>> GetBySpecializationAsync(string specialization)
    {
        return await _dbSet
            .Where(d => d.Specialization.ToLower() == specialization.ToLower() && d.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Doctor>> SearchAsync(string searchTerm)
    {
        var search = searchTerm.ToLower();
        return await _dbSet
            .Where(d =>
                d.FirstName.ToLower().Contains(search)
                || d.LastName.ToLower().Contains(search)
                || d.Specialization.ToLower().Contains(search)
                || d.Email.ToLower().Contains(search)
            )
            .ToListAsync();
    }

    public async Task<IEnumerable<Doctor>> GetActiveAsync()
    {
        return await _dbSet.Where(d => d.IsActive).ToListAsync();
    }

    public async Task<Doctor?> GetWithSchedulesAsync(int id)
    {
        return await _dbSet.Include(d => d.Schedules).FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Doctor?> GetWithAppointmentsAsync(int id)
    {
        return await _dbSet
            .Include(d => d.Appointments)
            .ThenInclude(a => a.Patient)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
}
