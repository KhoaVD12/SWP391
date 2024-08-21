using System;
using System.Collections.Generic;
using DataAccessObject.Enums;

namespace DataAccessObject.Entities;

public partial class Attendee
{
    public int Id { get; set; }

    public DateTime RegistrationDate { get; set; }

    public CheckInStatus CheckInStatus { get; set; }

    public int TicketId { get; set; }

    public int EventId { get; set; }
    public string CheckInCode { get; set; } = null!;

    public string PaymentStatus { get; set; }

    public virtual ICollection<AttendeeDetail> AttendeeDetails { get; set; } = new List<AttendeeDetail>();

    public virtual Event Event { get; set; } = null!;

    public virtual ICollection<GiftReception> GiftReceptions { get; set; } = new List<GiftReception>();

    public virtual Ticket Ticket { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public bool IsPaymentExpired(TimeSpan expirationPeriod)
    {
        var latestTransaction = Transactions.OrderByDescending(t => t.Date).FirstOrDefault();
        if (latestTransaction == null || latestTransaction.Status != "PENDING")
        {
            return false;
        }
        return DateTime.UtcNow - latestTransaction.Date > expirationPeriod;
    }
}
