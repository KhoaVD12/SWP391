using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Ticket
{
    public int TicketId { get; set; }

    public decimal Price { get; set; }

    public int EventId { get; set; }

    public int Quantity { get; set; }

    public virtual ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();
}
