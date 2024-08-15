using BusinessObject.Models.TicketDTO;
using Microsoft.AspNetCore.Http;

namespace BusinessObject.Models.EventDTO;

public class CreateEventWithTicketsDTO
{
    public string Title { get; set; } = null!;
    public IFormFile ImageUrl { get; set; } // Image file
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int OrganizerId { get; set; }
    public int VenueId { get; set; }
    public string Description { get; set; } = null!;
    public TicketDTO Ticket { get; set; } = new TicketDTO(); 
}

public class TicketDTO
{
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime TicketSaleEndDate { get; set; }
}