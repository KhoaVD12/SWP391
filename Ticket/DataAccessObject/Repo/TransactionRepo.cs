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

    public async Task<IEnumerable<Transaction>> GetTransactions()
    {
        return await _context.Transactions
            .Include(t => t.PaymentMethodNavigation)
            .Include(t => t.Attendee)
            .ThenInclude(a => a.AttendeeDetails)
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

    public async Task<Transaction?> UpdateTransactionStatusAsync(int transactionId, string status)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId);

        if (transaction == null) return null;

        transaction.Status = status;
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<Transaction?> GetTransactionByAttendeeIdAsync(int attendeeId)
    {
        return await _context.Transactions
            .Include(t => t.PaymentMethodNavigation)
            .FirstOrDefaultAsync(t => t.AttendeeId == attendeeId);
    }

    public async Task<Transaction?> GetByIdWithPaymentAsync(int transactionId)
    {
        return await _context.Transactions
            .Include(t => t.PaymentMethodNavigation)
            .Include(t => t.Attendee)
            .ThenInclude(a => a.Ticket)
            .FirstOrDefaultAsync(t => t.Id == transactionId);
    }
}