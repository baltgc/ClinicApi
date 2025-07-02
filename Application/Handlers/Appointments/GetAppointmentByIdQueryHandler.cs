using AutoMapper;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Queries.Appointments;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Appointments;

public class GetAppointmentByIdQueryHandler
    : IRequestHandler<GetAppointmentByIdQuery, AppointmentResponseDto?>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetAppointmentByIdQueryHandler(
        IAppointmentRepository appointmentRepository,
        IMapper mapper
    )
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<AppointmentResponseDto?> Handle(
        GetAppointmentByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var appointment = await _appointmentRepository.GetWithDetailsAsync(request.Id);

        return appointment == null ? null : _mapper.Map<AppointmentResponseDto>(appointment);
    }
}
