using System;
using System.Collections.Generic;

namespace DataAccessObject;

public partial class AttendeeDetail
{
    public int DetailId { get; set; }

    public string AttendeeId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;
}
