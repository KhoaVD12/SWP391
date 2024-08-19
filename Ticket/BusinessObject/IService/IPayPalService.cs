using BusinessObject.Models.PaymentDTO;
using BusinessObject.Models.PayPalDTO;
using DataAccessObject.Entities;

namespace BusinessObject.IService;

public interface IPayPalService
{
    Task<string> CreatePayment(decimal amount, string currency, string returnUrl, string cancelUrl);
    Task<bool> ExecutePayment(string paymentId, string payerId);
    Task<PayPalPaymentDetailsDto> GetPaymentDetails(string paymentId);
}