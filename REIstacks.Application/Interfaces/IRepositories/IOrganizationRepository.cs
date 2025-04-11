using REIstacks.Domain.Entities.Billing;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Repositories;

namespace REIstacks.Application.Repositories.Interfaces;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> GetBySubdomainAsync(string subdomain);
    Task<Organization> GetByCustomDomainAsync(string customDomain);
    Task<IEnumerable<Organization>> GetByOwnerIdAsync(string ownerId);
    Task<bool> IsSubdomainAvailableAsync(string subdomain);
    Task<bool> UpdateOwnerAsync(string organizationId, string newOwnerId);
    Task<bool> UpdateSubscriptionStatusAsync(string organizationId, SubscriptionStatus status);
    Task<bool> SubdomainExistsAsync(string Subdomain);
}