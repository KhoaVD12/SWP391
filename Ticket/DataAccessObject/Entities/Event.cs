using System;
using System.Collections.Generic;

namespace DataAccessObject.Entities;

public partial class Event
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public int OrganizerId { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Description { get; set; }

    public int VenueId { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();

    public virtual User Organizer { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual Venue Venue { get; set; } = null!;
}
