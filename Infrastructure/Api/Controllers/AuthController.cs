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
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "API is running", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// User login
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        try
        {
            var response = await _authService.LoginAsync(loginDto);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// User registration
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        try
        {
            var response = await _authService.RegisterAsync(registerDto);
            return CreatedAtAction(nameof(GetProfile), new { }, response);
        }
        catch (InvalidOperationException ex)
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
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> UpdateProfile(UpdateUserDto updateUserDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _authService.UpdateUserAsync(userId, updateUserDto);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Change password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
        if (!result)
        {
            return BadRequest(
                new { message = "Failed to change password. Please check your current password." }
            );
        }

        return Ok(new { message = "Password changed successfully." });
    }

    /// <summary>
    /// Forgot password
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
    {
        await _authService.ForgotPasswordAsync(forgotPasswordDto);
        return Ok(new { message = "If the email exists, a password reset link has been sent." });
    }

    /// <summary>
    /// Reset password
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var result = await _authService.ResetPasswordAsync(resetPasswordDto);
        if (!result)
        {
            return BadRequest(
                new { message = "Failed to reset password. Invalid token or email." }
            );
        }

        return Ok(new { message = "Password reset successfully." });
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    [HttpGet("users")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers(
        [FromQuery] UserFilterDto filter,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "Id",
        [FromQuery] bool isAscending = true
    )
    {
        var users = await _authService.GetUsersAsync(filter, page, pageSize, sortBy, isAscending);
        return Ok(users);
    }

    /// <summary>
    /// Get user by ID (Admin only)
    /// </summary>
    [HttpGet("users/{userId}")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public async Task<ActionResult<UserResponseDto>> GetUser(string userId)
    {
        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Update user (Admin only)
    /// </summary>
    [HttpPut("users/{userId}")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public async Task<ActionResult<UserResponseDto>> UpdateUser(
        string userId,
        UpdateUserDto updateUserDto
    )
    {
        var user = await _authService.UpdateUserAsync(userId, updateUserDto);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Assign role to user (Admin only)
    /// </summary>
    [HttpPost("users/assign-role")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public async Task<IActionResult> AssignRole(AssignRoleDto assignRoleDto)
    {
        var result = await _authService.AssignRoleAsync(assignRoleDto);
        if (!result)
        {
            return BadRequest(new { message = "Failed to assign role." });
        }

        return Ok(new { message = "Role assigned successfully." });
    }

    /// <summary>
    /// Remove role from user (Admin only)
    /// </summary>
    [HttpDelete("users/{userId}/roles/{role}")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public async Task<IActionResult> RemoveRole(string userId, string role)
    {
        var result = await _authService.RemoveRoleAsync(userId, role);
        if (!result)
        {
            return BadRequest(new { message = "Failed to remove role." });
        }

        return Ok(new { message = "Role removed successfully." });
    }

    /// <summary>
    /// Get user roles (Admin only)
    /// </summary>
    [HttpGet("users/{userId}/roles")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(string userId)
    {
        var roles = await _authService.GetUserRolesAsync(userId);
        return Ok(roles);
    }

    /// <summary>
    /// Deactivate user (Admin only)
    /// </summary>
    [HttpPatch("users/{userId}/deactivate")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public async Task<IActionResult> DeactivateUser(string userId)
    {
        var result = await _authService.DeactivateUserAsync(userId);
        if (!result)
        {
            return NotFound();
        }

        return Ok(new { message = "User deactivated successfully." });
    }

    /// <summary>
    /// Activate user (Admin only)
    /// </summary>
    [HttpPatch("users/{userId}/activate")]
    [Authorize(Roles = ClinicRoles.Admin)]
    public async Task<IActionResult> ActivateUser(string userId)
    {
        var result = await _authService.ActivateUserAsync(userId);
        if (!result)
        {
            return NotFound();
        }

        return Ok(new { message = "User activated successfully." });
    }

    /// <summary>
    /// Get available roles (Admin/Manager only)
    /// </summary>
    [HttpGet("roles")]
    [Authorize(Roles = $"{ClinicRoles.Admin},{ClinicRoles.Manager}")]
    public ActionResult<object> GetRoles()
    {
        var roles = ClinicRoles.AllRoles.Select(role => new
        {
            Name = role,
            Description = ClinicRoles.RoleDescriptions.GetValueOrDefault(role, ""),
        });

        return Ok(roles);
    }
}
