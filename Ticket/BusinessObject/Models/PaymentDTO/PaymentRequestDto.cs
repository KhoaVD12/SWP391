namespace BusinessObject.Models.PaymentDTO;

public class PaymentRequestDto
{
    public int AttendeeId { get; set; }
    public decimal Amount { get; set; }
    public int PaymentMethodId { get; set; }
    public string Currency { get; set; }
}