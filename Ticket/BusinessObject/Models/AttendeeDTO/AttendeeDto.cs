namespace BusinessObject.Models.AttendeeDto;

public class AttendeeDto
{
    public int Id { get; set; }                  
    public DateTime RegistrationDate { get; set; } 
    public string CheckInStatus { get; set; }    
    public int TicketId { get; set; }            
    public int EventId { get; set; }           
    public string Name { get; set; }           
    public string Email { get; set; }            
    public string Phone { get; set; }      
}