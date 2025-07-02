using ClinicApi.Application.DTOs;
using MediatR;

namespace ClinicApi.Application.Queries.Patients;

public record GetActivePatientsQuery : IRequest<IEnumerable<PatientResponseDto>>;
