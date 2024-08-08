using System.Runtime.Serialization;

namespace DataAccessObject.Enums;

public enum UserRole   
{
    [EnumMember(Value = "Admin")]
    Admin,

    [EnumMember(Value = "Staff")]
    Staff,

    [EnumMember(Value = "Sponsor")]
    Sponsor,

    [EnumMember(Value = "Organizer")]
    Organizer,

}