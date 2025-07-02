using ClinicApi.Application.Commands.Patients;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Patients;

public class DeactivatePatientCommandHandler : IRequestHandler<DeactivatePatientCommand, bool>
{
    private readonly IPatientRepository _patientRepository;

    public DeactivatePatientCommandHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<bool> Handle(
        DeactivatePatientCommand request,
        CancellationToken cancellationToken
    )
    {
        var patient = await _patientRepository.GetByIdAsync(request.PatientId);
        if (patient == null)
            return false;

        patient.IsActive = false;
        patient.UpdatedAt = DateTime.UtcNow;

        await _patientRepository.UpdateAsync(patient);
        return true;
    }
}
