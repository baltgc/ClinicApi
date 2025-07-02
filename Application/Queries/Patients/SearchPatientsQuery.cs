using ClinicApi.Application.DTOs;
using MediatR;

namespace ClinicApi.Application.Queries.Patients;

public record SearchPatientsQuery(string SearchTerm) : IRequest<IEnumerable<PatientResponseDto>>;
