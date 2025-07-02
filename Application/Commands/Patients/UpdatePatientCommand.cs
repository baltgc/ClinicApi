using ClinicApi.Application.DTOs;
using MediatR;

namespace ClinicApi.Application.Commands.Patients;

public record UpdatePatientCommand(
    int Id,
    string? FirstName = null,
    string? LastName = null,
    string? Email = null,
    string? PhoneNumber = null,
    DateTime? DateOfBirth = null,
    string? Gender = null,
    string? Address = null,
    string? BloodType = null,
    string? MedicalHistory = null,
    string? Allergies = null,
    string? EmergencyContact = null,
    string? EmergencyContactPhone = null,
    bool? IsActive = null
) : IRequest<PatientResponseDto?>; 