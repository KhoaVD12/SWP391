using System;
using System.Collections.Generic;

namespace DataAccessObject;

public partial class Attendee
{
    public string AttendeeId { get; set; } = null!;

    public DateTime RegistrationDate { get; set; }

    public string CheckInStatus { get; set; } = null!;

    public int TicketId { get; set; }

    public int EventId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
