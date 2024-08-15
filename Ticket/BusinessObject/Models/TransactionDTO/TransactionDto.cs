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
}