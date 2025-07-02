using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Authenticate user and return JWT token
    /// </summary>
    [HttpPost("login")]
    public IActionResult Login()
    {
        // TODO: Implement with MediatR command
        return Ok("Login endpoint - TODO: Implement with CQRS");
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Register()
    {
        // TODO: Implement with MediatR command
        return Ok("Register endpoint - TODO: Implement with CQRS");
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        // TODO: Implement with MediatR query
        return Ok("Profile endpoint - TODO: Implement with CQRS");
    }
}
