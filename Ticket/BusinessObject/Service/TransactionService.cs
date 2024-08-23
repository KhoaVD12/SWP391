using AutoMapper;
using BusinessObject.IService;
using BusinessObject.Models.TransactionDTO;
using BusinessObject.Responses;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;

namespace BusinessObject.Service;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepo _transactionRepo;
    private readonly IMapper _mapper;

    public TransactionService(ITransactionRepo transactionRepo, IMapper mapper)
    {
        _transactionRepo = transactionRepo;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<IEnumerable<TransactionDto>>> GetTransactionsByAttendeeAsync(int attendeeId)
    {
        var response = new ServiceResponse<IEnumerable<TransactionDto>>();

        try
        {
            var transactions = await _transactionRepo.GetTransactionsByAttendeeIdAsync(attendeeId);
            response.Data = _mapper.Map<IEnumerable<TransactionDto>>(transactions);
            response.Success = true;
            response.Message = "Transactions retrieved successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "An error occurred while retrieving transactions.";
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }

    public async Task<ServiceResponse<TransactionDto>> GetTransactionByIdAsync(int id)
    {
        var response = new ServiceResponse<TransactionDto>();

        try
        {
            var transaction = await _transactionRepo.GetByIdAsync(id);
            if (transaction == null)
            {
                response.Success = false;
                response.Message = "Transaction not found.";
                return response;
            }

            response.Data = _mapper.Map<TransactionDto>(transaction);
            response.Success = true;
            response.Message = "Transaction retrieved successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "An error occurred while retrieving the transaction.";
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }

    public async Task<ServiceResponse<TransactionDto>> CreateTransactionAsync(CreateTransactionDto dto)
    {
        var response = new ServiceResponse<TransactionDto>();

        try
        {
            var transaction = _mapper.Map<Transaction>(dto);
            await _transactionRepo.AddAsync(transaction);

            response.Data = _mapper.Map<TransactionDto>(transaction);
            response.Success = true;
            response.Message = "Transaction created successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "An error occurred while creating the transaction.";
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }

    public async Task<ServiceResponse<TransactionDto>> UpdateTransactionAsync(int id, TransactionDto dto)
    {
        var response = new ServiceResponse<TransactionDto>();

        try
        {
            var transaction = await _transactionRepo.GetByIdAsync(id);
            if (transaction == null)
            {
                response.Success = false;
                response.Message = "Transaction not found.";
                return response;
            }

            _mapper.Map(dto, transaction);
            await _transactionRepo.UpdateAsync(transaction);

            response.Data = _mapper.Map<TransactionDto>(transaction);
            response.Success = true;
            response.Message = "Transaction updated successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "An error occurred while updating the transaction.";
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }

    public async Task<ServiceResponse<bool>> DeleteTransactionAsync(int id)
    {
        var response = new ServiceResponse<bool>();

        try
        {
            var transaction = await _transactionRepo.GetByIdAsync(id);
            if (transaction == null)
            {
                response.Success = false;
                response.Message = "Transaction not found.";
                return response;
            }

            await _transactionRepo.RemoveAsync(transaction);

            response.Data = true;
            response.Success = true;
            response.Message = "Transaction deleted successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "An error occurred while deleting the transaction.";
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }

    public async Task<ServiceResponse<IEnumerable<TransactionDto>>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var response = new ServiceResponse<IEnumerable<TransactionDto>>();
        try
        {
            var transactions = await _transactionRepo.GetByDateRangeAsync(startDate, endDate);
            response.Data = _mapper.Map<IEnumerable<TransactionDto>>(transactions);
            response.Success = true;
            response.Message = "Transactions retrieved successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "An error occurred while retrieving transactions.";
            response.ErrorMessages = new List<string> { ex.Message };
        }
        return response;
    }

    public async Task<ServiceResponse<decimal>> GetTotalAmountByAttendeeAsync(int attendeeId)
    {
        var response = new ServiceResponse<decimal>();
        try
        {
            var totalAmount = await _transactionRepo.GetTotalAmountByAttendeeIdAsync(attendeeId);
            response.Data = totalAmount;
            response.Success = true;
            response.Message = "Total amount retrieved successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "An error occurred while retrieving the total amount.";
            response.ErrorMessages = new List<string> { ex.Message };
        }
        return response;
    }

    public async Task<ServiceResponse<Transaction>> UpdateTransactionStatusAsync(int transactionId, string status)
    {
        var response = new ServiceResponse<Transaction>();

        try
        {
            var transaction = await _transactionRepo.UpdateTransactionStatusAsync(transactionId, status);

            if (transaction == null)
            {
                response.Success = false;
                response.Message = "Transaction not found.";
            }
            else
            {
                response.Data = transaction;
                response.Success = true;
                response.Message = "Transaction status updated successfully.";
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "An error occurred while updating the transaction status.";
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;

    }
}