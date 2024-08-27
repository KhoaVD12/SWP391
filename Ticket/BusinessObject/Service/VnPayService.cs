using BusinessObject.IService;
using BusinessObject.Models.VnPayDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.Enums;
using DataAccessObject.IRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace BusinessObject.Service;

public class VnPayService : IVnPayService
{
    private readonly IConfiguration _configuration;
    private readonly ITransactionRepo _transactionRepo;
    private readonly IPaymentRepo _paymentRepo;
    private readonly IAttendeeRepo _attendeeRepo;
    private readonly ITicketRepo _ticketRepo;
    private readonly IMemoryCache _memoryCache;

    public VnPayService(IConfiguration configuration,
        ITransactionRepo transactionRepo,
        IPaymentRepo paymentRepo, IAttendeeRepo attendeeRepo, IMemoryCache memoryCache, ITicketRepo ticketRepo)
    {
        _configuration = configuration;
        _transactionRepo = transactionRepo;
        _paymentRepo = paymentRepo;
        _attendeeRepo = attendeeRepo;
        _memoryCache = memoryCache;
        _ticketRepo = ticketRepo;
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

                var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var nowUtc = DateTime.UtcNow;
                var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, localTimeZone);

                // Prepare transaction
                var payment = new Payment
                {
                    Name = "VNPay",
                    Status = PaymentStatus.PENDING,
                    PaymentDate = localDateTime
                };
                await _paymentRepo.AddAsync(payment);

                // Create Transaction Entry
                var transaction = new Transaction
                {
                    AttendeeId = attendeeId,
                    Date = localDateTime,
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
                vnpay.AddRequestData("vnp_TxnRef", transaction.Id.ToString());

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

            // Collect and validate the response data
            foreach (var (key, value) in queryParams)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_SecureHash = queryParams["vnp_SecureHash"].ToString();
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_HashSecret = _configuration["VNPay:HashSecret"];

            // Retrieve the transaction from the database
            var transaction = await _transactionRepo.GetByIdWithPaymentAsync((int)vnp_orderId);
            if (transaction == null)
            {
                response.Success = false;
                response.Message = "Transaction not found.";
                response.ErrorMessages.Add("Transaction with the given reference ID does not exist.");
                return response;
            }

            // Validate the secure hash
            var isValidSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
            if (!isValidSignature)
            {
                transaction.Status = TransactionStatus.FAILED;
                await _transactionRepo.UpdateAsync(transaction);

                response.Success = false;
                response.Message = "Invalid signature.";
                response.ErrorMessages.Add("The signature provided by VNPay is invalid.");
                return response;
            }

            // Determine the transaction status based on VNPay's response code
            if (vnp_ResponseCode == "00")
            {
                transaction.Status = TransactionStatus.COMPLETED;
            }
            else if (vnp_ResponseCode == "24") // Assuming "24" is the code for cancellation
            {
                transaction.Status = TransactionStatus.CANCELLED;
                response.Success = false;
                response.Message = "Payment was canceled by the user.";
                return response;
            }
            else
            {
                transaction.Status = TransactionStatus.FAILED;
            }

            // Update the transaction status in the database
            await _transactionRepo.UpdateAsync(transaction);

// Convert to SE Asia Standard Time
            var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            // Create a new payment record based on the transaction status
            var payment = transaction.PaymentMethodNavigation;
            payment.Status = transaction.Status == TransactionStatus.COMPLETED
                ? PaymentStatus.SUCCESSFUL
                : PaymentStatus.FAILED;
            payment.PaymentDate = localNow;

            await _paymentRepo.UpdateAsync(payment);

            if (transaction.Status == TransactionStatus.COMPLETED)
            {
                var ticket = await _ticketRepo.GetByIdAsync(transaction.Attendee.TicketId);
                if (ticket != null && ticket.Quantity > 0)
                {
                    ticket.Quantity -= 1;
                    await _ticketRepo.UpdateAsync(ticket);
                }

                // Retrieve the attendee
                var attendee = await _attendeeRepo.GetAttendeeByIdAsync(transaction.AttendeeId);
                if (attendee != null)
                {
                    attendee.PaymentStatus = PaymentStatus.SUCCESSFUL;

                    // Generate check-in code and update attendee
                    var checkInCode = GenerateCheckInCode();
                    attendee.CheckInCode = checkInCode;
                    attendee.CheckInStatus = CheckInStatus.NotCheckedIn;

                    await _attendeeRepo.UpdateAsync(attendee);

                    var eventTitle = ticket.Event.Title;
                    var eventStartDate = ticket.Event.StartDate;
                    var amountPaid = transaction.Amount;

                    // Send confirmation email with the check-in code
                    foreach (var attendeeDetail in attendee.AttendeeDetails)
                    {
                        await SendEmail.SendRegistrationEmail(
                            attendeeDetail.Email,
                            attendeeDetail.Name,
                            eventTitle,
                            eventStartDate,
                            amountPaid,
                            checkInCode
                        );
                    }
                }
            }

            // Prepare the response
            response.Success = true;
            response.Data = payment;
            response.Message = transaction.Status == TransactionStatus.COMPLETED
                ? "Payment processed and email sent successfully."
                : "Payment failed.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error occurred while processing VNPay payment response.";
            response.ErrorMessages.Add(ex.Message);
        }

        return response;
    }

    private string? GenerateCheckInCode()
    {
        return Guid.NewGuid().ToString()[..8].ToUpper();
    }
}