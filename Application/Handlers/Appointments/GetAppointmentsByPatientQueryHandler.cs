using AutoMapper;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Queries.Appointments;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Appointments;

public class GetAppointmentsByPatientQueryHandler
    : IRequestHandler<GetAppointmentsByPatientQuery, IEnumerable<AppointmentResponseDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetAppointmentsByPatientQueryHandler(
        IAppointmentRepository appointmentRepository,
        IMapper mapper
    )
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AppointmentResponseDto>> Handle(
        GetAppointmentsByPatientQuery request,
        CancellationToken cancellationToken
    )
    {
        var appointments = await _appointmentRepository.GetByPatientIdAsync(request.PatientId);

        return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
    }
}
