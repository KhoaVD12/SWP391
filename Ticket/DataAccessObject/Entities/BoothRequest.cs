using System;
using System.Collections.Generic;

namespace DataAccessObject.Entities;

public partial class BoothRequest
{
    public int Id { get; set; }

    public int SponsorId { get; set; }

    public int BoothId { get; set; }

    public DateTime RequestDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Booth Booth { get; set; } = null!;

    public virtual User Sponsor { get; set; } = null!;
}
