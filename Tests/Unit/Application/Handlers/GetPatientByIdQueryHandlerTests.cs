using AutoMapper;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Handlers.Patients;
using ClinicApi.Application.Queries.Patients;
using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Tests.Unit.Application.Handlers;

public class GetPatientByIdQueryHandlerTests
{
    private readonly Mock<IPatientRepository> _mockPatientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetPatientByIdQueryHandler _handler;

    public GetPatientByIdQueryHandlerTests()
    {
        _mockPatientRepository = new Mock<IPatientRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetPatientByIdQueryHandler(
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
    public async Task Handle_ValidId_ShouldReturnPatientDto()
    {
        // Arrange
        var patientId = 1;
        var query = new GetPatientByIdQuery(patientId);

        var patient = new Patient
        {
            Id = patientId,
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
            UpdatedAt = DateTime.UtcNow,
        };

        var expectedDto = new PatientResponseDto(
            patientId,
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
            DateTime.UtcNow,
            DateTime.UtcNow
        );

        _mockPatientRepository.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync(patient);

        _mockMapper.Setup(x => x.Map<PatientResponseDto>(patient)).Returns(expectedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(patientId);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john.doe@email.com");

        _mockPatientRepository.Verify(x => x.GetByIdAsync(patientId), Times.Once);
        _mockMapper.Verify(x => x.Map<PatientResponseDto>(patient), Times.Once);
    }

    [Test]
    public async Task Handle_NonExistentId_ShouldReturnNull()
    {
        // Arrange
        var patientId = 999;
        var query = new GetPatientByIdQuery(patientId);

        _mockPatientRepository.Setup(x => x.GetByIdAsync(patientId)).ReturnsAsync((Patient)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _mockPatientRepository.Verify(x => x.GetByIdAsync(patientId), Times.Once);
        _mockMapper.Verify(x => x.Map<PatientResponseDto>(It.IsAny<Patient>()), Times.Never);
    }

    [Test]
    public void Handle_RepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var patientId = 1;
        var query = new GetPatientByIdQuery(patientId);

        _mockPatientRepository
            .Setup(x => x.GetByIdAsync(patientId))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );

        _mockPatientRepository.Verify(x => x.GetByIdAsync(patientId), Times.Once);
        _mockMapper.Verify(x => x.Map<PatientResponseDto>(It.IsAny<Patient>()), Times.Never);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-999)]
    public async Task Handle_InvalidId_ShouldReturnNull(int invalidId)
    {
        // Arrange
        var query = new GetPatientByIdQuery(invalidId);

        _mockPatientRepository.Setup(x => x.GetByIdAsync(invalidId)).ReturnsAsync((Patient)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _mockPatientRepository.Verify(x => x.GetByIdAsync(invalidId), Times.Once);
    }
}
