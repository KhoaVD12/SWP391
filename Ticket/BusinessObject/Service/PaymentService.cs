using AutoMapper;
using BusinessObject.IService;
using BusinessObject.Mappers;
using BusinessObject.Models.PaymentDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.Enums;
using DataAccessObject.IRepo;

namespace BusinessObject.Service;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepo _paymentRepo;
    private readonly ITransactionRepo _transactionRepo;
    private readonly TicketContext _context;
    private readonly IMapper _mapper;
    private readonly IPayPalService _payPalService;

    public PaymentService(IPaymentRepo paymentRepo, IMapper mapper, ITransactionRepo transactionRepo, IPayPalService payPalService, TicketContext context)
    {
        _paymentRepo = paymentRepo;
        _mapper = mapper;
        _transactionRepo = transactionRepo;
        _payPalService = payPalService;
        _context = context;
    }

    public async Task<ServiceResponse<PaginationModel<PaymentMethodDto>>> GetAllPaymentMethodsAsync(int page,
        int pageSize,
        string search)
    {
        var response = new ServiceResponse<PaginationModel<PaymentMethodDto>>();

        try
        {
            var paymentMethods = await _paymentRepo.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                paymentMethods = paymentMethods
                    .Where(u => (u.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                 u.Name.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            var paymentDto = _mapper.Map<IEnumerable<PaymentMethodDto>>(paymentMethods);
            var paginationModel =
                await Pagination.GetPaginationEnum(paymentDto, page, pageSize);

            response.Data = paginationModel;
            response.Success = true;
            response.Message = "Payment methods retrieved successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "An error occurred while retrieving payment methods.";
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }

    public async Task<ServiceResponse<PaymentMethodDto>> GetPaymentMethodByIdAsync(int id)
    {
        var response = new ServiceResponse<PaymentMethodDto>();

        try
        {
            var paymentMethod = await _paymentRepo.GetByIdAsync(id);
            if (paymentMethod == null)
            {
                response.Success = false;
                response.Message = "Payment method not found.";
                return response;
            }

            response.Data = _mapper.Map<PaymentMethodDto>(paymentMethod);
            response.Success = true;
            response.Message = "Payment method retrieved successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "An error occurred while retrieving the payment method.";
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }

    public async Task<ServiceResponse<PaymentMethodDto>> CreatePaymentMethodAsync(CreatePaymentMethodDto dto)
    {
        var response = new ServiceResponse<PaymentMethodDto>();

        try
        {
            var paymentMethod = _mapper.Map<Payment>(dto);
            await _paymentRepo.AddAsync(paymentMethod);

            response.Data = _mapper.Map<PaymentMethodDto>(paymentMethod);
            response.Success = true;
            response.Message = "Payment method created successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "An error occurred while creating the payment method.";
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }

    public async Task<ServiceResponse<string>> CreatePayment(decimal amount, string currency, string returnUrl, string cancelUrl)
    {
        var response = new ServiceResponse<string>();

        try
        {
            var payment = await _payPalService.CreatePayment(amount, currency, returnUrl, cancelUrl);
            response.Data = payment;
            response.Success = true;
            response.Message = "Payment created successfully.";
        }
        catch (HttpRequestException ex)
        {
            // Log the full error response for troubleshooting
            response.Success = false;
            response.Message = $"Payment creation failed: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<bool>> ExecutePayment(string paymentId, string payerId)
    {
        var response = new ServiceResponse<bool>();

        try
        {
            var success = await _payPalService.ExecutePayment(paymentId, payerId);
            var paymentDetails = await _payPalService.GetPaymentDetails(paymentId);
            if (success)
            {
                var transaction = new Transaction
                {
                    AttendeeId = paymentDetails.AttendeeId, // This should be derived from your business logic
                    Date = DateTime.UtcNow,
                    Amount = paymentDetails.Amount, // The amount paid
                    PaymentMethod = paymentDetails.PaymentMethodId, // ID of the payment method used (e.g., PayPal)
                    Status = "Completed", // Set the status to reflect successful payment
                    PaymentMethodNavigation = new Payment
                    {
                        Name = "PayPal", // The name of the payment method
                        Status = "Success",
                        PaymentDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    }
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                response.Data = true;
                response.Success = true;
                response.Message = "Payment executed and transaction recorded successfully.";
            }
            else
            {
                response.Data = false;
                response.Success = false;
                response.Message = "Payment execution failed.";
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Payment execution failed: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<string>> InitiatePaymentAsync(PaymentRequestDto request)
    {
        var response = new ServiceResponse<string>();
        try
        {
            // Create a transaction record
            var transaction = _mapper.Map<Transaction>(request);
            transaction.Date = DateTime.UtcNow;
            transaction.Status = TransactionStatus.PENDING;

            await _transactionRepo.AddAsync(transaction);

            // Create a payment session with the payment gateway
            /*var paymentSession = await _paymentGateway.CreatePaymentSessionAsync(request);*/

            // Update response with the payment session URL
            /*
            response.Data = paymentSession.Url;
            */
            response.Success = true;
            response.Message = "Payment session created successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "An error occurred while initiating the payment.";
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }

    public async Task<ServiceResponse<bool>> HandlePaymentCallbackAsync(PaymentCallbackDto callback)
    {
        var transaction = await _transactionRepo.GetByIdAsync(callback.TransactionId);

        if (transaction == null)
        {
            return new ServiceResponse<bool> { Success = false, Message = "Transaction not found." };
        }

        // Map the payment status to transaction status
        transaction.Status = StatusMapper.MapPaymentStatusToTransactionStatus(callback.PaymentStatus);
        await _transactionRepo.UpdateAsync(transaction);

        return new ServiceResponse<bool> { Success = true, Message = "Payment status updated successfully." };
    }

    public async Task<ServiceResponse<PaymentMethodDto>> UpdatePaymentMethodAsync(int id, PaymentMethodDto dto)
    {
        var response = new ServiceResponse<PaymentMethodDto>();

        try
        {
            var paymentMethod = await _paymentRepo.GetByIdAsync(id);
            if (paymentMethod == null)
            {
                response.Success = false;
                response.Message = "Payment method not found.";
                return response;
            }

            _mapper.Map(dto, paymentMethod);
            await _paymentRepo.UpdateAsync(paymentMethod);

            response.Data = _mapper.Map<PaymentMethodDto>(paymentMethod);
            response.Success = true;
            response.Message = "Payment method updated successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "An error occurred while updating the payment method.";
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }

    public async Task<ServiceResponse<bool>> DeletePaymentMethodAsync(int id)
    {
        var response = new ServiceResponse<bool>();

        try
        {
            var paymentMethod = await _paymentRepo.GetByIdAsync(id);
            if (paymentMethod == null)
            {
                response.Success = false;
                response.Message = "Payment method not found.";
                return response;
            }

            await _paymentRepo.RemoveAsync(paymentMethod);

            response.Data = true;
            response.Success = true;
            response.Message = "Payment method deleted successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "An error occurred while deleting the payment method.";
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }
}