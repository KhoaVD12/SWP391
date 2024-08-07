using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public string AttendeeId { get; set; } = null!;

    public DateTime Date { get; set; }

    public decimal Amount { get; set; }

    public int PaymentMethod { get; set; }

    public string Status { get; set; } = null!;

    public virtual Attendee Attendee { get; set; } = null!;

    public virtual Payment PaymentMethodNavigation { get; set; } = null!;
}
