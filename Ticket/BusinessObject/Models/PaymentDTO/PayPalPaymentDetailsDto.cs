namespace BusinessObject.Models.PaymentDTO;

public class PayPalPaymentDetailsDto
{
    public string PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public int PaymentMethodId { get; set; }
    public int AttendeeId { get; set; }
}