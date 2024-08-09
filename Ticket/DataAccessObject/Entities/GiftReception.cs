using System;
using System.Collections.Generic;

namespace DataAccessObject.Entities;

public partial class GiftReception
{
    public int Id { get; set; }

    public int AttendeeId { get; set; }

    public int GiftId { get; set; }

    public DateTime ReceptionDate { get; set; }

    public virtual Attendee Attendee { get; set; } = null!;

    public virtual Gift Gift { get; set; } = null!;
}
