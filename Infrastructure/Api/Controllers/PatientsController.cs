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
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    /// <summary>
    /// Get all patients (Staff only)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetAllPatients()
    {
        var patients = await _patientService.GetAllPatientsAsync();
        return Ok(patients);
    }

    /// <summary>
    /// Get patient by ID (Staff or own patient record)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PatientResponseDto>> GetPatient(int id)
    {
        // Check if user is a patient trying to access their own record
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var userPatientId = User.FindFirst("patientId")?.Value;

        if (
            userRoles.Contains(ClinicRoles.Patient)
            && (userPatientId == null || int.Parse(userPatientId) != id)
        )
        {
            return Forbid("Patients can only access their own records.");
        }

        var patient = await _patientService.GetPatientByIdAsync(id);
        if (patient == null)
        {
            return NotFound($"Patient with ID {id} not found.");
        }
        return Ok(patient);
    }

    /// <summary>
    /// Search patients by name, email, or phone (Staff only)
    /// </summary>
    [HttpGet("search")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<IEnumerable<PatientResponseDto>>> SearchPatients(
        [FromQuery] string searchTerm
    )
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return BadRequest("Search term cannot be empty.");
        }

        var patients = await _patientService.SearchPatientsAsync(searchTerm);
        return Ok(patients);
    }

    /// <summary>
    /// Get active patients (Staff only)
    /// </summary>
    [HttpGet("active")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetActivePatients()
    {
        var patients = await _patientService.GetActivePatientsAsync();
        return Ok(patients);
    }

    /// <summary>
    /// Get patient with appointments (Staff or own patient record)
    /// </summary>
    [HttpGet("{id}/appointments")]
    public async Task<ActionResult<PatientResponseDto>> GetPatientWithAppointments(int id)
    {
        // Check if user is a patient trying to access their own record
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var userPatientId = User.FindFirst("patientId")?.Value;

        if (
            userRoles.Contains(ClinicRoles.Patient)
            && (userPatientId == null || int.Parse(userPatientId) != id)
        )
        {
            return Forbid("Patients can only access their own records.");
        }

        var patient = await _patientService.GetPatientWithAppointmentsAsync(id);
        if (patient == null)
        {
            return NotFound($"Patient with ID {id} not found.");
        }
        return Ok(patient);
    }

    /// <summary>
    /// Get patient with medical records (Medical staff or own patient record)
    /// </summary>
    [HttpGet("{id}/medical-records")]
    public async Task<ActionResult<PatientResponseDto>> GetPatientWithMedicalRecords(int id)
    {
        // Check if user is a patient trying to access their own record or medical staff
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var userPatientId = User.FindFirst("patientId")?.Value;

        var isMedicalStaff = userRoles.Any(r => r == ClinicRoles.Doctor || r == ClinicRoles.Nurse);
        var isOwnRecord =
            userRoles.Contains(ClinicRoles.Patient)
            && userPatientId != null
            && int.Parse(userPatientId) == id;

        if (!isMedicalStaff && !isOwnRecord)
        {
            return Forbid(
                "Access denied. Medical records require medical staff privileges or own patient access."
            );
        }

        var patient = await _patientService.GetPatientWithMedicalRecordsAsync(id);
        if (patient == null)
        {
            return NotFound($"Patient with ID {id} not found.");
        }
        return Ok(patient);
    }

    /// <summary>
    /// Create a new patient (Staff only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<PatientResponseDto>> CreatePatient(
        CreatePatientDto createPatientDto
    )
    {
        try
        {
            var patient = await _patientService.CreatePatientAsync(createPatientDto);
            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update patient information (Staff or own patient record)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<PatientResponseDto>> UpdatePatient(
        int id,
        UpdatePatientDto updatePatientDto
    )
    {
        // Check if user is a patient trying to update their own record
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var userPatientId = User.FindFirst("patientId")?.Value;

        if (
            userRoles.Contains(ClinicRoles.Patient)
            && (userPatientId == null || int.Parse(userPatientId) != id)
        )
        {
            return Forbid("Patients can only update their own records.");
        }

        try
        {
            var patient = await _patientService.UpdatePatientAsync(id, updatePatientDto);
            if (patient == null)
            {
                return NotFound($"Patient with ID {id} not found.");
            }
            return Ok(patient);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete a patient (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var result = await _patientService.DeletePatientAsync(id);
        if (!result)
        {
            return NotFound($"Patient with ID {id} not found.");
        }
        return NoContent();
    }

    /// <summary>
    /// Deactivate a patient (Admin/Manager only)
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> DeactivatePatient(int id)
    {
        var result = await _patientService.DeactivatePatientAsync(id);
        if (!result)
        {
            return NotFound($"Patient with ID {id} not found.");
        }
        return NoContent();
    }
}
