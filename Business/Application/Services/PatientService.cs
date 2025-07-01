using AutoMapper;
using ClinicApi.Business.Application.DTOs;
using ClinicApi.Business.Application.Interfaces;
using ClinicApi.Business.Domain.Interfaces;
using ClinicApi.Business.Domain.Models;
using ClinicApi.Business.Domain.Services;

namespace ClinicApi.Business.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientDomainService _patientDomainService;
    private readonly IMapper _mapper;

    public PatientService(
        IPatientRepository patientRepository,
        IPatientDomainService patientDomainService,
        IMapper mapper
    )
    {
        _patientRepository = patientRepository;
        _patientDomainService = patientDomainService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PatientResponseDto>> GetAllPatientsAsync()
    {
        var patients = await _patientRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PatientResponseDto>>(patients);
    }

    public async Task<PatientResponseDto?> GetPatientByIdAsync(int id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        return patient != null ? _mapper.Map<PatientResponseDto>(patient) : null;
    }

    public async Task<PatientResponseDto?> GetPatientByEmailAsync(string email)
    {
        var patient = await _patientRepository.GetByEmailAsync(email);
        return patient != null ? _mapper.Map<PatientResponseDto>(patient) : null;
    }

    public async Task<IEnumerable<PatientResponseDto>> SearchPatientsAsync(string searchTerm)
    {
        var patients = await _patientRepository.SearchAsync(searchTerm);
        return _mapper.Map<IEnumerable<PatientResponseDto>>(patients);
    }

    public async Task<IEnumerable<PatientResponseDto>> GetActivePatientsAsync()
    {
        var patients = await _patientRepository.GetActiveAsync();
        return _mapper.Map<IEnumerable<PatientResponseDto>>(patients);
    }

    public async Task<PatientResponseDto> CreatePatientAsync(CreatePatientDto createPatientDto)
    {
        // Check if email already exists
        var existingPatient = await _patientRepository.GetByEmailAsync(createPatientDto.Email);
        if (existingPatient != null)
        {
            throw new InvalidOperationException("A patient with this email already exists.");
        }

        var patient = _mapper.Map<Patient>(createPatientDto);
        patient.CreatedAt = DateTime.UtcNow;
        patient.UpdatedAt = DateTime.UtcNow;

        var createdPatient = await _patientRepository.AddAsync(patient);
        return _mapper.Map<PatientResponseDto>(createdPatient);
    }

    public async Task<PatientResponseDto?> UpdatePatientAsync(
        int id,
        UpdatePatientDto updatePatientDto
    )
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
        {
            return null;
        }

        // Check if email is being changed and if it already exists
        if (
            !string.IsNullOrEmpty(updatePatientDto.Email)
            && updatePatientDto.Email != patient.Email
        )
        {
            var existingPatient = await _patientRepository.GetByEmailAsync(updatePatientDto.Email);
            if (existingPatient != null)
            {
                throw new InvalidOperationException("A patient with this email already exists.");
            }
        }

        // Update only provided fields
        if (!string.IsNullOrEmpty(updatePatientDto.FirstName))
            patient.FirstName = updatePatientDto.FirstName;
        if (!string.IsNullOrEmpty(updatePatientDto.LastName))
            patient.LastName = updatePatientDto.LastName;
        if (!string.IsNullOrEmpty(updatePatientDto.Email))
            patient.Email = updatePatientDto.Email;
        if (!string.IsNullOrEmpty(updatePatientDto.PhoneNumber))
            patient.PhoneNumber = updatePatientDto.PhoneNumber;
        if (updatePatientDto.DateOfBirth.HasValue)
            patient.DateOfBirth = updatePatientDto.DateOfBirth.Value;
        if (!string.IsNullOrEmpty(updatePatientDto.Gender))
            patient.Gender = updatePatientDto.Gender;
        if (updatePatientDto.Address != null)
            patient.Address = updatePatientDto.Address;
        if (updatePatientDto.BloodType != null)
            patient.BloodType = updatePatientDto.BloodType;
        if (updatePatientDto.MedicalHistory != null)
            patient.MedicalHistory = updatePatientDto.MedicalHistory;
        if (updatePatientDto.Allergies != null)
            patient.Allergies = updatePatientDto.Allergies;
        if (updatePatientDto.EmergencyContact != null)
            patient.EmergencyContact = updatePatientDto.EmergencyContact;
        if (updatePatientDto.EmergencyContactPhone != null)
            patient.EmergencyContactPhone = updatePatientDto.EmergencyContactPhone;

        patient.UpdatedAt = DateTime.UtcNow;

        var updatedPatient = await _patientRepository.UpdateAsync(patient);
        return _mapper.Map<PatientResponseDto>(updatedPatient);
    }

    public async Task<bool> DeletePatientAsync(int id)
    {
        return await _patientRepository.DeleteAsync(id);
    }

    public async Task<bool> DeactivatePatientAsync(int id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
        {
            return false;
        }

        patient.IsActive = false;
        patient.UpdatedAt = DateTime.UtcNow;

        await _patientRepository.UpdateAsync(patient);
        return true;
    }

    public async Task<PatientResponseDto?> GetPatientWithAppointmentsAsync(int id)
    {
        var patient = await _patientRepository.GetWithAppointmentsAsync(id);
        return patient != null ? _mapper.Map<PatientResponseDto>(patient) : null;
    }

    public async Task<PatientResponseDto?> GetPatientWithMedicalRecordsAsync(int id)
    {
        var patient = await _patientRepository.GetWithMedicalRecordsAsync(id);
        return patient != null ? _mapper.Map<PatientResponseDto>(patient) : null;
    }

    // Additional methods leveraging domain services

    /// <summary>
    /// Get patient risk level assessment
    /// </summary>
    public async Task<string> GetPatientRiskLevelAsync(int id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
            throw new InvalidOperationException("Patient not found.");

        return _patientDomainService.CalculateRiskLevel(patient);
    }

    /// <summary>
    /// Get required accommodations for patient
    /// </summary>
    public async Task<List<string>> GetPatientAccommodationsAsync(int id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
            throw new InvalidOperationException("Patient not found.");

        return _patientDomainService.GetRequiredAccommodations(patient);
    }

    /// <summary>
    /// Validate if patient is eligible for specific appointment type
    /// </summary>
    public async Task<bool> IsPatientEligibleForAppointmentAsync(int id, string appointmentType)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
            return false;

        return _patientDomainService.IsEligibleForAppointmentType(patient, appointmentType);
    }

    /// <summary>
    /// Get recommended follow-up interval for patient based on diagnosis
    /// </summary>
    public async Task<TimeSpan> GetRecommendedFollowUpIntervalAsync(int id, string diagnosisType)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
            throw new InvalidOperationException("Patient not found.");

        return _patientDomainService.GetRecommendedFollowUpInterval(patient, diagnosisType);
    }
}
