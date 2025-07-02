using System.Security.Claims;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Interfaces;
using ClinicApi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    /// <summary>
    /// Get all appointments (Staff only)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAllAppointments()
    {
        var appointments = await _appointmentService.GetAllAppointmentsAsync();
        return Ok(appointments);
    }

    /// <summary>
    /// Get appointment by ID (Staff, or own appointment for patients)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentResponseDto>> GetAppointment(int id)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
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

        var appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);
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

        var appointments = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctorId);
        return Ok(appointments);
    }

    /// <summary>
    /// Create a new appointment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AppointmentResponseDto>> CreateAppointment(
        CreateAppointmentDto createAppointmentDto
    )
    {
        try
        {
            var appointment = await _appointmentService.CreateAppointmentAsync(
                createAppointmentDto
            );
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
        UpdateAppointmentDto updateAppointmentDto
    )
    {
        try
        {
            var appointment = await _appointmentService.UpdateAppointmentAsync(
                id,
                updateAppointmentDto
            );
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
        var result = await _appointmentService.CancelAppointmentAsync(id, reason);
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
        var result = await _appointmentService.DeleteAppointmentAsync(id);
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
        var duration = TimeSpan.FromMinutes(durationMinutes);
        var isAvailable = await _appointmentService.CheckAvailabilityAsync(
            doctorId,
            appointmentDate,
            duration
        );
        return Ok(isAvailable);
    }
}
