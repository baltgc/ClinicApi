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
        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new ArgumentException("First name is required.", nameof(request.FirstName));

        if (string.IsNullOrWhiteSpace(request.LastName))
            throw new ArgumentException("Last name is required.", nameof(request.LastName));

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required.", nameof(request.Email));

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
        var savedPatient = await _patientRepository.AddAsync(patient);

        // Map to DTO and return
        return _mapper.Map<PatientResponseDto>(savedPatient);
    }
}
