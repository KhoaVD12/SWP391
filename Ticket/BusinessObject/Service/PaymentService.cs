using AutoMapper;
using BusinessObject.IService;
using BusinessObject.Models.PaymentDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;

namespace BusinessObject.Service;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepo _paymentRepo;
    private readonly IMapper _mapper;

    public PaymentService(IPaymentRepo paymentRepo, IMapper mapper)
    {
        _paymentRepo = paymentRepo;
        _mapper = mapper;
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
}