using Microsoft.AspNetCore.Http;

namespace BusinessObject.Models.EventDTO
{
    public class CreateEventDTO
    {
        public string? Title { get; set; }

        public DateTime StartDate { get; set; }

        public int OrganizerId { get; set; }

        public  DateTime EndDate { get; set; }

        public string? Description { get; set; }

        public int VenueId { get; set; }
        public IFormFile ImageUrl { get; set; }

        
    }
}
