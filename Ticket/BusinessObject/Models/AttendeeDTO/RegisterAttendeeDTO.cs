namespace BusinessObject.Models.AttendeeDto;

public class RegisterAttendeeDTO
{
    public DateTime RegistrationDate { get; set; }
    public int TicketId { get; set; }
    public int EventId { get; set; }
    public List<AttendeeDetailDto> AttendeeDetails { get; set; }
}

public class AttendeeDetailDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}