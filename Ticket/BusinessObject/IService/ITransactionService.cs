using BusinessObject.Models.TransactionDTO;
using BusinessObject.Responses;

namespace BusinessObject.IService;

public interface ITransactionService
{
    Task<ServiceResponse<IEnumerable<TransactionDto>>> GetTransactionsByAttendeeAsync(int attendeeId);
    Task<ServiceResponse<TransactionDto>> GetTransactionByIdAsync(int id);
    Task<ServiceResponse<TransactionDto>> CreateTransactionAsync(CreateTransactionDto dto);
    Task<ServiceResponse<TransactionDto>> UpdateTransactionAsync(int id, TransactionDto dto);
    Task<ServiceResponse<bool>> DeleteTransactionAsync(int id);

    Task<ServiceResponse<IEnumerable<TransactionDto>>> GetTransactionsByDateRangeAsync(DateTime startDate,
        DateTime endDate);

    Task<ServiceResponse<decimal>> GetTotalAmountByAttendeeAsync(int attendeeId);
}