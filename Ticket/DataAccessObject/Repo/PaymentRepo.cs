using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject.Repo;

public class PaymentRepo : RepoBase<Payment>, IPaymentRepo
{
    private readonly TicketContext _context;

    public PaymentRepo(TicketContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
    {
        return await _context.Payments.ToListAsync();
    }

    public async Task<Payment> GetPaymentByIdAsync(int id)
    {
        return await _context.Payments.FindAsync(id);
    }

    public async Task AddPaymentAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePaymentAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePaymentAsync(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment != null)
        {
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }
    }
}