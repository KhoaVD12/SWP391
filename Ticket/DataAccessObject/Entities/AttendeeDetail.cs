using System;
using System.Collections.Generic;

namespace DataAccessObject.Entities;

public partial class AttendeeDetail
{
    public int Id { get; set; }

    public int AttendeeId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual Attendee Attendee { get; set; } = null!;
}
