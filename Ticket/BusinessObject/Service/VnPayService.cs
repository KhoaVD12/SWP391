using System.Globalization;
using BusinessObject.IService;
using BusinessObject.Models.VnPayDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.Enums;
using DataAccessObject.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BusinessObject.Service;

public class VnPayService : IVnPayService
{
    private readonly IConfiguration _configuration;
    private readonly ITransactionRepo _transactionRepo;
    private readonly IPaymentRepo _paymentRepo;

    public VnPayService(IConfiguration configuration,
        ITransactionRepo transactionRepo,
        IPaymentRepo paymentRepo)
    {
        _configuration = configuration;
        _transactionRepo = transactionRepo;
        _paymentRepo = paymentRepo;
    }

    public async Task<ServiceResponse<VnPaymentResponseModel>> CreatePaymentRequest(int attendeeId, decimal amount,
        HttpContext httpContext)
    {
        var response = new ServiceResponse<VnPaymentResponseModel>();

        try
        {
            var vnpay = new VnPayLibrary();
            var vnp_TmnCode = _configuration["VNPay:TmnCode"];
            var vnp_HashSecret = _configuration["VNPay:HashSecret"];
            var vnp_BaseUrl = _configuration["VNPay:BaseUrl"];
            var returnUrl = _configuration["VNPay:ReturnUrl"];
            var tick = DateTime.Now.Ticks.ToString();

            // Prepare transaction
            var payment = new Payment
            {
                Name = "VNPay",
                Status = PaymentStatus.PENDING,
                PaymentDate = DateTime.UtcNow
            };
            await _paymentRepo.AddAsync(payment);

            // Create Transaction Entry
            var transaction = new Transaction
            {
                AttendeeId = attendeeId,
                Date = DateTime.UtcNow,
                Amount = amount,
                PaymentMethod = payment.Id, // Reference to the Payment entry
                Status = TransactionStatus.PENDING
            };
            await _transactionRepo.AddAsync(transaction);

            // Add request data
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((int)(amount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", VnPayUtils.GetIpAddress(httpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Payment for transaction {transaction.Id}");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnpay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
                vnpay.CreateRequestUrl(_configuration["VNPay:BaseUrl"], _configuration["VNPay:HashSecret"]);

            // Prepare response
            response.Data = new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VNPay",
                OrderId = transaction.Id.ToString(),
                PaymentId = transaction.Id.ToString(),
                TransactionId = transaction.Id.ToString(),
                Token = paymentUrl
            };
            response.Success = true;
            response.Message = "VNPay payment request created successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error occurred while creating VNPay payment request.";
            response.ErrorMessages.Add(ex.Message);
        }

        return response;
    }

    public async Task<ServiceResponse<Payment>> ProcessPaymentResponse(IQueryCollection queryParams)
    {
        var response = new ServiceResponse<Payment>();

        try
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in queryParams)
            {
                vnpay.AddResponseData(key, value);
            }

            var txnRef = vnpay.GetResponseData("vnp_TxnRef");
            var vnp_Amount = vnpay.GetResponseData("vnp_Amount");
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_SecureHash = vnpay.GetResponseData("vnp_SecureHash");

            if (string.IsNullOrEmpty(txnRef) || string.IsNullOrEmpty(vnp_Amount) ||
                string.IsNullOrEmpty(vnp_ResponseCode) || string.IsNullOrEmpty(vnp_SecureHash))
            {
                return new ServiceResponse<Payment>
                {
                    Success = false,
                    Message = "Missing or invalid parameters in the payment response.",
                    ErrorMessages = new List<string> { "One or more required parameters are missing or invalid." }
                };
            }

            if (!int.TryParse(txnRef, out var txnRefId) || !decimal.TryParse(vnp_Amount, NumberStyles.Number,
                    CultureInfo.InvariantCulture, out var vnpAmount))
            {
                return new ServiceResponse<Payment>
                {
                    Success = false,
                    Message = "Invalid format for transaction reference or amount.",
                    ErrorMessages = new List<string> { "Transaction reference or amount is not in the correct format." }
                };
            }

            var transaction = await _transactionRepo.GetByIdAsync(txnRefId);
            if (transaction == null)
            {
                return new ServiceResponse<Payment>
                {
                    Success = false,
                    Message = "Transaction not found.",
                    ErrorMessages = new List<string> { "Transaction with the given reference ID does not exist." }
                };
            }

            var vnp_HashSecret = _configuration["VNPay:HashSecret"];
            var isValidSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
            if (!isValidSignature)
            {
                transaction.Status = TransactionStatus.FAILED;
                await _transactionRepo.UpdateAsync(transaction);

                return new ServiceResponse<Payment>
                {
                    Success = false,
                    Message = "Invalid signature.",
                    ErrorMessages = new List<string> { "The signature provided by VNPay is invalid." }
                };
            }

            if (vnp_ResponseCode == "00")
            {
                transaction.Status = TransactionStatus.COMPLETED;
                var payment = new Payment
                {
                    Name = "VNPay",
                    Status = PaymentStatus.SUCCESSFUL,
                    PaymentDate = DateTime.UtcNow
                };
                await _paymentRepo.AddAsync(payment);

                return new ServiceResponse<Payment>
                {
                    Success = true,
                    Data = payment,
                    Message = "Payment successful."
                };
            }
            else
            {
                transaction.Status = TransactionStatus.FAILED;
                await _transactionRepo.UpdateAsync(transaction);

                return new ServiceResponse<Payment>
                {
                    Success = false,
                    Message = "Payment failed.",
                    ErrorMessages = new List<string> { $"Payment failed with response code: {vnp_ResponseCode}" }
                };
            }
        }
        catch (Exception ex)
        {
            return new ServiceResponse<Payment>
            {
                Success = false,
                Message = "Error occurred while processing VNPay payment response.",
                ErrorMessages = new List<string> { ex.Message }
            };
        }
    }
}