using ClinicApi.Application.DTOs;
using MediatR;

namespace ClinicApi.Application.Commands.Auth;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;
