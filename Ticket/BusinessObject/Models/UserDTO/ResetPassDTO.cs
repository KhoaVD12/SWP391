using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models.UserDTO;

public class ResetPassDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [StringLength(15, ErrorMessage = "{0} to {2} from {1} character.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }       
    public string ConfirmPassword { get; set; }
}