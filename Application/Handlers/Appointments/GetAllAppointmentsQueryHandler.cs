using AutoMapper;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Queries.Appointments;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Appointments;

public class GetAllAppointmentsQueryHandler
    : IRequestHandler<GetAllAppointmentsQuery, IEnumerable<AppointmentResponseDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetAllAppointmentsQueryHandler(
        IAppointmentRepository appointmentRepository,
        IMapper mapper
    )
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AppointmentResponseDto>> Handle(
        GetAllAppointmentsQuery request,
        CancellationToken cancellationToken
    )
    {
        var appointments = await _appointmentRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
    }
}
