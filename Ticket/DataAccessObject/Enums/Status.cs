using System.Runtime.Serialization;

namespace DataAccessObject.Enums;

public enum Status
{
    [EnumMember(Value = "Active")]
    Active,

    [EnumMember(Value = "Inactive")]
    Inactive,

    [EnumMember(Value = "Pending")]
    Pending
}