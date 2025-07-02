using AutoMapper;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Queries.Patients;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Patients;

public class SearchPatientsQueryHandler
    : IRequestHandler<SearchPatientsQuery, IEnumerable<PatientResponseDto>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public SearchPatientsQueryHandler(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PatientResponseDto>> Handle(
        SearchPatientsQuery request,
        CancellationToken cancellationToken
    )
    {
        // Validate search term
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
            throw new ArgumentException(
                "Search term cannot be null or empty.",
                nameof(request.SearchTerm)
            );

        var patients = await _patientRepository.SearchPatientsAsync(request.SearchTerm);
        return _mapper.Map<IEnumerable<PatientResponseDto>>(patients);
    }
}
