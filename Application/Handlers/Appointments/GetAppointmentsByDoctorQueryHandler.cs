using AutoMapper;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Queries.Appointments;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Appointments;

public class GetAppointmentsByDoctorQueryHandler
    : IRequestHandler<GetAppointmentsByDoctorQuery, IEnumerable<AppointmentResponseDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetAppointmentsByDoctorQueryHandler(
        IAppointmentRepository appointmentRepository,
        IMapper mapper
    )
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AppointmentResponseDto>> Handle(
        GetAppointmentsByDoctorQuery request,
        CancellationToken cancellationToken
    )
    {
        var appointments = await _appointmentRepository.GetByDoctorIdAsync(request.DoctorId);
        return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
    }
}
