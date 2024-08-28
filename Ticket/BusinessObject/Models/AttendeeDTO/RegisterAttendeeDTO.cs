using System.ComponentModel.DataAnnotations;
using DataAccessObject.Enums;

namespace BusinessObject.Models.AttendeeDto;

public class RegisterAttendeeDTO
{
    public DateTime RegistrationDate { get; set; }
    [Required] public int TicketId { get; set; }
    [Required] public int EventId { get; set; }
    public List<AttendeeDetailRegisterDto> AttendeeDetails { get; set; }
}

public class AttendeeDetailRegisterDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required]
    public string Phone { get; set; }
}

public class AttendeeDetailDto
{
   
    public string Name { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string CheckInCode { get; set; }
    public CheckInStatus CheckInStatus { get; set; }
}