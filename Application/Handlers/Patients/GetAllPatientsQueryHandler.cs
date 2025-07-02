using AutoMapper;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Queries.Patients;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Patients;

public class GetAllPatientsQueryHandler
    : IRequestHandler<GetAllPatientsQuery, IEnumerable<PatientResponseDto>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public GetAllPatientsQueryHandler(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PatientResponseDto>> Handle(
        GetAllPatientsQuery request,
        CancellationToken cancellationToken
    )
    {
        var patients = await _patientRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PatientResponseDto>>(patients);
    }
}
