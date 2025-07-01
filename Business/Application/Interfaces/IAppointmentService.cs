using ClinicApi.Business.Application.DTOs;

namespace ClinicApi.Business.Application.Interfaces;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsAsync();
    Task<AppointmentResponseDto?> GetAppointmentByIdAsync(int id);
    Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByPatientIdAsync(int patientId);
    Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByDoctorIdAsync(int doctorId);
    Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByDateRangeAsync(
        DateTime startDate,
        DateTime endDate
    );
    Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByStatusAsync(string status);
    Task<IEnumerable<AppointmentResponseDto>> GetUpcomingAppointmentsAsync(int days = 7);
    Task<AppointmentResponseDto> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto);
    Task<AppointmentResponseDto?> UpdateAppointmentAsync(
        int id,
        UpdateAppointmentDto updateAppointmentDto
    );
    Task<bool> DeleteAppointmentAsync(int id);
    Task<bool> CancelAppointmentAsync(int id, string reason);
    Task<AppointmentResponseDto?> GetAppointmentWithDetailsAsync(int id);
    Task<bool> CheckAvailabilityAsync(int doctorId, DateTime appointmentDate, TimeSpan duration);
}
