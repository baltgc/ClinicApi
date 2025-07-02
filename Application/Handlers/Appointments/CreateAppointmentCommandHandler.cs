using AutoMapper;
using ClinicApi.Application.Commands.Appointments;
using ClinicApi.Application.DTOs;
using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Enums;
using ClinicApi.Domain.Interfaces;
using ClinicApi.Domain.Services;
using MediatR;

namespace ClinicApi.Application.Handlers.Appointments;

public class CreateAppointmentCommandHandler
    : IRequestHandler<CreateAppointmentCommand, AppointmentResponseDto>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IAppointmentDomainService _appointmentDomainService;
    private readonly IMapper _mapper;

    public CreateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository,
        IAppointmentDomainService appointmentDomainService,
        IMapper mapper
    )
    {
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _appointmentDomainService = appointmentDomainService;
        _mapper = mapper;
    }

    public async Task<AppointmentResponseDto> Handle(
        CreateAppointmentCommand request,
        CancellationToken cancellationToken
    )
    {
        // Validate patient exists
        var patient = await _patientRepository.GetByIdAsync(request.PatientId);
        if (patient == null)
            throw new ArgumentException($"Patient with ID {request.PatientId} not found.");

        // Validate doctor exists
        var doctor = await _doctorRepository.GetByIdAsync(request.DoctorId);
        if (doctor == null)
            throw new ArgumentException($"Doctor with ID {request.DoctorId} not found.");

        // Validate appointment can be scheduled using domain service
        var canSchedule = await _appointmentDomainService.CanScheduleAppointmentAsync(
            request.DoctorId,
            request.AppointmentDate,
            request.Duration
        );

        if (!canSchedule)
            throw new InvalidOperationException(
                "Appointment cannot be scheduled due to conflicts or business rules."
            );

        // Calculate consultation fee using domain service
        var consultationFee = _appointmentDomainService.CalculateConsultationFee(
            doctor,
            request.Duration,
            "Regular"
        );

        // Create appointment entity
        var appointment = new Appointment
        {
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            AppointmentDate = request.AppointmentDate,
            Duration = request.Duration,
            ReasonForVisit = request.ReasonForVisit,
            Notes = request.Notes,
            Status = AppointmentStatus.Scheduled,
            ConsultationFee = consultationFee,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        // Save appointment
        await _appointmentRepository.AddAsync(appointment);

        // Load relationships for response
        var savedAppointment = await _appointmentRepository.GetWithDetailsAsync(appointment.Id);

        // Map to DTO and return
        return _mapper.Map<AppointmentResponseDto>(savedAppointment);
    }
}
