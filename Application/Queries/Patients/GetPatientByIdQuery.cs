using ClinicApi.Application.DTOs;
using MediatR;

namespace ClinicApi.Application.Queries.Patients;

public record GetPatientByIdQuery(int Id) : IRequest<PatientResponseDto?>;
