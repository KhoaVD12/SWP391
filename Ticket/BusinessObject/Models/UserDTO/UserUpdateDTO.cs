using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models.UserDTO;

public class UserUpdateDTO
{
    [Required]
    public int Id { get; set; }

    public string Name { get; set; } 

    public string Email { get; set; } 
}