using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace BusinessObject.Models.EventDTO
{
    public class CreateEventDTO
    {
        [SwaggerSchema(Description = "Title of the event")]
        public string Title { get; set; }

        [SwaggerSchema(Description = "Start date of the event")]
        public DateTime StartDate { get; set; }

        [SwaggerSchema(Description = "End date of the event")]
        public DateTime EndDate { get; set; }

        [SwaggerSchema(Description = "ID of the event organizer")]
        public int OrganizerId { get; set; }

        [SwaggerSchema(Description = "ID of the staff assigned to the event")]
        public int? StaffId { get; set; }

        [SwaggerSchema(Description = "Description of the event")]
        public string? Description { get; set; }

        [SwaggerSchema(Description = "ID of the venue where the event will be held")]
        public int VenueId { get; set; }

        [SwaggerSchema(Description = "URL of the image for the event banner")]
        public string? ImageUrl { get; set; }

        [SwaggerSchema(Description = "Ticket Price for this Event")]
        public decimal Price { get; set; }
        [SwaggerSchema(Description = "Ticket Quantity for this Event")]
        public int Quantity { get; set; }

        [SwaggerSchema(Description = "Name of the presenter for the event")]
        public string? Presenter { get; set; }

        [SwaggerSchema(Description = "Name of the host for the event")]
        public string? Host { get; set; }
    }

}