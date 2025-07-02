using AutoMapper;
using ClinicApi.Application.Commands.Appointments;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Handlers.Appointments;
using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Enums;
using ClinicApi.Domain.Interfaces;
using ClinicApi.Domain.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ClinicApi.Tests.Unit.Application.Handlers;

public class CreateAppointmentCommandHandlerTests
{
    private readonly Mock<IAppointmentRepository> _mockAppointmentRepository;
    private readonly Mock<IPatientRepository> _mockPatientRepository;
    private readonly Mock<IDoctorRepository> _mockDoctorRepository;
    private readonly Mock<IAppointmentDomainService> _mockAppointmentDomainService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CreateAppointmentCommandHandler _handler;

    public CreateAppointmentCommandHandlerTests()
    {
        _mockAppointmentRepository = new Mock<IAppointmentRepository>();
        _mockPatientRepository = new Mock<IPatientRepository>();
        _mockDoctorRepository = new Mock<IDoctorRepository>();
        _mockAppointmentDomainService = new Mock<IAppointmentDomainService>();
        _mockMapper = new Mock<IMapper>();

        _handler = new CreateAppointmentCommandHandler(
            _mockAppointmentRepository.Object,
            _mockPatientRepository.Object,
            _mockDoctorRepository.Object,
            _mockAppointmentDomainService.Object,
            _mockMapper.Object
        );
    }

    [SetUp]
    public void SetUp()
    {
        // Reset all mocks before each test
        _mockAppointmentRepository.Reset();
        _mockPatientRepository.Reset();
        _mockDoctorRepository.Reset();
        _mockAppointmentDomainService.Reset();
        _mockMapper.Reset();
    }

    [Test]
    public async Task Handle_ValidCommand_ShouldCreateAppointmentAndReturnDto()
    {
        // Arrange
        var appointmentDate = DateTime.UtcNow.AddDays(1);
        var command = new CreateAppointmentCommand(
            1, // PatientId
            2, // DoctorId
            appointmentDate,
            TimeSpan.FromMinutes(30),
            "Regular checkup",
            "Annual physical examination"
        );

        var patient = new Patient
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
        };
        var doctor = new Doctor
        {
            Id = 2,
            FirstName = "Dr. Jane",
            LastName = "Smith",
        };

        var appointment = new Appointment
        {
            Id = 1,
            PatientId = 1,
            DoctorId = 2,
            AppointmentDate = appointmentDate,
            Duration = TimeSpan.FromMinutes(30),
            ReasonForVisit = "Regular checkup",
            Notes = "Annual physical examination",
            Status = AppointmentStatus.Scheduled,
            CreatedAt = DateTime.UtcNow,
        };

        var expectedDto = new AppointmentResponseDto(
            1,
            1,
            2,
            appointmentDate,
            TimeSpan.FromMinutes(30),
            AppointmentStatus.Scheduled,
            "Regular checkup",
            "Annual physical examination",
            null,
            100.0m,
            DateTime.UtcNow,
            DateTime.UtcNow
        );

        // Setup mocks
        _mockPatientRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(patient);

        _mockDoctorRepository.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(doctor);

        _mockAppointmentDomainService
            .Setup(x =>
                x.CanScheduleAppointmentAsync(
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<TimeSpan>()
                )
            )
            .ReturnsAsync(true);

        _mockMapper.Setup(x => x.Map<Appointment>(command)).Returns(appointment);

        _mockAppointmentRepository
            .Setup(x => x.AddAsync(It.IsAny<Appointment>()))
            .ReturnsAsync(appointment);

        _mockAppointmentRepository
            .Setup(x => x.GetWithDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync(appointment);

        _mockMapper.Setup(x => x.Map<AppointmentResponseDto>(appointment)).Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.PatientId.Should().Be(1);
        result.DoctorId.Should().Be(2);
        result.ReasonForVisit.Should().Be("Regular checkup");
        result.Notes.Should().Be("Annual physical examination");
        result.Status.Should().Be(AppointmentStatus.Scheduled);

        // Verify all mocks were called
        _mockPatientRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        _mockDoctorRepository.Verify(x => x.GetByIdAsync(2), Times.Once);
        _mockAppointmentDomainService.Verify(
            x =>
                x.CanScheduleAppointmentAsync(
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<TimeSpan>()
                ),
            Times.Once
        );
        _mockAppointmentRepository.Verify(x => x.AddAsync(It.IsAny<Appointment>()), Times.Once);
    }

    [Test]
    public async Task Handle_NonExistentPatient_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            999, // Non-existent PatientId
            2,
            DateTime.UtcNow.AddDays(1),
            TimeSpan.FromMinutes(30),
            "Regular checkup",
            "Notes"
        );

        _mockPatientRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Patient?)null);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        _mockPatientRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
        _mockDoctorRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task Handle_NonExistentDoctor_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            1,
            999, // Non-existent DoctorId
            DateTime.UtcNow.AddDays(1),
            TimeSpan.FromMinutes(30),
            "Regular checkup",
            "Notes"
        );

        var patient = new Patient { Id = 1 };

        _mockPatientRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(patient);

        _mockDoctorRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Doctor?)null);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        _mockPatientRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        _mockDoctorRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
    }

    [Test]
    public async Task Handle_AppointmentConflict_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            1,
            2,
            DateTime.UtcNow.AddDays(1),
            TimeSpan.FromMinutes(30),
            "Regular checkup",
            "Notes"
        );

        var patient = new Patient { Id = 1 };
        var doctor = new Doctor { Id = 2 };

        _mockPatientRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(patient);

        _mockDoctorRepository.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(doctor);

        _mockAppointmentDomainService
            .Setup(x =>
                x.CanScheduleAppointmentAsync(
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<TimeSpan>()
                )
            )
            .ReturnsAsync(false); // Simulate conflict

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        _mockAppointmentDomainService.Verify(
            x =>
                x.CanScheduleAppointmentAsync(
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<TimeSpan>()
                ),
            Times.Once
        );
        _mockAppointmentRepository.Verify(x => x.AddAsync(It.IsAny<Appointment>()), Times.Never);
    }

    [Test]
    public async Task Handle_PastDate_ShouldThrowArgumentException()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var command = new CreateAppointmentCommand(
            1,
            2,
            pastDate,
            TimeSpan.FromMinutes(30),
            "Regular checkup",
            "Notes"
        );

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }
}
