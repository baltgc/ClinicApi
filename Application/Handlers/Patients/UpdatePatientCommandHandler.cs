using AutoMapper;
using ClinicApi.Application.Commands.Patients;
using ClinicApi.Application.DTOs;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Patients;

public class UpdatePatientCommandHandler
    : IRequestHandler<UpdatePatientCommand, PatientResponseDto?>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public UpdatePatientCommandHandler(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<PatientResponseDto?> Handle(
        UpdatePatientCommand request,
        CancellationToken cancellationToken
    )
    {
        var patient = await _patientRepository.GetByIdAsync(request.Id);
        if (patient == null)
            return null;

        // Update only provided fields
        if (!string.IsNullOrEmpty(request.FirstName))
            patient.FirstName = request.FirstName;

        if (!string.IsNullOrEmpty(request.LastName))
            patient.LastName = request.LastName;

        if (!string.IsNullOrEmpty(request.Email))
            patient.Email = request.Email;

        if (!string.IsNullOrEmpty(request.PhoneNumber))
            patient.PhoneNumber = request.PhoneNumber;

        if (request.DateOfBirth.HasValue)
            patient.DateOfBirth = request.DateOfBirth.Value;

        if (!string.IsNullOrEmpty(request.Gender))
            patient.Gender = request.Gender;

        if (!string.IsNullOrEmpty(request.Address))
            patient.Address = request.Address;

        if (!string.IsNullOrEmpty(request.BloodType))
            patient.BloodType = request.BloodType;

        if (!string.IsNullOrEmpty(request.MedicalHistory))
            patient.MedicalHistory = request.MedicalHistory;

        if (!string.IsNullOrEmpty(request.Allergies))
            patient.Allergies = request.Allergies;

        if (!string.IsNullOrEmpty(request.EmergencyContact))
            patient.EmergencyContact = request.EmergencyContact;

        if (!string.IsNullOrEmpty(request.EmergencyContactPhone))
            patient.EmergencyContactPhone = request.EmergencyContactPhone;

        if (request.IsActive.HasValue)
            patient.IsActive = request.IsActive.Value;

        patient.UpdatedAt = DateTime.UtcNow;

        await _patientRepository.UpdateAsync(patient);
        return _mapper.Map<PatientResponseDto>(patient);
    }
}
