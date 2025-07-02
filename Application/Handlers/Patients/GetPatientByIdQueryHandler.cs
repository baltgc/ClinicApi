using AutoMapper;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Queries.Patients;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Patients;

public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientResponseDto?>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public GetPatientByIdQueryHandler(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<PatientResponseDto?> Handle(
        GetPatientByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var patient = await _patientRepository.GetByIdAsync(request.Id);

        return patient == null ? null : _mapper.Map<PatientResponseDto>(patient);
    }
}
