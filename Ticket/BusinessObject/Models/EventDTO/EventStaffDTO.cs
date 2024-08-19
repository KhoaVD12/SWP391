namespace BusinessObject.Models.EventDTO;

public class EventStaffDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<EventDTO> AssignedEvents { get; set; } = new List<EventDTO>();
}

public class EventDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string VenueName { get; set; } 
    public string Status { get; set; } = null!;
    public string? Description { get; set; }
    
}