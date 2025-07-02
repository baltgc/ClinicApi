using ClinicApi.Application.DTOs;
using MediatR;

namespace ClinicApi.Application.Queries.Appointments;

public record GetAppointmentsByPatientQuery(int PatientId) : IRequest<IEnumerable<AppointmentResponseDto>>; 