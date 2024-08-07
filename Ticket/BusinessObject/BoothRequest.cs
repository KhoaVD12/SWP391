using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class BoothRequest
{
    public int BoothRequestId { get; set; }

    public int SponsorId { get; set; }

    public int BoothId { get; set; }

    public DateTime RequestDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Booth Booth { get; set; } = null!;

    public virtual User Sponsor { get; set; } = null!;
}
