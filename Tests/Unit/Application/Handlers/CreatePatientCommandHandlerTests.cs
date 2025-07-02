using AutoMapper;
using ClinicApi.Application.Commands.Patients;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Handlers.Patients;
using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Tests.Unit.Application.Handlers;

public class CreatePatientCommandHandlerTests
{
    private readonly Mock<IPatientRepository> _mockPatientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CreatePatientCommandHandler _handler;

    public CreatePatientCommandHandlerTests()
    {
        _mockPatientRepository = new Mock<IPatientRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new CreatePatientCommandHandler(
            _mockPatientRepository.Object,
            _mockMapper.Object
        );
    }

    [SetUp]
    public void SetUp()
    {
        // Reset all mocks before each test
        _mockPatientRepository.Reset();
        _mockMapper.Reset();
    }

    [Test]
    public async Task Handle_ValidCommand_ShouldCreatePatientAndReturnDto()
    {
        // Arrange
        var command = new CreatePatientCommand(
            "John",
            "Doe",
            "john.doe@email.com",
            "1234567890",
            new DateTime(1990, 1, 1),
            "Male",
            "123 Main St",
            "O+",
            "No medical history",
            "None",
            "Jane Doe",
            "0987654321"
        );

        var patient = new Patient
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@email.com",
            PhoneNumber = "1234567890",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Male",
            Address = "123 Main St",
            BloodType = "O+",
            MedicalHistory = "No medical history",
            Allergies = "None",
            EmergencyContact = "Jane Doe",
            EmergencyContactPhone = "0987654321",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        var createdAt = DateTime.UtcNow;
        var expectedDto = new PatientResponseDto(
            1,
            "John",
            "Doe",
            "john.doe@email.com",
            "1234567890",
            new DateTime(1990, 1, 1),
            "Male",
            "123 Main St",
            "O+",
            "No medical history",
            "None",
            "Jane Doe",
            "0987654321",
            true,
            createdAt,
            createdAt
        );

        _mockMapper.Setup(x => x.Map<Patient>(command)).Returns(patient);

        _mockPatientRepository.Setup(x => x.AddAsync(It.IsAny<Patient>())).ReturnsAsync(patient);

        _mockMapper.Setup(x => x.Map<PatientResponseDto>(patient)).Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john.doe@email.com");
        result.IsActive.Should().BeTrue();

        _mockPatientRepository.Verify(x => x.AddAsync(It.IsAny<Patient>()), Times.Once);
        _mockMapper.Verify(x => x.Map<Patient>(command), Times.Once);
        _mockMapper.Verify(x => x.Map<PatientResponseDto>(patient), Times.Once);
    }

    [TestCase("", "Doe", "john.doe@email.com")]
    [TestCase("John", "", "john.doe@email.com")]
    [TestCase("John", "Doe", "")]
    public async Task Handle_InvalidCommand_ShouldThrowArgumentException(
        string firstName,
        string lastName,
        string email
    )
    {
        // Arrange
        var command = new CreatePatientCommand(
            firstName,
            lastName,
            email,
            "1234567890",
            new DateTime(1990, 1, 1),
            "Male",
            "123 Main St",
            "O+",
            "No medical history",
            "None",
            "Jane Doe",
            "0987654321"
        );

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Test]
    public async Task Handle_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new CreatePatientCommand(
            "John",
            "Doe",
            "john.doe@email.com",
            "1234567890",
            new DateTime(1990, 1, 1),
            "Male",
            "123 Main St",
            "O+",
            "No medical history",
            "None",
            "Jane Doe",
            "0987654321"
        );

        var patient = new Patient();
        _mockMapper.Setup(x => x.Map<Patient>(command)).Returns(patient);
        _mockPatientRepository
            .Setup(x => x.AddAsync(It.IsAny<Patient>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Test]
    public async Task Handle_ValidCommand_ShouldSetDefaultValues()
    {
        // Arrange
        var command = new CreatePatientCommand(
            "John",
            "Doe",
            "john.doe@email.com",
            "1234567890",
            new DateTime(1990, 1, 1),
            "Male",
            "123 Main St",
            "O+",
            "No medical history",
            "None",
            "Jane Doe",
            "0987654321"
        );

        var capturedPatient = new Patient();
        var currentTime = DateTime.UtcNow;

        _mockMapper.Setup(x => x.Map<Patient>(command)).Returns(capturedPatient);

        _mockPatientRepository
            .Setup(x => x.AddAsync(It.IsAny<Patient>()))
            .Callback<Patient>(p => capturedPatient = p)
            .ReturnsAsync(capturedPatient);

        _mockMapper
            .Setup(x => x.Map<PatientResponseDto>(It.IsAny<Patient>()))
            .Returns(
                new PatientResponseDto(
                    1,
                    "John",
                    "Doe",
                    "john.doe@email.com",
                    "1234567890",
                    new DateTime(1990, 1, 1),
                    "Male",
                    "123 Main St",
                    "O+",
                    "No medical history",
                    "None",
                    "Jane Doe",
                    "0987654321",
                    true,
                    currentTime,
                    currentTime
                )
            );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedPatient.IsActive.Should().BeTrue();
        capturedPatient.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
