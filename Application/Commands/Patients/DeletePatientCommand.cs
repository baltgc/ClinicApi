using MediatR;

namespace ClinicApi.Application.Commands.Patients;

public record DeletePatientCommand(int PatientId) : IRequest<bool>;
