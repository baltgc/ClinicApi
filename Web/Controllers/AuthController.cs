using System.Security.Claims;
using ClinicApi.Application.Commands.Auth;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Queries.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Authenticate user and return JWT token
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var command = new LoginCommand(loginDto.Email, loginDto.Password);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var command = new RegisterCommand(
                registerDto.FirstName,
                registerDto.LastName,
                registerDto.Email,
                registerDto.Password,
                registerDto.PhoneNumber,
                registerDto.Role,
                registerDto.Gender,
                registerDto.Address,
                registerDto.Department,
                registerDto.EmployeeId
            );

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> GetProfile()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not found." });
            }

            var query = new GetUserProfileQuery(userId);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = "User profile not found." });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Change user password (TODO: Implement with CQRS)
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public IActionResult ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        // TODO: Implement ChangePasswordCommand and Handler
        return Ok(new { message = "Change password endpoint - TODO: Implement with CQRS" });
    }

    /// <summary>
    /// Request password reset (TODO: Implement with CQRS)
    /// </summary>
    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        // TODO: Implement ForgotPasswordCommand and Handler
        return Ok(new { message = "Forgot password endpoint - TODO: Implement with CQRS" });
    }

    /// <summary>
    /// Reset password using token (TODO: Implement with CQRS)
    /// </summary>
    [HttpPost("reset-password")]
    public IActionResult ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        // TODO: Implement ResetPasswordCommand and Handler
        return Ok(new { message = "Reset password endpoint - TODO: Implement with CQRS" });
    }
}
