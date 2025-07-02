using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Enums;
using ClinicApi.Domain.Interfaces;
using ClinicApi.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Infrastructure.Data.Repositories;

public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ClinicDbContext context)
        : base(context) { }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
    {
        return await _context
            .Appointments.Where(a => a.PatientId == patientId)
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
    {
        return await _context
            .Appointments.Where(a => a.DoctorId == doctorId)
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate
    )
    {
        return await _context
            .Appointments.Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status)
    {
        return await _context
            .Appointments.Where(a => a.Status == status)
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetUpcomingAsync(int days = 7)
    {
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(days);

        return await _context
            .Appointments.Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
            .Where(a =>
                a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed
            )
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<Appointment?> GetWithDetailsAsync(int id)
    {
        return await _context
            .Appointments.Include(a => a.Patient)
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

        var query = _context
            .Appointments.Where(a => a.DoctorId == doctorId)
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .Where(a =>
                (a.AppointmentDate < endTime)
                && (a.AppointmentDate.Add(a.Duration) > appointmentDate)
            );

        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
