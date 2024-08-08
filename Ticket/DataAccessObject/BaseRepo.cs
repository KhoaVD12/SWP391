using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class BaseRepo<TEntity>:IBaseRepo<TEntity> where TEntity : class
    {
        private readonly TicketContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public BaseRepo(TicketContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity);


        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities);

        }

        public void Delete(params TEntity[] entities)
        {
            _dbSet.RemoveRange(entities);

        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<TEntity>?> Find(Expression<Func<TEntity, bool>>? filter = null,
            string includeProperties = "", Func<IQueryable<TEntity>,
                IOrderedQueryable<TEntity>>? orderBy = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public async Task<TEntity?> FindOne(Expression<Func<TEntity, bool>>? filter = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties.Split
                         (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return await query.FirstOrDefaultAsync();

        }

        public IQueryable<TEntity> Get()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<TEntity?> GetById(object id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            await _context.SaveChangesAsync(cancellationToken);

        }

        public void Update(params TEntity[] entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }
        public async Task<bool> AddHashKey(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingEntry = _context.ChangeTracker.Entries<TEntity>()
           .SingleOrDefault(e =>
               e.Entity.GetType() == entity.GetType() &&
               e.Properties.Any(p => p.Metadata.IsKey() && Equals(p.CurrentValue, entity.GetType().GetProperty(p.Metadata.Name)?.GetValue(entity))));

                if (existingEntry != null)
                {
                    // Entity with the same key values is being tracked, update its properties
                    existingEntry.CurrentValues.SetValues(entity);
                }
                else
                {
                    // Entity is not being tracked, add it
                    await _dbSet.AddAsync(entity, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public void Add(TEntity entity)
        {
            throw new NotImplementedException();
        }


    }
}
