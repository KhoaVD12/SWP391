using DataAccessObject.Entities;

namespace DataAccessObject.IRepo;

public interface ITransactionRepo  : IGenericRepo<Transaction>
{
   
    Task<IEnumerable<Transaction>> GetTransactionsByAttendeeIdAsync(int attendeeId);
    Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalAmountByAttendeeIdAsync(int attendeeId);
}