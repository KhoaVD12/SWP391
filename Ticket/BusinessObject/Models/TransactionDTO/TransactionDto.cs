using BusinessObject.Models.PaymentDTO;

namespace BusinessObject.Models.TransactionDTO;

public class TransactionDto
{
    public int Id { get; set; }
    public int AttendeeId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethodDto PaymentMethod { get; set; }
    public string Status { get; set; }

    public AttendeeTransactionDto Attendee { get; set; } // Add Attendee information
}

public class AttendeeTransactionDto
{
    public int AttendeeId { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string PaymentStatus { get; set; }
    public string? CheckInCode { get; set; }

    public List<AttendeeDetailTransactionDto> AttendeeDetails { get; set; } = []; // Add AttendeeDetails
}

public class AttendeeDetailTransactionDto
{
    public int AttendeeDetailId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}