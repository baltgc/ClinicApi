using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Enums;

namespace ClinicApi.Domain.Interfaces;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
    Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status);
    Task<IEnumerable<Appointment>> GetUpcomingAsync(int days = 7);
    Task<Appointment?> GetWithDetailsAsync(int id);
    Task<bool> HasConflictAsync(
        int doctorId,
        DateTime appointmentDate,
        TimeSpan duration,
        int? excludeId = null
    );
}
