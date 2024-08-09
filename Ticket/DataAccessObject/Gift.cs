using System;
using System.Collections.Generic;

namespace DataAccessObject;

public partial class Gift
{
    public int GiftId { get; set; }

    public string? Description { get; set; }

    public int BoothId { get; set; }

    public string Name { get; set; } = null!;

    public int Quantity { get; set; }

    public virtual Booth Booth { get; set; } = null!;

    public virtual ICollection<GiftReception> GiftReceptions { get; set; } = new List<GiftReception>();
}
