using AutoMapper;
using ClinicApi.Application.Commands.Auth;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Services;
using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClinicApi.Application.Handlers.Auth;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IMapper mapper
    )
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with email {request.Email} already exists.");
        }

        // Validate role
        var validRoles = new[]
        {
            ClinicRoles.Admin,
            ClinicRoles.Doctor,
            ClinicRoles.Patient,
            ClinicRoles.Nurse,
            ClinicRoles.Receptionist,
            ClinicRoles.Manager,
        };
        if (!validRoles.Contains(request.Role))
        {
            throw new ArgumentException($"Invalid role: {request.Role}");
        }

        // Create new user
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Gender = request.Gender,
            Address = request.Address,
            Department = request.Department,
            EmployeeId = request.EmployeeId,
            EmailConfirmed = true, // For development - in production, implement email confirmation
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        // Create user with password
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        // Add user to role
        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!roleResult.Succeeded)
        {
            // If role assignment fails, delete the user and throw error
            await _userManager.DeleteAsync(user);
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to assign role: {errors}");
        }

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate JWT token
        var token = await _jwtTokenService.GenerateTokenAsync(user, roles);

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
