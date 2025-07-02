using System.Security.Claims;
using ClinicApi.Application.Commands.Appointments;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Queries.Appointments;
using ClinicApi.Application.Services;
using ClinicApi.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IBackgroundJobService _backgroundJobService;

    public AppointmentsController(IMediator mediator, IBackgroundJobService backgroundJobService)
    {
        _mediator = mediator;
        _backgroundJobService = backgroundJobService;
    }

    /// <summary>
    /// Get all appointments (Staff only)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAllAppointments()
    {
        var appointments = await _mediator.Send(new GetAllAppointmentsQuery());
        return Ok(appointments);
    }

    /// <summary>
    /// Get appointment by ID (Staff, or own appointment for patients)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentResponseDto>> GetAppointment(int id)
    {
        var appointment = await _mediator.Send(new GetAppointmentByIdQuery(id));
        if (appointment == null)
        {
            return NotFound($"Appointment with ID {id} not found.");
        }

        // Check if user is a patient trying to access their own appointment
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var userPatientId = User.FindFirst("patientId")?.Value;
        var userDoctorId = User.FindFirst("doctorId")?.Value;

        if (
            userRoles.Contains(ClinicRoles.Patient)
            && (userPatientId == null || int.Parse(userPatientId) != appointment.PatientId)
        )
        {
            return Forbid("Patients can only access their own appointments.");
        }

        if (
            userRoles.Contains(ClinicRoles.Doctor)
            && (userDoctorId == null || int.Parse(userDoctorId) != appointment.DoctorId)
        )
        {
            return Forbid("Doctors can only access their own appointments.");
        }

        return Ok(appointment);
    }

    /// <summary>
    /// Get appointments by patient ID (Staff or own appointments for patients)
    /// </summary>
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAppointmentsByPatient(
        int patientId
    )
    {
        // Check if user is a patient trying to access their own appointments
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var userPatientId = User.FindFirst("patientId")?.Value;

        if (
            userRoles.Contains(ClinicRoles.Patient)
            && (userPatientId == null || int.Parse(userPatientId) != patientId)
        )
        {
            return Forbid("Patients can only access their own appointments.");
        }

        var appointments = await _mediator.Send(new GetAppointmentsByPatientQuery(patientId));
        return Ok(appointments);
    }

    /// <summary>
    /// Get appointments by doctor ID (Staff or own appointments for doctors)
    /// </summary>
    [HttpGet("doctor/{doctorId}")]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAppointmentsByDoctor(
        int doctorId
    )
    {
        // Check if user is a doctor trying to access their own appointments
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var userDoctorId = User.FindFirst("doctorId")?.Value;

        var isStaff = userRoles.Any(r =>
            r == ClinicRoles.Receptionist || r == ClinicRoles.Manager || r == ClinicRoles.Admin
        );
        var isOwnDoctor =
            userRoles.Contains(ClinicRoles.Doctor)
            && userDoctorId != null
            && int.Parse(userDoctorId) == doctorId;

        if (!isStaff && !isOwnDoctor)
        {
            return Forbid("Doctors can only access their own appointments.");
        }

        var appointments = await _mediator.Send(new GetAppointmentsByDoctorQuery(doctorId));
        return Ok(appointments);
    }

    /// <summary>
    /// Create a new appointment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AppointmentResponseDto>> CreateAppointment(
        CreateAppointmentCommand createAppointmentCommand
    )
    {
        try
        {
            var appointment = await _mediator.Send(createAppointmentCommand);
            return CreatedAtAction(
                nameof(GetAppointment),
                new { id = appointment.Id },
                appointment
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update an existing appointment
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<AppointmentResponseDto>> UpdateAppointment(
        int id,
        [FromBody] UpdateAppointmentCommand updateAppointmentCommand
    )
    {
        try
        {
            var command = updateAppointmentCommand with { Id = id };
            var appointment = await _mediator.Send(command);
            if (appointment == null)
            {
                return NotFound($"Appointment with ID {id} not found.");
            }
            return Ok(appointment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Cancel an appointment
    /// </summary>
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(int id, [FromBody] string reason)
    {
        var result = await _mediator.Send(new CancelAppointmentCommand(id, reason));
        if (!result)
        {
            return NotFound($"Appointment with ID {id} not found or cannot be cancelled.");
        }
        return NoContent();
    }

    /// <summary>
    /// Delete an appointment (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var result = await _mediator.Send(new DeleteAppointmentCommand(id));
        if (!result)
        {
            return NotFound($"Appointment with ID {id} not found.");
        }
        return NoContent();
    }

    /// <summary>
    /// Check doctor availability for a specific time slot
    /// </summary>
    [HttpGet("check-availability")]
    public async Task<ActionResult<bool>> CheckAvailability(
        [FromQuery] int doctorId,
        [FromQuery] DateTime appointmentDate,
        [FromQuery] int durationMinutes = 30
    )
    {
        var isAvailable = await _mediator.Send(
            new CheckAvailabilityQuery(doctorId, appointmentDate, durationMinutes)
        );
        return Ok(isAvailable);
    }

    /// <summary>
    /// Schedule an appointment reminder (Staff only)
    /// </summary>
    [HttpPost("{id}/schedule-reminder")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> ScheduleAppointmentReminder(
        int id,
        [FromBody] ScheduleReminderRequest request
    )
    {
        try
        {
            // Verify appointment exists
            var appointment = await _mediator.Send(new GetAppointmentByIdQuery(id));
            if (appointment == null)
            {
                return NotFound($"Appointment with ID {id} not found.");
            }

            // Schedule the reminder
            var jobId = _backgroundJobService.ScheduleAppointmentReminder(id, request.ReminderTime);

            return Ok(
                new
                {
                    Message = "Appointment reminder scheduled successfully",
                    JobId = jobId,
                    AppointmentId = id,
                    ReminderTime = request.ReminderTime,
                }
            );
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to schedule reminder: {ex.Message}");
        }
    }

    /// <summary>
    /// Send appointment confirmation immediately (Staff only)
    /// </summary>
    [HttpPost("{id}/send-confirmation")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> SendAppointmentConfirmation(int id)
    {
        try
        {
            // Verify appointment exists
            var appointment = await _mediator.Send(new GetAppointmentByIdQuery(id));
            if (appointment == null)
            {
                return NotFound($"Appointment with ID {id} not found.");
            }

            // Send confirmation (background job)
            await _backgroundJobService.SendAppointmentConfirmationAsync(id);

            return Ok(
                new { Message = "Appointment confirmation sent successfully", AppointmentId = id }
            );
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to send confirmation: {ex.Message}");
        }
    }
}

/// <summary>
/// Request model for scheduling appointment reminders
/// </summary>
public record ScheduleReminderRequest(DateTime ReminderTime);
