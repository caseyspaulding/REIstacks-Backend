// LeadRepository.cs
using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Infrastructure.Data;
using REIstacks.Infrastructure.Repositories.BaseRepository;


namespace REIstacks.Infrastructure.Repositories.LeadGeneration
{
    public class LeadRepository : Repository<Lead>, ILeadRepository
    {
        public LeadRepository(AppDbContext context) : base(context)
        {
        }

        public class PaginatedResult<T>
        {
            public List<T> Items { get; set; }
            public int TotalCount { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        }

        public async Task<PaginatedResult<Lead>> GetPaginatedAsync(
            System.Linq.Expressions.Expression<Func<Lead, bool>> filter = null,
            int page = 1,
            int pageSize = 20,
            Func<IQueryable<Lead>, IOrderedQueryable<Lead>> orderBy = null)
        {
            // Start with all leads
            IQueryable<Lead> query = Context.Leads;

            // Apply the filter if one was specified
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply ordering if specified, otherwise order by CreatedAt
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                query = query.OrderByDescending(l => l.CreatedAt);
            }

            // Apply pagination
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Return the paginated result
            return new PaginatedResult<Lead>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<Lead>> GetByOrganizationIdAsync(string organizationId)
        {
            return await Context.Leads
                .Where(l => l.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<int> GetLeadCountByOrganizationIdAsync(string organizationId)
        {
            return await Context.Leads
                .CountAsync(l => l.OrganizationId == organizationId);
        }

        public async Task<bool> ImportLeadsAsync(string organizationId, IEnumerable<Lead> leads)
        {
            try
            {
                foreach (var lead in leads)
                {
                    lead.OrganizationId = organizationId;
                    lead.CreatedAt = DateTime.UtcNow;
                    lead.UpdatedAt = DateTime.UtcNow;
                }

                await Context.Leads.AddRangeAsync(leads);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}