using ClinicApi.Business.Domain.Interfaces;
using ClinicApi.Business.Domain.Models;
using ClinicApi.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Infrastructure.Data.Repositories;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(ClinicDbContext context)
        : base(context) { }

    public async Task<Patient?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Email == email);
    }

    public async Task<IEnumerable<Patient>> SearchAsync(string searchTerm)
    {
        var search = searchTerm.ToLower();
        return await _dbSet
            .Where(p =>
                p.FirstName.ToLower().Contains(search)
                || p.LastName.ToLower().Contains(search)
                || p.Email.ToLower().Contains(search)
                || p.PhoneNumber.Contains(search)
            )
            .ToListAsync();
    }

    public async Task<IEnumerable<Patient>> GetActiveAsync()
    {
        return await _dbSet.Where(p => p.IsActive).ToListAsync();
    }

    public async Task<Patient?> GetWithAppointmentsAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Appointments)
            .ThenInclude(a => a.Doctor)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Patient?> GetWithMedicalRecordsAsync(int id)
    {
        return await _dbSet
            .Include(p => p.MedicalRecords)
            .ThenInclude(mr => mr.Doctor)
            .Include(p => p.Prescriptions)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
