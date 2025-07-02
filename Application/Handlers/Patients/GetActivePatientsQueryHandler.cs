using AutoMapper;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Queries.Patients;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Patients;

public class GetActivePatientsQueryHandler
    : IRequestHandler<GetActivePatientsQuery, IEnumerable<PatientResponseDto>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public GetActivePatientsQueryHandler(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PatientResponseDto>> Handle(
        GetActivePatientsQuery request,
        CancellationToken cancellationToken
    )
    {
        var patients = await _patientRepository.GetActivePatientsAsync();
        return _mapper.Map<IEnumerable<PatientResponseDto>>(patients);
    }
}
