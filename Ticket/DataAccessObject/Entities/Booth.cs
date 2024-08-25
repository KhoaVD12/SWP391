using System;
using System.Collections.Generic;

namespace DataAccessObject.Entities;

public partial class Booth
{
    public int Id { get; set; }

    public int SponsorId { get; set; }

    public int EventId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public string Location { get; set; } = null!;

    public virtual ICollection<BoothRequest> BoothRequests { get; set; } = new List<BoothRequest>();

    public virtual ICollection<Gift> Gifts { get; set; } = new List<Gift>();

    public virtual Event Event { get; set; } = null!;
}
