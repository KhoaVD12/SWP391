﻿using System;
using System.Collections.Generic;

namespace DataAccessObject;

public partial class GiftReception
{
    public int GiftReceptionId { get; set; }

    public string AttendeeId { get; set; } = null!;

    public int GiftId { get; set; }

    public DateTime ReceptionDate { get; set; }

    public virtual Gift Gift { get; set; } = null!;
}
