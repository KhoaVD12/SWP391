namespace BusinessObject.Models.EventDTO
{
    public class ViewEventDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int OrganizerId { get; set; }
        public string OrganizerName { get; set; }
        public int VenueId { get; set; }
        public string VenueName { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ImageURL { get; set; } 
        public string Status { get; set; }


    }
}
