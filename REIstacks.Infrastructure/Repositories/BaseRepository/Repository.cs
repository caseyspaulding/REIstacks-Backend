using Microsoft.EntityFrameworkCore;
using REIstacks.Domain.Common;
using REIstacks.Domain.Repositories;
using REIstacks.Infrastructure.Data;
using System.Linq.Expressions;

namespace REIstacks.Infrastructure.Repositories.BaseRepository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly AppDbContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(AppDbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }
        public virtual async Task<PaginatedResult<TEntity>> GetPaginatedAsync(
            Expression<Func<TEntity, bool>> filter = null,
            int page = 1,
            int pageSize = 20,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var totalCount = await query.CountAsync();

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<TEntity>(items, totalCount, page, pageSize);
        }
        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            // If id is a string but the entity has a Guid Id property, try to convert
            if (id is string stringId)
            {
                // Get the type of the primary key
                var keyType = typeof(TEntity).GetProperty("Id")?.PropertyType;

                // If the key type is Guid, we need to convert the string
                if (keyType == typeof(Guid))
                {
                    if (Guid.TryParse(stringId, out Guid guidId))
                    {
                        return await DbSet.FindAsync(guidId);
                    }
                    throw new ArgumentException($"Could not convert '{stringId}' to a valid Guid");
                }
            }

            // Default case - use the id as provided
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.SingleOrDefaultAsync(predicate);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }

        public virtual void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }

        public virtual void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }
    }
}