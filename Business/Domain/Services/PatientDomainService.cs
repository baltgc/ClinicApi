using ClinicApi.Business.Domain.Models;

namespace ClinicApi.Business.Domain.Services;

/// <summary>
/// Domain service implementing complex patient business rules
/// </summary>
public class PatientDomainService : IPatientDomainService
{
    /// <summary>
    /// Business rule: Patient eligibility based on age, medical history, and previous appointments
    /// </summary>
    public bool IsEligibleForAppointmentType(Patient patient, string appointmentType)
    {
        var age = CalculateAge(patient.DateOfBirth);

        return appointmentType.ToLower() switch
        {
            "pediatric" => age < 18,
            "geriatric" => age >= 65,
            "surgery" => age >= 18 && age <= 75 && !HasHighRiskConditions(patient),
            "emergency" => true, // Always eligible for emergency
            "routine" => patient.IsActive,
            "follow-up" => patient.IsActive && HasPreviousAppointments(patient),
            "specialist" => patient.IsActive && HasReferral(patient),
            _ => patient.IsActive,
        };
    }

    /// <summary>
    /// Business rule: Risk assessment based on age, medical history, and known conditions
    /// </summary>
    public string CalculateRiskLevel(Patient patient)
    {
        var riskScore = 0;
        var age = CalculateAge(patient.DateOfBirth);

        // Age-based risk
        riskScore += age switch
        {
            >= 75 => 3,
            >= 65 => 2,
            >= 50 => 1,
            _ => 0,
        };

        // Medical history risk factors
        var medicalHistory = patient.MedicalHistory?.ToLower() ?? "";
        var highRiskConditions = new[]
        {
            "diabetes",
            "hypertension",
            "heart disease",
            "cancer",
            "stroke",
        };
        riskScore += highRiskConditions.Count(condition => medicalHistory.Contains(condition));

        // Allergy risk
        if (!string.IsNullOrEmpty(patient.Allergies))
        {
            var allergyCount = patient.Allergies.Split(',').Length;
            riskScore +=
                allergyCount > 3 ? 2
                : allergyCount > 1 ? 1
                : 0;
        }

        return riskScore switch
        {
            <= 1 => "Low",
            <= 3 => "Medium",
            <= 5 => "High",
            _ => "Critical",
        };
    }

    /// <summary>
    /// Business rule: Special accommodations based on age, disabilities, and medical conditions
    /// </summary>
    public List<string> GetRequiredAccommodations(Patient patient)
    {
        var accommodations = new List<string>();
        var age = CalculateAge(patient.DateOfBirth);
        var medicalHistory = patient.MedicalHistory?.ToLower() ?? "";

        // Age-based accommodations
        if (age >= 75)
        {
            accommodations.Add("Extra time for consultation");
            accommodations.Add("Ground floor room preferred");
            accommodations.Add("Large print materials");
        }

        if (age < 5)
        {
            accommodations.Add("Pediatric equipment required");
            accommodations.Add("Parent/guardian presence required");
        }

        // Medical condition accommodations
        if (medicalHistory.Contains("wheelchair") || medicalHistory.Contains("mobility"))
            accommodations.Add("Wheelchair accessible room");

        if (medicalHistory.Contains("hearing") || medicalHistory.Contains("deaf"))
            accommodations.Add("Sign language interpreter");

        if (medicalHistory.Contains("anxiety") || medicalHistory.Contains("panic"))
            accommodations.Add("Quiet environment preferred");

        if (!string.IsNullOrEmpty(patient.Allergies))
            accommodations.Add("Allergy protocol required");

        return accommodations;
    }

    /// <summary>
    /// Business rule: Multiple appointments same day based on patient condition and appointment types
    /// </summary>
    public bool CanHaveMultipleAppointmentsSameDay(
        Patient patient,
        List<Appointment> existingAppointments
    )
    {
        var age = CalculateAge(patient.DateOfBirth);
        var riskLevel = CalculateRiskLevel(patient);

        // High-risk patients limited to 2 appointments per day
        if (riskLevel == "High" || riskLevel == "Critical")
            return existingAppointments.Count < 2;

        // Elderly patients (75+) limited to 2 appointments per day
        if (age >= 75)
            return existingAppointments.Count < 2;

        // Children under 12 limited to 1 appointment per day
        if (age < 12)
            return existingAppointments.Count < 1;

        // Regular patients can have up to 3 appointments per day
        return existingAppointments.Count < 3;
    }

    /// <summary>
    /// Business rule: Follow-up intervals based on diagnosis severity and patient risk
    /// </summary>
    public TimeSpan GetRecommendedFollowUpInterval(Patient patient, string diagnosisType)
    {
        var riskLevel = CalculateRiskLevel(patient);
        var age = CalculateAge(patient.DateOfBirth);

        var baseInterval = diagnosisType.ToLower() switch
        {
            "acute" => TimeSpan.FromDays(3),
            "chronic" => TimeSpan.FromDays(30),
            "post-surgery" => TimeSpan.FromDays(7),
            "emergency" => TimeSpan.FromDays(1),
            "routine" => TimeSpan.FromDays(90),
            "preventive" => TimeSpan.FromDays(365),
            _ => TimeSpan.FromDays(30),
        };

        // Adjust based on risk level
        var riskMultiplier = riskLevel switch
        {
            "Critical" => 0.5, // More frequent follow-ups
            "High" => 0.7,
            "Medium" => 1.0,
            "Low" => 1.5, // Less frequent follow-ups
            _ => 1.0,
        };

        // Adjust for age (elderly need more frequent follow-ups)
        var ageMultiplier =
            age >= 75 ? 0.8
            : age >= 65 ? 0.9
            : 1.0;

        var adjustedDays = (int)(baseInterval.TotalDays * riskMultiplier * ageMultiplier);
        return TimeSpan.FromDays(Math.Max(1, adjustedDays)); // Minimum 1 day
    }

    #region Private Helper Methods

    private int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age))
            age--;
        return age;
    }

    private bool HasHighRiskConditions(Patient patient)
    {
        var medicalHistory = patient.MedicalHistory?.ToLower() ?? "";
        var highRiskConditions = new[]
        {
            "heart disease",
            "diabetes",
            "cancer",
            "kidney disease",
            "liver disease",
        };
        return highRiskConditions.Any(condition => medicalHistory.Contains(condition));
    }

    private bool HasPreviousAppointments(Patient patient)
    {
        // This would typically check the patient's appointment history
        // For now, we'll assume patients with medical history have had previous appointments
        return !string.IsNullOrEmpty(patient.MedicalHistory);
    }

    private bool HasReferral(Patient patient)
    {
        // This would typically check for referral documents in the system
        // For now, we'll check if patient has complex medical history requiring specialist care
        var medicalHistory = patient.MedicalHistory?.ToLower() ?? "";
        var specialistConditions = new[] { "cardiology", "neurology", "oncology", "orthopedic" };
        return specialistConditions.Any(condition => medicalHistory.Contains(condition));
    }

    #endregion
}
