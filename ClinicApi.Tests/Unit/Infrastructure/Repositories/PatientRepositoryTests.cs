using ClinicApi.Domain.Entities;
using ClinicApi.Infrastructure.Data.Context;
using ClinicApi.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace ClinicApi.Tests.Unit.Infrastructure.Repositories;

public class PatientRepositoryTests : IDisposable
{
    private readonly ClinicDbContext _context;
    private readonly PatientRepository _repository;

    public PatientRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ClinicDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ClinicDbContext(options);
        _repository = new PatientRepository(_context);
    }

    [SetUp]
    public void SetUp()
    {
        // Clear all data before each test to ensure isolation
        _context.Patients.RemoveRange(_context.Patients);
        _context.SaveChanges();
    }

    [Test]
    public async Task AddAsync_ValidPatient_ShouldAddToDatabase()
    {
        // Arrange
        var patient = new Patient
        {
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

        // Act
        var result = await _repository.AddAsync(patient);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john.doe@email.com");

        var savedPatient = await _context.Patients.FindAsync(result.Id);
        savedPatient.Should().NotBeNull();
        savedPatient!.FirstName.Should().Be("John");
    }

    [Test]
    public async Task GetByIdAsync_ExistingPatient_ShouldReturnPatient()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@email.com",
            PhoneNumber = "0987654321",
            DateOfBirth = new DateTime(1985, 5, 15),
            Gender = "Female",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(patient.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(patient.Id);
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
        result.Email.Should().Be("jane.smith@email.com");
    }

    [Test]
    public async Task GetByIdAsync_NonExistentPatient_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetByEmailAsync_ExistingEmail_ShouldReturnPatient()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "Bob",
            LastName = "Johnson",
            Email = "bob.johnson@email.com",
            PhoneNumber = "5555551234",
            DateOfBirth = new DateTime(1992, 8, 20),
            Gender = "Male",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("bob.johnson@email.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("bob.johnson@email.com");
        result.FirstName.Should().Be("Bob");
        result.LastName.Should().Be("Johnson");
    }

    [Test]
    public async Task GetByEmailAsync_NonExistentEmail_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@email.com");

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task EmailExistsAsync_ExistingEmail_ShouldReturnTrue()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "Alice",
            LastName = "Brown",
            Email = "alice.brown@email.com",
            PhoneNumber = "1111111111",
            DateOfBirth = new DateTime(1988, 3, 10),
            Gender = "Female",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.EmailExistsAsync("alice.brown@email.com");

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task EmailExistsAsync_NonExistentEmail_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.EmailExistsAsync("nonexistent@email.com");

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task EmailExistsAsync_ExcludeId_ShouldExcludeSpecificPatient()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "Charlie",
            LastName = "Davis",
            Email = "charlie.davis@email.com",
            PhoneNumber = "2222222222",
            DateOfBirth = new DateTime(1995, 12, 5),
            Gender = "Male",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.EmailExistsAsync("charlie.davis@email.com", patient.Id);

        // Assert
        result.Should().BeFalse(); // Should return false because we excluded this patient's ID
    }

    [Test]
    public async Task GetActivePatientsAsync_ShouldReturnOnlyActivePatients()
    {
        // Arrange
        var activePatient = new Patient
        {
            FirstName = "Active",
            LastName = "Patient",
            Email = "active@email.com",
            PhoneNumber = "3333333333",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "Male",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var inactivePatient = new Patient
        {
            FirstName = "Inactive",
            LastName = "Patient",
            Email = "inactive@email.com",
            PhoneNumber = "4444444444",
            DateOfBirth = new DateTime(1985, 1, 1),
            Gender = "Female",
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.Patients.AddRange(activePatient, inactivePatient);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActivePatientsAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().FirstName.Should().Be("Active");
        result.First().IsActive.Should().BeTrue();
    }

    [Test]
    public async Task SearchPatientsAsync_ShouldFindPatientsBySearchTerm()
    {
        // Arrange
        var patients = new[]
        {
            new Patient
            {
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@email.com",
                PhoneNumber = "1111111111",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Male",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new Patient
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@email.com",
                PhoneNumber = "2222222222",
                DateOfBirth = new DateTime(1985, 1, 1),
                Gender = "Female",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new Patient
            {
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob.johnson@email.com",
                PhoneNumber = "3333333333",
                DateOfBirth = new DateTime(1992, 1, 1),
                Gender = "Male",
                IsActive = false, // Inactive patient
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
        };

        _context.Patients.AddRange(patients);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchPatientsAsync("john");

        // Assert
        result.Should().HaveCount(1);
        result.First().FirstName.Should().Be("John");
        result.First().LastName.Should().Be("Smith");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
