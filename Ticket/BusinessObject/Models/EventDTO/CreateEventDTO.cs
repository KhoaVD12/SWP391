using BusinessObject.Models.TicketDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models.EventDTO
{
    public class CreateEventDTO
    {
        public string? Title { get; set; }

        public DateTime StartDate { get; set; }

        public int OrganizerId { get; set; }

        public DateTime EndDate { get; set; }

        public string? Description { get; set; }

        public int VenueId { get; set; }

        
    }
}
