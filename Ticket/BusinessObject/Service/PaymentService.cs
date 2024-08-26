using System.Globalization;
using AutoMapper;
using BusinessObject.IService;
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
    private readonly IMapper _mapper;
    private readonly PaypalClient _paypalClient;

    public PaymentService(IPaymentRepo paymentRepo, IMapper mapper, ITransactionRepo transactionRepo,
        PaypalClient paypalClient)
    {
        _paymentRepo = paymentRepo;
        _mapper = mapper;
        _transactionRepo = transactionRepo;
        _paypalClient = paypalClient;
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

    public async Task<CreateOrderResponse> CreateOrderAsync(int attendeeId, decimal amount, string currency)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
        {
            throw new ArgumentException("Invalid currency code.");
        }

        decimal conversionRate = 1 / 24925m; // 1 USD = 24,925 VND

        // Check if currency is VND and convert to USD for PayPal
        if (currency.Equals("VND", StringComparison.CurrentCultureIgnoreCase))
        {
            // Convert the amount from VND to USD
            amount *= conversionRate;
            currency = "USD";
        }

        amount = Math.Round(amount, 2);

        var referenceId = Guid.NewGuid().ToString();

        try
        {
            var orderResponse =
                await _paypalClient.CreateOrder(amount.ToString(CultureInfo.InvariantCulture), currency, referenceId);

            if (string.IsNullOrEmpty(orderResponse.id))
            {
                throw new Exception("Failed to create PayPal order.");
            }

            // Save Payment and Transaction in the database
            var payment = new Payment
            {
                Name = "PayPal",
                Status = PaymentStatus.PENDING,
                PaymentDate = DateTime.Now
            };

            await _paymentRepo.AddAsync(payment);

            var transaction = new Transaction
            {
                AttendeeId = attendeeId,
                Date = DateTime.Now,
                Amount = amount,
                PaymentMethod = payment.Id,
                Status = TransactionStatus.PENDING
            };

            await _transactionRepo.AddAsync(transaction);

            return orderResponse;
        }
        catch (HttpRequestException httpEx)
        {
            throw new Exception("Failed to communicate with PayPal. Please try again later.", httpEx);
        }
        catch (Exception ex)
        {
            // Log the exception details
            throw new Exception("An error occurred while creating the order. Please try again later.", ex);
        }
    }

    /*public void SchedulePaymentCancellation(int attendeeId)
    {
        // Schedule the job to run immediately or at a specified time
        BackgroundJob.Enqueue(() => _attendeeJobs.HandlePaymentCancellationAsync(attendeeId));
    }*/
}