﻿using System;
using System.Collections.Generic;

namespace DataAccessObject.Entities;

public partial class Attendee
{
    public int Id { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string CheckInStatus { get; set; } = null!;

    public int TicketId { get; set; }

    public int EventId { get; set; }

    public virtual ICollection<AttendeeDetail> AttendeeDetails { get; set; } = new List<AttendeeDetail>();

    public virtual Event Event { get; set; } = null!;

    public virtual ICollection<GiftReception> GiftReceptions { get; set; } = new List<GiftReception>();

    public virtual Ticket Ticket { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
