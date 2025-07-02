using AutoMapper;
using ClinicApi.Application.Commands.Appointments;
using ClinicApi.Application.DTOs;
using ClinicApi.Domain.Interfaces;
using MediatR;

namespace ClinicApi.Application.Handlers.Appointments;

public class UpdateAppointmentCommandHandler
    : IRequestHandler<UpdateAppointmentCommand, AppointmentResponseDto?>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public UpdateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IMapper mapper
    )
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<AppointmentResponseDto?> Handle(
        UpdateAppointmentCommand request,
        CancellationToken cancellationToken
    )
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.Id);
        if (appointment == null)
            return null;

        // Update only provided fields
        if (request.AppointmentDate.HasValue)
            appointment.AppointmentDate = request.AppointmentDate.Value;

        if (request.Duration.HasValue)
            appointment.Duration = request.Duration.Value;

        if (!string.IsNullOrEmpty(request.ReasonForVisit))
            appointment.ReasonForVisit = request.ReasonForVisit;

        if (!string.IsNullOrEmpty(request.Notes))
            appointment.Notes = request.Notes;

        appointment.UpdatedAt = DateTime.UtcNow;

        await _appointmentRepository.UpdateAsync(appointment);
        return _mapper.Map<AppointmentResponseDto>(appointment);
    }
}
