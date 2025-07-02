using ClinicApi.Application.DTOs;
using MediatR;

namespace ClinicApi.Application.Queries.Auth;

public record GetUserProfileQuery(string UserId) : IRequest<UserResponseDto?>;
