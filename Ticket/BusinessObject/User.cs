using System;
using System.Collections.Generic;
using DataAccessObject.Enums;

namespace BusinessObject;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<BoothRequest> BoothRequests { get; set; } = new List<BoothRequest>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();


    public UserRole UserRoleEnum
    {
        get => Helpers.Helpers.EnumHelper.ParseUserRole(Role);
        set => Role = Helpers.Helpers.EnumHelper.UserRoleToString(value);
    }

    public Status UserStatusEnum
    {
        get => Helpers.Helpers.EnumHelper.ParseStatus(Status);
        set => Status = Helpers.Helpers.EnumHelper.UserStatusToString(value);
    }
}
