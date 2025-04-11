using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Repositories;

namespace REIstacks.Application.Repositories.Interfaces;

public interface IDomainVerificationRepository : IRepository<DomainVerification>
{
    Task<DomainVerification> GetByDomainAsync(string domain);
    Task<IEnumerable<DomainVerification>> GetByOrganizationIdAsync(string organizationId);
    Task<bool> MarkAsVerifiedAsync(string domain, string organizationId);
}