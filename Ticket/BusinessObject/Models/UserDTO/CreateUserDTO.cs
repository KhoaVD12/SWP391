using DataAccessObject.Enums;

namespace BusinessObject.Models.UserDTO;

public class CreateUserDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    /// <summary>
    /// The role of the user. Accepted values are 'Organizer', 'Staff', 'Sponsor'.
    /// </summary>
    public string Role { get; set; } = null!;
}