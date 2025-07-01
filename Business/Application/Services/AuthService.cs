using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using ClinicApi.Business.Application.DTOs;
using ClinicApi.Business.Application.Interfaces;
using ClinicApi.Business.Domain.Constants;
using ClinicApi.Business.Domain.Models;
using ClinicApi.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ClinicApi.Business.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ClinicDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IMapper mapper,
        ClinicDbContext context
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _mapper = mapper;
        _context = context;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = await GenerateJwtToken(user, roles);

        return new AuthResponseDto(
            Token: token,
            Expiration: DateTime.UtcNow.AddHours(GetJwtExpirationHours()),
            UserId: user.Id,
            Email: user.Email!,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Roles: roles.ToArray(),
            PatientId: user.PatientId,
            DoctorId: user.DoctorId
        );
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            PhoneNumber = registerDto.PhoneNumber,
            DateOfBirth = registerDto.DateOfBirth,
            Gender = registerDto.Gender,
            Address = registerDto.Address,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            EmailConfirmed = true, // Auto-confirm for clinic registration
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        // Assign role
        if (ClinicRoles.AllRoles.Contains(registerDto.Role))
        {
            await _userManager.AddToRoleAsync(user, registerDto.Role);
        }
        else
        {
            await _userManager.AddToRoleAsync(user, ClinicRoles.Patient);
        }

        // If registering as Patient, create Patient entity
        if (registerDto.Role == ClinicRoles.Patient)
        {
            var patient = new Patient
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber ?? "",
                DateOfBirth = registerDto.DateOfBirth ?? DateTime.MinValue,
                Gender = registerDto.Gender ?? "",
                Address = registerDto.Address,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            user.PatientId = patient.Id;
            await _userManager.UpdateAsync(user);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = await GenerateJwtToken(user, roles);

        return new AuthResponseDto(
            Token: token,
            Expiration: DateTime.UtcNow.AddHours(GetJwtExpirationHours()),
            UserId: user.Id,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Roles: roles.ToArray(),
            PatientId: user.PatientId,
            DoctorId: user.DoctorId
        );
    }

    public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ChangePasswordAsync(
            user,
            changePasswordDto.CurrentPassword,
            changePasswordDto.NewPassword
        );
        return result.Succeeded;
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
        if (user == null)
        {
            return false; // Don't reveal that user doesn't exist
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // TODO: Send email with reset token
        // For now, we'll just log it (in production, send via email service)
        Console.WriteLine($"Password reset token for {user.Email}: {token}");

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ResetPasswordAsync(
            user,
            resetPasswordDto.Token,
            resetPasswordDto.NewPassword
        );
        return result.Succeeded;
    }

    public async Task<string> GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey =
            jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"] ?? "ClinicApi";
        var audience = jwtSettings["Audience"] ?? "ClinicApi";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("firstName", user.FirstName),
            new("lastName", user.LastName),
            new("fullName", user.FullName),
        };

        // Add role claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Add clinic-specific claims
        if (user.PatientId.HasValue)
        {
            claims.Add(new Claim("patientId", user.PatientId.Value.ToString()));
        }

        if (user.DoctorId.HasValue)
        {
            claims.Add(new Claim("doctorId", user.DoctorId.Value.ToString()));
        }

        if (!string.IsNullOrEmpty(user.EmployeeId))
        {
            claims.Add(new Claim("employeeId", user.EmployeeId));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(GetJwtExpirationHours()),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager
            .Users.Include(u => u.Patient)
            .Include(u => u.Doctor)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new UserResponseDto(
            Id: user.Id,
            Email: user.Email!,
            FirstName: user.FirstName,
            LastName: user.LastName,
            PhoneNumber: user.PhoneNumber,
            DateOfBirth: user.DateOfBirth,
            Gender: user.Gender,
            Address: user.Address,
            Department: user.Department,
            EmployeeId: user.EmployeeId,
            CreatedAt: user.CreatedAt,
            UpdatedAt: user.UpdatedAt,
            IsActive: user.IsActive,
            Roles: roles.ToArray(),
            PatientId: user.PatientId,
            DoctorId: user.DoctorId
        );
    }

    public async Task<IEnumerable<UserResponseDto>> GetUsersAsync(
        UserFilterDto filter,
        int page,
        int pageSize,
        string sortBy,
        bool isAscending
    )
    {
        var query = _userManager.Users.Include(u => u.Patient).Include(u => u.Doctor).AsQueryable();

        if (filter.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == filter.IsActive.Value);
        }

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(searchTerm)
                || u.LastName.ToLower().Contains(searchTerm)
                || u.Email!.ToLower().Contains(searchTerm)
            );
        }

        var users = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var userDtos = new List<UserResponseDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (!string.IsNullOrEmpty(filter.Role) && !roles.Contains(filter.Role))
            {
                continue;
            }

            userDtos.Add(
                new UserResponseDto(
                    Id: user.Id,
                    Email: user.Email!,
                    FirstName: user.FirstName,
                    LastName: user.LastName,
                    PhoneNumber: user.PhoneNumber,
                    DateOfBirth: user.DateOfBirth,
                    Gender: user.Gender,
                    Address: user.Address,
                    Department: user.Department,
                    EmployeeId: user.EmployeeId,
                    CreatedAt: user.CreatedAt,
                    UpdatedAt: user.UpdatedAt,
                    IsActive: user.IsActive,
                    Roles: roles.ToArray(),
                    PatientId: user.PatientId,
                    DoctorId: user.DoctorId
                )
            );
        }

        return userDtos;
    }

    public async Task<UserResponseDto?> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(updateUserDto.FirstName))
            user.FirstName = updateUserDto.FirstName;
        if (!string.IsNullOrEmpty(updateUserDto.LastName))
            user.LastName = updateUserDto.LastName;
        if (updateUserDto.PhoneNumber != null)
            user.PhoneNumber = updateUserDto.PhoneNumber;
        if (updateUserDto.DateOfBirth.HasValue)
            user.DateOfBirth = updateUserDto.DateOfBirth;
        if (updateUserDto.Gender != null)
            user.Gender = updateUserDto.Gender;
        if (updateUserDto.Address != null)
            user.Address = updateUserDto.Address;
        if (updateUserDto.Department != null)
            user.Department = updateUserDto.Department;

        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return null;
        }

        return await GetUserByIdAsync(userId);
    }

    public async Task<bool> AssignRoleAsync(AssignRoleDto assignRoleDto)
    {
        var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);
        if (user == null)
        {
            return false;
        }

        if (!ClinicRoles.AllRoles.Contains(assignRoleDto.Role))
        {
            return false;
        }

        var result = await _userManager.AddToRoleAsync(user, assignRoleDto.Role);
        return result.Succeeded;
    }

    public async Task<bool> RemoveRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        return result.Succeeded;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Array.Empty<string>();
        }

        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> DeactivateUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> ActivateUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    private int GetJwtExpirationHours()
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        return int.TryParse(jwtSettings["ExpirationHours"], out var hours) ? hours : 24;
    }
}
