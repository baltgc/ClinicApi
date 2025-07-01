using ClinicApi.Business.Domain.Interfaces;
using ClinicApi.Business.Domain.Models;
using ClinicApi.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Infrastructure.Data.Repositories;

public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ClinicDbContext context)
        : base(context) { }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
    {
        return await _dbSet
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate
    )
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByStatusAsync(string status)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.Status == status)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetUpcomingAsync(int days = 7)
    {
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(days);

        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a =>
                a.AppointmentDate >= startDate
                && a.AppointmentDate <= endDate
                && a.Status != "Cancelled"
            )
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<Appointment?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Include(a => a.MedicalRecords)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> HasConflictAsync(
        int doctorId,
        DateTime appointmentDate,
        TimeSpan duration,
        int? excludeId = null
    )
    {
        var endTime = appointmentDate.Add(duration);

        var query = _dbSet.Where(a =>
            a.DoctorId == doctorId
            && a.Status != "Cancelled"
            && (
                (a.AppointmentDate < endTime && a.AppointmentDate.Add(a.Duration) > appointmentDate)
            )
        );

        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
