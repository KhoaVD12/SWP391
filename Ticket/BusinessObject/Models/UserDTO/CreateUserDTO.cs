using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models.UserDTO;

public class CreateUserDto
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = null!;

    /// <summary>
    /// The role of the user. Accepted values are 'Organizer', 'Staff', 'Sponsor'.
    /// </summary>
    [Required(ErrorMessage = "Role is required.")]
    [RegularExpression("^(Organizer|Staff|Sponsor)$", ErrorMessage = "Role must be 'Organizer', 'Staff', or 'Sponsor'.")]
    public string Role { get; set; } = null!;
}