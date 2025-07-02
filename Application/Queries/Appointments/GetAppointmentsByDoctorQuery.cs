using ClinicApi.Application.DTOs;
using MediatR;

namespace ClinicApi.Application.Queries.Appointments;

public record GetAppointmentsByDoctorQuery(int DoctorId)
    : IRequest<IEnumerable<AppointmentResponseDto>>;
