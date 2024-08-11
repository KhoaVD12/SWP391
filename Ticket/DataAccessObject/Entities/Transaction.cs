using System;
using System.Collections.Generic;

namespace DataAccessObject.Entities;

public partial class Transaction
{
    public int Id { get; set; }

    public int AttendeeId { get; set; }

    public DateTime Date { get; set; }

    public decimal Amount { get; set; }

    public int PaymentMethod { get; set; }

    public string Status { get; set; } = null!;

    public virtual Attendee Attendee { get; set; } = null!;

    public virtual Payment PaymentMethodNavigation { get; set; } = null!;
}
