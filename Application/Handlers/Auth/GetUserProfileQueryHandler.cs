using AutoMapper;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Queries.Auth;
using ClinicApi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ClinicApi.Application.Handlers.Auth;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserResponseDto?>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public GetUserProfileQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<UserResponseDto?> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new UserResponseDto(
            user.Id,
            user.Email ?? string.Empty,
            user.FirstName,
            user.LastName,
            user.PhoneNumber ?? string.Empty,
            user.Department,
            user.EmployeeId,
            true,
            roles
        );
    }
}
