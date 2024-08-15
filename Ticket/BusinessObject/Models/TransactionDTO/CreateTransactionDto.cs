namespace BusinessObject.Models.TransactionDTO;

public class CreateTransactionDto
{
    public int AttendeeId { get; set; }
    public decimal Amount { get; set; }
    public int PaymentMethodId { get; set; }
    public string Status { get; set; }
}