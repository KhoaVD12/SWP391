using System.Linq.Expressions;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject.Repo;

public class RepoBase<T> :IGenericRepo<T> where T : class
{
    protected readonly DbSet<T> _dbSet;
    protected readonly TicketContext _context;

    protected RepoBase(TicketContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();  
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        _ = await _dbSet.AddAsync(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _ = _dbSet.Update(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(T entity)
    {
        _ = _dbSet.Remove(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(IEnumerable<T> entities)
    {
       _dbSet.RemoveRange(entities);
       await _context.SaveChangesAsync();
    }
}