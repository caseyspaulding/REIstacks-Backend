using REIstacks.Domain.Common;
using System.Linq.Expressions;

namespace REIstacks.Domain.Repositories;
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(object id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    Task AddAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
    Task<PaginatedResult<TEntity>> GetPaginatedAsync(
           Expression<Func<TEntity, bool>> filter = null,
           int page = 1,
           int pageSize = 20,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);
}