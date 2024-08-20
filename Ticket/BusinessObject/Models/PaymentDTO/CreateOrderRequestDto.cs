namespace BusinessObject.Models.PaymentDTO;

public class CreateOrderRequestDto
{
    public int AttendeeId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";
}

public class CaptureOrderRequestDto
{
    public string OrderId { get; set; }
    public int TransactionId { get; set; }
}