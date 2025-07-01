namespace ClinicApi.Business.Domain.Constants;

public static class ClinicRoles
{
    public const string Admin = "Admin";
    public const string Doctor = "Doctor";
    public const string Nurse = "Nurse";
    public const string Receptionist = "Receptionist";
    public const string Patient = "Patient";
    public const string Manager = "Manager";

    public static readonly string[] AllRoles =
    {
        Admin,
        Doctor,
        Nurse,
        Receptionist,
        Patient,
        Manager,
    };

    public static readonly Dictionary<string, string> RoleDescriptions = new()
    {
        { Admin, "System administrator with full access" },
        { Doctor, "Medical doctor with access to patient records and appointments" },
        { Nurse, "Nursing staff with limited patient access" },
        { Receptionist, "Front desk staff for appointment management" },
        { Patient, "Patient with access to own records" },
        { Manager, "Clinic manager with administrative privileges" },
    };
}
