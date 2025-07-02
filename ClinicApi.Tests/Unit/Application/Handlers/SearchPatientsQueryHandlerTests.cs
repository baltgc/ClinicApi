using AutoMapper;
using ClinicApi.Application.DTOs;
using ClinicApi.Application.Handlers.Patients;
using ClinicApi.Application.Queries.Patients;
using ClinicApi.Domain.Entities;
using ClinicApi.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ClinicApi.Tests.Unit.Application.Handlers;

public class SearchPatientsQueryHandlerTests
{
    private readonly Mock<IPatientRepository> _mockPatientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly SearchPatientsQueryHandler _handler;

    public SearchPatientsQueryHandlerTests()
    {
        _mockPatientRepository = new Mock<IPatientRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new SearchPatientsQueryHandler(
            _mockPatientRepository.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task Handle_ValidSearchTerm_ShouldReturnMatchingPatients()
    {
        // Arrange
        var searchTerm = "john";
        var query = new SearchPatientsQuery(searchTerm);

        var patients = new List<Patient>
        {
            new Patient
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com",
                IsActive = true,
            },
            new Patient
            {
                Id = 2,
                FirstName = "Johnny",
                LastName = "Smith",
                Email = "johnny.smith@email.com",
                IsActive = true,
            },
        };

        var expectedDtos = new List<PatientResponseDto>
        {
            new PatientResponseDto(
                1,
                "John",
                "Doe",
                "john.doe@email.com",
                "1234567890",
                DateTime.Now,
                "Male",
                "",
                "",
                "",
                "",
                "",
                "",
                true,
                DateTime.Now,
                DateTime.Now
            ),
            new PatientResponseDto(
                2,
                "Johnny",
                "Smith",
                "johnny.smith@email.com",
                "0987654321",
                DateTime.Now,
                "Male",
                "",
                "",
                "",
                "",
                "",
                "",
                true,
                DateTime.Now,
                DateTime.Now
            ),
        };

        _mockPatientRepository.Setup(x => x.SearchPatientsAsync(searchTerm)).ReturnsAsync(patients);

        _mockMapper
            .Setup(x => x.Map<IEnumerable<PatientResponseDto>>(patients))
            .Returns(expectedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().FirstName.Should().Be("John");
        result.Last().FirstName.Should().Be("Johnny");

        _mockPatientRepository.Verify(x => x.SearchPatientsAsync(searchTerm), Times.Once);
        _mockMapper.Verify(x => x.Map<IEnumerable<PatientResponseDto>>(patients), Times.Once);
    }

    [Fact]
    public async Task Handle_NoMatchesFound_ShouldReturnEmptyCollection()
    {
        // Arrange
        var searchTerm = "nonexistent";
        var query = new SearchPatientsQuery(searchTerm);
        var emptyList = new List<Patient>();
        var emptyDtoList = new List<PatientResponseDto>();

        _mockPatientRepository
            .Setup(x => x.SearchPatientsAsync(searchTerm))
            .ReturnsAsync(emptyList);

        _mockMapper
            .Setup(x => x.Map<IEnumerable<PatientResponseDto>>(emptyList))
            .Returns(emptyDtoList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockPatientRepository.Verify(x => x.SearchPatientsAsync(searchTerm), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Handle_InvalidSearchTerm_ShouldThrowArgumentException(string invalidTerm)
    {
        // Arrange
        var query = new SearchPatientsQuery(invalidTerm);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );
    }
}
