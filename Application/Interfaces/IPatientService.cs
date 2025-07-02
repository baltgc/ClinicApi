using ClinicApi.Application.DTOs;

namespace ClinicApi.Application.Interfaces;

public interface IPatientService
{
    Task<IEnumerable<PatientResponseDto>> GetAllPatientsAsync();
    Task<PatientResponseDto?> GetPatientByIdAsync(int id);
    Task<PatientResponseDto?> GetPatientByEmailAsync(string email);
    Task<IEnumerable<PatientResponseDto>> GetActivePatientsAsync();
    Task<IEnumerable<PatientResponseDto>> SearchPatientsAsync(string searchTerm);
    Task<PatientResponseDto> CreatePatientAsync(CreatePatientDto createPatientDto);
    Task<PatientResponseDto?> UpdatePatientAsync(int id, UpdatePatientDto updatePatientDto);
    Task<bool> DeletePatientAsync(int id);
    Task<bool> DeactivatePatientAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
} 