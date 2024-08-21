namespace BusinessObject.Models.VnPayDTO;

public class VnPaymentResponseModel
{
    public bool Success { get; set; }
    public string PaymentMethod { get; set; } // The method of payment (e.g., "VNPay")
    public string OrderDescription { get; set; } // Description of the order
    public string OrderId { get; set; } // The ID of the order
    public string PaymentId { get; set; } // The ID of the payment
    public string TransactionId { get; set; } // The ID of the transaction
    public string Token { get; set; } // The URL to redirect to VNPay for payment
    public string VnPayResponseCode { get; set; } // The response code from VNPay (e.g., "00" for success)
}

public class VnPaymentRequestModel
{
    public string FullName { get; set; }
    public string Description { get; set; } 
    public string Amount { get; set; } 


}