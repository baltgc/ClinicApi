using ClinicApi.Business.Domain.Constants;
using ClinicApi.Business.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ClinicApi.Infrastructure.Data.Context;

public class ClinicDbContext : IdentityDbContext<ApplicationUser>
{
    public ClinicDbContext(DbContextOptions<ClinicDbContext> options)
        : base(options) { }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<DoctorSchedule> DoctorSchedules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning)
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ApplicationUser entity
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PhoneNumberSecondary).HasMaxLength(20);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.EmployeeId).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // Relationships with clinic entities
            entity
                .HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.SetNull);

            entity
                .HasOne(e => e.Doctor)
                .WithMany()
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.EmployeeId).IsUnique();
        });

        // Configure Patient entity
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Gender).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.BloodType).HasMaxLength(50);
            entity.Property(e => e.MedicalHistory).HasMaxLength(1000);
            entity.Property(e => e.Allergies).HasMaxLength(500);
            entity.Property(e => e.EmergencyContact).HasMaxLength(500);
            entity.Property(e => e.EmergencyContactPhone).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure Doctor entity
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Specialization).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LicenseNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Qualifications).HasMaxLength(200);
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.OfficeAddress).HasMaxLength(500);
            entity.Property(e => e.ConsultationFee).HasColumnType("decimal(18,2)");
            entity.Property(e => e.AvailableHours).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.LicenseNumber).IsUnique();
        });

        // Configure Appointment entity
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ReasonForVisit).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.CancellationReason).HasMaxLength(500);
            entity.Property(e => e.ConsultationFee).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity
                .HasOne(e => e.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure MedicalRecord entity
        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ChiefComplaint).HasMaxLength(100);
            entity.Property(e => e.Symptoms).HasMaxLength(1000);
            entity.Property(e => e.Diagnosis).HasMaxLength(1000);
            entity.Property(e => e.Treatment).HasMaxLength(1000);
            entity.Property(e => e.VitalSigns).HasMaxLength(500);
            entity.Property(e => e.PhysicalExamination).HasMaxLength(1000);
            entity.Property(e => e.LabResults).HasMaxLength(1000);
            entity.Property(e => e.Recommendations).HasMaxLength(1000);
            entity.Property(e => e.FollowUpInstructions).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity
                .HasOne(e => e.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.Doctor)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.Appointment)
                .WithMany(a => a.MedicalRecords)
                .HasForeignKey(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Prescription entity
        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MedicationName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Dosage).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Frequency).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Instructions).HasMaxLength(500);
            entity.Property(e => e.SideEffects).HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity
                .HasOne(e => e.Patient)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.Doctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(e => e.MedicalRecord)
                .WithMany(mr => mr.Prescriptions)
                .HasForeignKey(e => e.MedicalRecordId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure DoctorSchedule entity
        modelBuilder.Entity<DoctorSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity
                .HasOne(e => e.Doctor)
                .WithMany(d => d.Schedules)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed default roles
        SeedRoles(modelBuilder);

        // Add automatic timestamp updates
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType
                .ClrType.GetProperties()
                .Where(p => p.Name == "CreatedAt" || p.Name == "UpdatedAt");

            foreach (var property in properties)
            {
                modelBuilder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasDefaultValueSql(
                        property.Name == "CreatedAt" ? "GETUTCDATE()" : "GETUTCDATE()"
                    );
            }
        }
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        var roles = ClinicRoles
            .AllRoles.Select(
                (role, index) =>
                    new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = role,
                        NormalizedName = role.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                    }
            )
            .ToArray();

        modelBuilder.Entity<IdentityRole>().HasData(roles);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e =>
                e.Entity is Patient
                || e.Entity is Doctor
                || e.Entity is Appointment
                || e.Entity is MedicalRecord
                || e.Entity is Prescription
                || e.Entity is DoctorSchedule
                || e.Entity is ApplicationUser
            );

        foreach (var entry in entries)
        {
            var now = DateTime.UtcNow;

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.CurrentValues["CreatedAt"] = now;
                    entry.CurrentValues["UpdatedAt"] = now;
                    break;
                case EntityState.Modified:
                    entry.CurrentValues["UpdatedAt"] = now;
                    break;
            }
        }
    }
}
