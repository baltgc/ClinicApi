using ClinicApi.Application.DTOs;
using MediatR;

namespace ClinicApi.Application.Commands.Auth;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string PhoneNumber,
    string Role,
    string? Gender = null,
    string? Address = null,
    string? Department = null,
    string? EmployeeId = null
) : IRequest<AuthResponseDto>;
