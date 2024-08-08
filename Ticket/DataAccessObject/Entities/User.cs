using DataAccessObject.Enums;
using DataAccessObject.Helpers;

namespace DataAccessObject.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string  Role { get; set; } = null!;

    public string  Status { get; set; } = null!;

    public virtual ICollection<BoothRequest> BoothRequests { get; set; } = new List<BoothRequest>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public Role UserRoleEnum
    {
        get => EnumsHelper.ParseUserRole(Role);
        set => Role = EnumsHelper.UserRoleToString(value);
    }

    public Status UserStatusEnum
    {
        get => EnumsHelper.ParseUserStatus(Status);
        set => Status = EnumsHelper.UserStatusToString(value);
    }
   
}
