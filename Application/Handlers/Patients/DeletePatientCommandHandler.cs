using ClinicApi.Application.Commands.Patients;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Patients;

public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, bool>
{
    private readonly IPatientRepository _patientRepository;

    public DeletePatientCommandHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<bool> Handle(
        DeletePatientCommand request,
        CancellationToken cancellationToken
    )
    {
        var patient = await _patientRepository.GetByIdAsync(request.PatientId);
        if (patient == null)
            return false;

        await _patientRepository.DeleteAsync(patient.Id);
        return true;
    }
}
