using Microsoft.EntityFrameworkCore;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using System.Linq.Expressions;

namespace MobID.MainGateway.Repo
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class, IBaseEntity
    {
        private readonly MainDbContext _context;

        public GenericRepository(MainDbContext context)
        {
            _context = context;
        }

        public Task<T?> GetById(Guid id, CancellationToken ct = default)
        {
            return _context.Set<T>().SingleOrDefaultAsync(e => e.Id == id, ct);
        }

        public Task<T?> FirstOrDefault(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return _context.Set<T>().FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<int> Add(T entity, CancellationToken ct = default)
        {
            await _context.Set<T>().AddAsync(entity, ct);
            return await _context.SaveChangesAsync(ct);
        }

        public async Task<int> Update(T entity, CancellationToken ct = default)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return await _context.SaveChangesAsync(ct);
        }

        public async Task<int> Remove(T entity, CancellationToken ct = default)
        {
            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync(ct);
        }
        public async Task<bool> IsIdPresent(Guid id, CancellationToken ct = default)
        {
            return await _context.Set<T>().AnyAsync(x => x.Id == id, ct);
        }

        public async Task<IEnumerable<T>> GetAll(CancellationToken ct = default)
        {
            return await _context.Set<T>().ToListAsync(ct);
        }

        public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync(ct);
        }

        public Task<int> CountAll(CancellationToken ct = default)
        {
            return _context.Set<T>().CountAsync(ct);
        }

        public Task<int> CountWhere(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return _context.Set<T>().CountAsync(predicate, ct);
        }

        public async Task<T?> GetByIdWithInclude(Guid id, CancellationToken ct = default, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = IncludeProperties(ct, includeProperties);
            return await query.FirstOrDefaultAsync(entity => entity.Id == id, ct);
        }

        public async Task<IEnumerable<T>> GetAllWithInclude(CancellationToken ct = default, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> entities = IncludeProperties(ct, includeProperties);

            return await entities.ToListAsync(ct);
        }

        public async Task<IEnumerable<T>> GetWhereWithInclude(Expression<Func<T, bool>> predicate, CancellationToken ct = default, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> entities = IncludeProperties(ct, includeProperties);
            return await entities.Where(predicate).ToListAsync(ct);
        }

        private IQueryable<T> IncludeProperties(CancellationToken ct = default, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> entities = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                entities = entities.Include(includeProperty);
            }
            return entities;
        }

        /*public async Task<PaginatedResult<T>> GetPagedData<T>(PagedRequest pagedRequest, CancellationToken ct = default) where T : class, IBaseEntity
        {
            return await _context.Set<T>().CreatePaginatedResultAsync<T>(pagedRequest);
        }*/
    }
}
