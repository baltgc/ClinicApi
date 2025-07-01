using AutoMapper;
using ClinicApi.Business.Application.DTOs;
using ClinicApi.Business.Application.Interfaces;
using ClinicApi.Business.Domain.Interfaces;
using ClinicApi.Business.Domain.Models;
using ClinicApi.Business.Domain.Services;

namespace ClinicApi.Business.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IAppointmentDomainService _appointmentDomainService;
    private readonly IMapper _mapper;

    public AppointmentService(
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

    public async Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsAsync()
    {
        var appointments = await _appointmentRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
    }

    public async Task<AppointmentResponseDto?> GetAppointmentByIdAsync(int id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        return appointment != null ? _mapper.Map<AppointmentResponseDto>(appointment) : null;
    }

    public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByPatientIdAsync(
        int patientId
    )
    {
        var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId);
        return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
    }

    public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByDoctorIdAsync(
        int doctorId
    )
    {
        var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId);
        return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
    }

    public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByDateRangeAsync(
        DateTime startDate,
        DateTime endDate
    )
    {
        var appointments = await _appointmentRepository.GetByDateRangeAsync(startDate, endDate);
        return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
    }

    public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByStatusAsync(
        string status
    )
    {
        var appointments = await _appointmentRepository.GetByStatusAsync(status);
        return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
    }

    public async Task<IEnumerable<AppointmentResponseDto>> GetUpcomingAppointmentsAsync(
        int days = 7
    )
    {
        var appointments = await _appointmentRepository.GetUpcomingAsync(days);
        return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
    }

    public async Task<AppointmentResponseDto> CreateAppointmentAsync(
        CreateAppointmentDto createAppointmentDto
    )
    {
        // Validate patient exists
        var patient = await _patientRepository.GetByIdAsync(createAppointmentDto.PatientId);
        if (patient == null)
        {
            throw new InvalidOperationException("Patient not found.");
        }

        // Validate doctor exists
        var doctor = await _doctorRepository.GetByIdAsync(createAppointmentDto.DoctorId);
        if (doctor == null)
        {
            throw new InvalidOperationException("Doctor not found.");
        }

        // Use domain service for comprehensive appointment validation
        var canSchedule = await _appointmentDomainService.CanScheduleAppointmentAsync(
            createAppointmentDto.DoctorId,
            createAppointmentDto.AppointmentDate,
            createAppointmentDto.Duration
        );

        if (!canSchedule)
        {
            throw new InvalidOperationException(
                "Cannot schedule appointment: time slot conflicts, outside working hours, or violates business rules."
            );
        }

        // Calculate consultation fee using domain service
        var consultationFee = _appointmentDomainService.CalculateConsultationFee(
            doctor!,
            createAppointmentDto.Duration,
            createAppointmentDto.ReasonForVisit ?? "Regular"
        );

        var appointment = _mapper.Map<Appointment>(createAppointmentDto);
        appointment.Status = "Scheduled";
        appointment.ConsultationFee = consultationFee;
        appointment.CreatedAt = DateTime.UtcNow;
        appointment.UpdatedAt = DateTime.UtcNow;

        var createdAppointment = await _appointmentRepository.AddAsync(appointment);
        return _mapper.Map<AppointmentResponseDto>(createdAppointment);
    }

    public async Task<AppointmentResponseDto?> UpdateAppointmentAsync(
        int id,
        UpdateAppointmentDto updateAppointmentDto
    )
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
        {
            return null;
        }

        // Check for conflicts if appointment date or duration is being changed
        if (updateAppointmentDto.AppointmentDate.HasValue || updateAppointmentDto.Duration.HasValue)
        {
            var newDate = updateAppointmentDto.AppointmentDate ?? appointment.AppointmentDate;
            var newDuration = updateAppointmentDto.Duration ?? appointment.Duration;

            var hasConflict = await _appointmentRepository.HasConflictAsync(
                appointment.DoctorId,
                newDate,
                newDuration,
                id
            );

            if (hasConflict)
            {
                throw new InvalidOperationException("The selected time slot is not available.");
            }
        }

        // Update only provided fields
        if (updateAppointmentDto.AppointmentDate.HasValue)
            appointment.AppointmentDate = updateAppointmentDto.AppointmentDate.Value;
        if (updateAppointmentDto.Duration.HasValue)
            appointment.Duration = updateAppointmentDto.Duration.Value;
        if (!string.IsNullOrEmpty(updateAppointmentDto.Status))
            appointment.Status = updateAppointmentDto.Status;
        if (updateAppointmentDto.ReasonForVisit != null)
            appointment.ReasonForVisit = updateAppointmentDto.ReasonForVisit;
        if (updateAppointmentDto.Notes != null)
            appointment.Notes = updateAppointmentDto.Notes;
        if (updateAppointmentDto.CancellationReason != null)
            appointment.CancellationReason = updateAppointmentDto.CancellationReason;
        if (updateAppointmentDto.ConsultationFee.HasValue)
            appointment.ConsultationFee = updateAppointmentDto.ConsultationFee;

        appointment.UpdatedAt = DateTime.UtcNow;

        var updatedAppointment = await _appointmentRepository.UpdateAsync(appointment);
        return _mapper.Map<AppointmentResponseDto>(updatedAppointment);
    }

    public async Task<bool> DeleteAppointmentAsync(int id)
    {
        return await _appointmentRepository.DeleteAsync(id);
    }

    public async Task<bool> CancelAppointmentAsync(int id, string reason)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
        {
            return false;
        }

        // Use domain service to validate cancellation rules
        var canCancel = _appointmentDomainService.CanCancelAppointment(
            appointment,
            DateTime.UtcNow
        );
        if (!canCancel)
        {
            throw new InvalidOperationException(
                "Cannot cancel appointment: insufficient notice period or appointment has already started."
            );
        }

        appointment.Status = "Cancelled";
        appointment.CancellationReason = reason;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _appointmentRepository.UpdateAsync(appointment);
        return true;
    }

    public async Task<AppointmentResponseDto?> GetAppointmentWithDetailsAsync(int id)
    {
        var appointment = await _appointmentRepository.GetWithDetailsAsync(id);
        return appointment != null ? _mapper.Map<AppointmentResponseDto>(appointment) : null;
    }

    public async Task<bool> CheckAvailabilityAsync(
        int doctorId,
        DateTime appointmentDate,
        TimeSpan duration
    )
    {
        return await _appointmentDomainService.CanScheduleAppointmentAsync(
            doctorId,
            appointmentDate,
            duration
        );
    }
}
