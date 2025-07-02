using ClinicApi.Application.DTOs;
using MediatR;

namespace ClinicApi.Application.Commands.Patients;

public record CreatePatientCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth,
    string Gender,
    string? Address = null,
    string? BloodType = null,
    string? MedicalHistory = null,
    string? Allergies = null,
    string? EmergencyContact = null,
    string? EmergencyContactPhone = null
) : IRequest<PatientResponseDto>; 