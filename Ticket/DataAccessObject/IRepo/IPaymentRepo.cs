using DataAccessObject.Entities;

namespace DataAccessObject.IRepo;

public interface IPaymentRepo : IGenericRepo<Payment>
{
    Task<IEnumerable<Payment>> GetAllPaymentsAsync();
    Task<Payment> GetPaymentByIdAsync(int id);
    Task AddPaymentAsync(Payment payment);
    Task UpdatePaymentAsync(Payment payment);
    Task DeletePaymentAsync(int id);
}