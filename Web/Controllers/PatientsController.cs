using ClinicApi.Application.Commands.Patients;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Queries.Patients;
using ClinicApi.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PatientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all patients (Staff only)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetAllPatients()
    {
        var patients = await _mediator.Send(new GetAllPatientsQuery());
        return Ok(patients);
    }

    /// <summary>
    /// Get patient by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PatientResponseDto>> GetPatient(int id)
    {
        var patient = await _mediator.Send(new GetPatientByIdQuery(id));
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
        var patients = await _mediator.Send(new GetActivePatientsQuery());
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

        var patients = await _mediator.Send(new SearchPatientsQuery(searchTerm));
        return Ok(patients);
    }

    /// <summary>
    /// Create a new patient
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "StaffOnly")]
    public async Task<ActionResult<PatientResponseDto>> CreatePatient(
        CreatePatientCommand createPatientCommand
    )
    {
        try
        {
            var patient = await _mediator.Send(createPatientCommand);
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
        [FromBody] UpdatePatientCommand updatePatientCommand
    )
    {
        try
        {
            var command = updatePatientCommand with { Id = id };
            var patient = await _mediator.Send(command);
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
        var result = await _mediator.Send(new DeactivatePatientCommand(id));
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
        var result = await _mediator.Send(new DeletePatientCommand(id));
        if (!result)
        {
            return NotFound($"Patient with ID {id} not found.");
        }
        return NoContent();
    }
}
