using System;
using System.Collections.Generic;

namespace DataAccessObject;

public partial class Venue
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
