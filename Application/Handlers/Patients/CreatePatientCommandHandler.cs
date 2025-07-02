using AutoMapper;
using ClinicApi.Application.Commands.Patients;
using ClinicApi.Application.DTOs;
using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Patients;

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, PatientResponseDto>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public CreatePatientCommandHandler(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<PatientResponseDto> Handle(
        CreatePatientCommand request,
        CancellationToken cancellationToken
    )
    {
        // Check if patient with email already exists
        var existingPatients = await _patientRepository.GetAllAsync();
        var existingPatient = existingPatients.FirstOrDefault(p => p.Email == request.Email);

        if (existingPatient != null)
            throw new InvalidOperationException(
                $"Patient with email {request.Email} already exists."
            );

        // Create patient entity
        var patient = _mapper.Map<Patient>(request);
        patient.CreatedAt = DateTime.UtcNow;
        patient.UpdatedAt = DateTime.UtcNow;
        patient.IsActive = true;

        // Save patient
        await _patientRepository.AddAsync(patient);

        // Map to DTO and return
        return _mapper.Map<PatientResponseDto>(patient);
    }
}
