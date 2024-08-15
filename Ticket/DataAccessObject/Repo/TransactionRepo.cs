using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject.Repo;

public class TransactionRepo : RepoBase<Transaction>, ITransactionRepo
{
    private readonly TicketContext _context;

    public TransactionRepo(TicketContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByAttendeeIdAsync(int attendeeId)
    {
        return await _context.Transactions
            .Include(t => t.Attendee)
            .Include(t => t.PaymentMethodNavigation)
            .Where(t => t.AttendeeId == attendeeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Transactions
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalAmountByAttendeeIdAsync(int attendeeId)
    {
        return await _context.Transactions
            .Where(t => t.AttendeeId == attendeeId)
            .SumAsync(t => t.Amount);
    }
}