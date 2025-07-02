using ClinicApi.Application.Commands.Auth;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Services;
using ClinicApi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClinicApi.Application.Handlers.Auth;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService
    )
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        // Find user by email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // Check password
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // Check if account is locked out
        if (await _userManager.IsLockedOutAsync(user))
        {
            throw new UnauthorizedAccessException("Account is locked out. Please try again later.");
        }

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate JWT token
        var token = await _jwtTokenService.GenerateTokenAsync(user, roles);

        // Update last login time
        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto(
            token,
            DateTime.UtcNow.AddHours(24),
            new UserResponseDto(
                user.Id,
                user.Email ?? string.Empty,
                user.FirstName,
                user.LastName,
                user.PhoneNumber ?? string.Empty,
                user.Department,
                user.EmployeeId,
                true,
                roles
            )
        );
    }
}
