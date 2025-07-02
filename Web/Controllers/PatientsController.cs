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
    /// Get patient by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PatientResponseDto>> GetPatient(int id)
    {
        var patient = await _patientService.GetPatientByIdAsync(id);
        if (patient == null)
        {
            return NotFound($"Patient with ID {id} not found.");
        }
        return Ok(patient);
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
            return BadRequest("Search term is required.");
        }

        var patients = await _patientService.SearchPatientsAsync(searchTerm);
        return Ok(patients);
    }

    /// <summary>
    /// Create a new patient
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<PatientResponseDto>> CreatePatient(
        CreatePatientDto createPatientDto
    )
    {
        try
        {
            // Check if email already exists
            if (await _patientService.EmailExistsAsync(createPatientDto.Email))
            {
                return BadRequest("A patient with this email already exists.");
            }

            var patient = await _patientService.CreatePatientAsync(createPatientDto);
            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update an existing patient
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<PatientResponseDto>> UpdatePatient(
        int id,
        UpdatePatientDto updatePatientDto
    )
    {
        try
        {
            // Check if email already exists for another patient
            if (
                !string.IsNullOrEmpty(updatePatientDto.Email)
                && await _patientService.EmailExistsAsync(updatePatientDto.Email, id)
            )
            {
                return BadRequest("A patient with this email already exists.");
            }

            var patient = await _patientService.UpdatePatientAsync(id, updatePatientDto);
            if (patient == null)
            {
                return NotFound($"Patient with ID {id} not found.");
            }
            return Ok(patient);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deactivate a patient (Staff only)
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    [Authorize(Policy = "StaffOnly")]
    public async Task<IActionResult> DeactivatePatient(int id)
    {
        var result = await _patientService.DeactivatePatientAsync(id);
        if (!result)
        {
            return NotFound($"Patient with ID {id} not found.");
        }
        return NoContent();
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
}
