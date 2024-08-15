using BusinessObject.Models.PaymentDTO;
using BusinessObject.Responses;

namespace BusinessObject.IService;

public interface IPaymentService
{
    Task<ServiceResponse<PaginationModel<PaymentMethodDto>>> GetAllPaymentMethodsAsync(int page, int pageSize, string search);
    Task<ServiceResponse<PaymentMethodDto>> GetPaymentMethodByIdAsync(int id);
    Task<ServiceResponse<PaymentMethodDto>> CreatePaymentMethodAsync(CreatePaymentMethodDto dto);
    Task<ServiceResponse<PaymentMethodDto>> UpdatePaymentMethodAsync(int id, PaymentMethodDto dto);
    Task<ServiceResponse<bool>> DeletePaymentMethodAsync(int id);
}