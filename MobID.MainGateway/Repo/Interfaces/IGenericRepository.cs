using MobID.MainGateway.Models.Entities;
using System.Linq.Expressions;

namespace MobID.MainGateway.Repo.Interfaces
{
    public interface IGenericRepository<T>
        where T : class, IBaseEntity
    {
        Task<T?> GetById(Guid id, CancellationToken ct = default);

        Task<T?> FirstOrDefault(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

        Task<int> Add(T entity, CancellationToken ct = default);

        Task<int> Update(T entity, CancellationToken ct = default);

        Task<int> Remove(T entity, CancellationToken ct = default);

        Task<bool> IsIdPresent(Guid id, CancellationToken ct = default);

        Task<IEnumerable<T>> GetAll(CancellationToken ct = default);

        Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

        Task<int> CountAll(CancellationToken ct = default);

        Task<int> CountWhere(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

        Task<T?> GetByIdWithInclude(Guid id, CancellationToken ct = default, params Expression<Func<T, object>>[] includeProperties);

        Task<IEnumerable<T>> GetAllWithInclude(CancellationToken ct = default, params Expression<Func<T, object>>[] includeProperties);

        Task<IEnumerable<T>> GetWhereWithInclude(Expression<Func<T, bool>> predicate, CancellationToken ct = default, params Expression<Func<T, object>>[] includeProperties);

        //Task<PaginatedResult<T>> GetPagedData<T>(PagedRequest pagedRequest, CancellationToken ct = default) where T : class, IBaseEntity;
    }
}

