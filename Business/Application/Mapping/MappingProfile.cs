using AutoMapper;
using ClinicApi.Business.Application.DTOs;
using ClinicApi.Business.Domain.Models;

namespace ClinicApi.Business.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Patient mappings
        CreateMap<Patient, PatientResponseDto>();
        CreateMap<CreatePatientDto, Patient>();
        CreateMap<UpdatePatientDto, Patient>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Doctor mappings
        CreateMap<Doctor, DoctorResponseDto>();
        CreateMap<CreateDoctorDto, Doctor>();
        CreateMap<UpdateDoctorDto, Doctor>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Appointment mappings
        CreateMap<Appointment, AppointmentResponseDto>();
        CreateMap<CreateAppointmentDto, Appointment>();
        CreateMap<UpdateAppointmentDto, Appointment>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // MedicalRecord mappings
        CreateMap<MedicalRecord, MedicalRecordResponseDto>();
        CreateMap<CreateMedicalRecordDto, MedicalRecord>()
            .ForMember(
                dest => dest.VisitDate,
                opt => opt.MapFrom(src => src.VisitDate ?? DateTime.UtcNow)
            );
        CreateMap<UpdateMedicalRecordDto, MedicalRecord>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Prescription mappings
        CreateMap<Prescription, PrescriptionResponseDto>();
        CreateMap<CreatePrescriptionDto, Prescription>()
            .ForMember(dest => dest.PrescribedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(
                dest => dest.StartDate,
                opt => opt.MapFrom(src => src.StartDate ?? DateTime.UtcNow)
            )
            .ForMember(
                dest => dest.EndDate,
                opt =>
                    opt.MapFrom(src =>
                        src.StartDate.HasValue
                            ? src.StartDate.Value.AddDays(src.DurationDays)
                            : DateTime.UtcNow.AddDays(src.DurationDays)
                    )
            );
        CreateMap<UpdatePrescriptionDto, Prescription>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // DoctorSchedule mappings
        CreateMap<DoctorSchedule, DoctorSchedule>();

        // ApplicationUser mappings
        CreateMap<ApplicationUser, UserResponseDto>()
            .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Roles are handled separately

        CreateMap<RegisterDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true));

        CreateMap<UpdateUserDto, ApplicationUser>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
