using ClinicApi.Business.Application.DTOs;

namespace ClinicApi.Business.Application.Interfaces;

public interface IPatientService
{
    Task<IEnumerable<PatientResponseDto>> GetAllPatientsAsync();
    Task<PatientResponseDto?> GetPatientByIdAsync(int id);
    Task<PatientResponseDto?> GetPatientByEmailAsync(string email);
    Task<IEnumerable<PatientResponseDto>> SearchPatientsAsync(string searchTerm);
    Task<IEnumerable<PatientResponseDto>> GetActivePatientsAsync();
    Task<PatientResponseDto> CreatePatientAsync(CreatePatientDto createPatientDto);
    Task<PatientResponseDto?> UpdatePatientAsync(int id, UpdatePatientDto updatePatientDto);
    Task<bool> DeletePatientAsync(int id);
    Task<bool> DeactivatePatientAsync(int id);
    Task<PatientResponseDto?> GetPatientWithAppointmentsAsync(int id);
    Task<PatientResponseDto?> GetPatientWithMedicalRecordsAsync(int id);

    // Domain service methods
    Task<string> GetPatientRiskLevelAsync(int id);
    Task<List<string>> GetPatientAccommodationsAsync(int id);
    Task<bool> IsPatientEligibleForAppointmentAsync(int id, string appointmentType);
    Task<TimeSpan> GetRecommendedFollowUpIntervalAsync(int id, string diagnosisType);
}
