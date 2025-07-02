using MediatR;

namespace ClinicApi.Application.Commands.Patients;

public record DeactivatePatientCommand(int PatientId) : IRequest<bool>;
