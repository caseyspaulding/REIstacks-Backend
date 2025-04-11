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