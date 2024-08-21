using System.ComponentModel.DataAnnotations;
using DataAccessObject.Enums;

namespace BusinessObject.Models.AttendeeDto;

public class RegisterAttendeeDTO
{
    public DateTime RegistrationDate { get; set; }
    [Required]
    public int TicketId { get; set; }
    [Required]
    public int EventId { get; set; }
    public List<AttendeeDetailDto> AttendeeDetails { get; set; }
}

public class AttendeeDetailDto
{
    [Required]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }
    [Required]
    [Phone(ErrorMessage = "Invalid phone number.")]
    public string Phone { get; set; }

    public string CheckInCode { get; set; }
    public CheckInStatus CheckInStatus { get; set; }
}