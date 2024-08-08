using DataAccessObject.Enums;

namespace BusinessObject.Helpers;

public class Helpers
{
    public static class EnumHelper
    {
        public static UserRole ParseUserRole(string role)
        {
            return Enum.TryParse(role, out UserRole parsedRole) ? parsedRole : UserRole.Staff;
        }

        public static Status ParseStatus(string status)
        {
            return Enum.TryParse(status, out Status parsedStatus) ? parsedStatus : Status.Active;
        }

        public static string UserRoleToString(UserRole role)
        {
            return role.ToString();
        }

        public static string UserStatusToString(Status status)
        {
            return status.ToString();
        }
    }
}