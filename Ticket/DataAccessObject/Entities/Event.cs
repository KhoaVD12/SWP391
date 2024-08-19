using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessObject.Entities;

public partial class Event
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;
    public string? ImageUrl { get; set; }

    public DateTime StartDate { get; set; }

    public int OrganizerId { get; set; }

    public int? StaffId { get; set; }

    public DateTime EndDate { get; set; }

    public string? Description { get; set; }

    public int VenueId { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();

    public virtual User Organizer { get; set; } = null!;
    [ForeignKey("StaffId")]
    public virtual User Staff { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual Venue Venue { get; set; } = null!;
}
