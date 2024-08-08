using System.Runtime.Serialization;
using DataAccessObject.Enums;

namespace DataAccessObject.Helpers;

public static class EnumsHelper
{
    public static Role ParseUserRole(string role)
    {
        return Enum.TryParse(role, out Role parsedRole) ? parsedRole : Role.Staff;
    }

    // Converts a string to UserStatus enum
    public static Status ParseUserStatus(string status)
    {
        return Enum.TryParse(status, out Status parsedStatus) ? parsedStatus : Status.Active;
    }

    // Converts a UserRole enum to its string representation
    public static string UserRoleToString(Role role)
    {
        return role.ToString();
    }

    // Converts a UserStatus enum to its string representation
    public static string UserStatusToString(Status status)
    {
        return status.ToString();
    }
}