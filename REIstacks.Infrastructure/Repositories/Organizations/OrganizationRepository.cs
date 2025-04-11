// OrganizationRepository.cs
using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Entities.Billing;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Infrastructure.Data;
using REIstacks.Infrastructure.Repositories.BaseRepository;


namespace REIstacks.Infrastructure.Repositories.Organizations
{
    public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(AppDbContext context) : base(context)
        {
        }



        public async Task<Organization> GetBySubdomainAsync(string subdomain)
        {
            return await Context.Organizations
                .FirstOrDefaultAsync(o => o.Subdomain == subdomain);
        }

        public async Task<Organization> GetByCustomDomainAsync(string customDomain)
        {
            return await Context.Organizations
                .FirstOrDefaultAsync(o => o.CustomDomain == customDomain);
        }

        public async Task<IEnumerable<Organization>> GetByOwnerIdAsync(string ownerId)
        {
            if (Guid.TryParse(ownerId, out Guid ownerGuid))
            {
                return await Context.Organizations
                    .Where(o => o.OwnerId == ownerGuid)
                    .ToListAsync();
            }

            // Return empty list if parsing fails
            return new List<Organization>();
        }
        public async Task<bool> IsSubdomainAvailableAsync(string subdomain)
        {
            return !await Context.Organizations.AnyAsync(o => o.Subdomain == subdomain);
        }

        public async Task<bool> UpdateOwnerAsync(string organizationId, string newOwnerId)
        {
            // Parse the organization ID
            if (!Guid.TryParse(organizationId, out Guid orgGuid))
            {
                return false; // Invalid organization ID format
            }

            // Parse the new owner ID
            if (!Guid.TryParse(newOwnerId, out Guid ownerGuid))
            {
                return false; // Invalid owner ID format
            }

            var organization = await GetByIdAsync(orgGuid);
            if (organization == null) return false;

            organization.OwnerId = ownerGuid;
            organization.UpdatedAt = DateTime.UtcNow;
            return true;
        }



        public async Task<bool> SubdomainExistsAsync(string Subdomain)
        {
            return await Context.Organizations.AnyAsync(o => o.Subdomain == Subdomain);
        }
        public async Task<bool> UpdateSubscriptionStatusAsync(string organizationId, SubscriptionStatus status)
        {
            var organization = await GetByIdAsync(organizationId);
            if (organization == null) return false;

            organization.SubscriptionStatus = status;
            organization.UpdatedAt = DateTime.UtcNow;
            return true;
        }

        // We can access the AppDbContext through the Context property
        private AppDbContext AppDbContext => Context;
    }
}