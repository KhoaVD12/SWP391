using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models.EventDTO
{
    public class ViewEventDTO2nd
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Image { get; set; }
        public string Status { get; set; }
        public EventTicket Ticket { get; set; }
    }
    public class EventTicket
    {
        public int Id { get; set; }

        public decimal Price { get; set; }

        public int EventId { get; set; }

        public int Quantity { get; set; }

        public DateTime TicketSaleEndDate { get; set; }
    }
}
