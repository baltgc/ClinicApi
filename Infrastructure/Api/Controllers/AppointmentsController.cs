using System.Security.Claims;
using ClinicApi.Business.Application.DTOs;
using ClinicApi.Business.Application.Interfaces;
using ClinicApi.Business.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApi.Infrastructure.Api.Controllers;

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
    /// Get appointments by date range (Staff only)
    /// </summary>
    [HttpGet("date-range")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAppointmentsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate
    )
    {
        if (startDate > endDate)
        {
            return BadRequest("Start date cannot be after end date.");
        }

        var appointments = await _appointmentService.GetAppointmentsByDateRangeAsync(
            startDate,
            endDate
        );
        return Ok(appointments);
    }

    /// <summary>
    /// Get appointments by status (Staff only)
    /// </summary>
    [HttpGet("status/{status}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAppointmentsByStatus(
        string status
    )
    {
        var appointments = await _appointmentService.GetAppointmentsByStatusAsync(status);
        return Ok(appointments);
    }

    /// <summary>
    /// Get upcoming appointments (Staff only)
    /// </summary>
    [HttpGet("upcoming")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetUpcomingAppointments(
        [FromQuery] int days = 7
    )
    {
        var appointments = await _appointmentService.GetUpcomingAppointmentsAsync(days);
        return Ok(appointments);
    }

    /// <summary>
    /// Get appointment with full details (Medical staff or involved parties)
    /// </summary>
    [HttpGet("{id}/details")]
    public async Task<ActionResult<AppointmentResponseDto>> GetAppointmentWithDetails(int id)
    {
        var appointment = await _appointmentService.GetAppointmentWithDetailsAsync(id);
        if (appointment == null)
        {
            return NotFound($"Appointment with ID {id} not found.");
        }

        // Check if user has access to appointment details
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var userPatientId = User.FindFirst("patientId")?.Value;
        var userDoctorId = User.FindFirst("doctorId")?.Value;

        var isMedicalStaff = userRoles.Any(r => r == ClinicRoles.Doctor || r == ClinicRoles.Nurse);
        var isStaff = userRoles.Any(r =>
            r == ClinicRoles.Receptionist || r == ClinicRoles.Manager || r == ClinicRoles.Admin
        );
        var isPatient =
            userRoles.Contains(ClinicRoles.Patient)
            && userPatientId != null
            && int.Parse(userPatientId) == appointment.PatientId;
        var isDoctor =
            userRoles.Contains(ClinicRoles.Doctor)
            && userDoctorId != null
            && int.Parse(userDoctorId) == appointment.DoctorId;

        if (!isMedicalStaff && !isStaff && !isPatient && !isDoctor)
        {
            return Forbid("Access denied to appointment details.");
        }

        return Ok(appointment);
    }

    /// <summary>
    /// Check doctor availability
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
        return Ok(new { IsAvailable = isAvailable });
    }

    /// <summary>
    /// Create a new appointment (Staff or patients for themselves)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AppointmentResponseDto>> CreateAppointment(
        CreateAppointmentDto createAppointmentDto
    )
    {
        // Check if user is a patient trying to create appointment for themselves
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var userPatientId = User.FindFirst("patientId")?.Value;

        if (
            userRoles.Contains(ClinicRoles.Patient)
            && (userPatientId == null || int.Parse(userPatientId) != createAppointmentDto.PatientId)
        )
        {
            return Forbid("Patients can only create appointments for themselves.");
        }

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
    /// Update appointment (Staff or involved parties)
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
    /// Cancel appointment (Staff or involved parties)
    /// </summary>
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(int id, [FromBody] string reason)
    {
        var result = await _appointmentService.CancelAppointmentAsync(id, reason);
        if (!result)
        {
            return NotFound($"Appointment with ID {id} not found.");
        }
        return NoContent();
    }

    /// <summary>
    /// Delete appointment (Admin only)
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
}
