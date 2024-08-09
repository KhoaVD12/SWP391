using System;
using System.Collections.Generic;

namespace DataAccessObject.Entities;

public partial class Ticket
{
    public int Id { get; set; }

    public decimal Price { get; set; }

    public int EventId { get; set; }

    public int Quantity { get; set; }

    public DateTime TicketSaleEndDate { get; set; }

    public virtual ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();

    public virtual Event Event { get; set; } = null!;
}
