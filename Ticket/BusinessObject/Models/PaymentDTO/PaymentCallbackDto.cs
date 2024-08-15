using DataAccessObject.Enums;

namespace BusinessObject.Models.PaymentDTO;

public class PaymentCallbackDto
{
    public int TransactionId { get; set; }
    public string PaymentStatus { get; set; }
}