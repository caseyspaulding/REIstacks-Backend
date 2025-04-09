using REIstack.Domain.Models;
using REIstacks.Domain.Repositories;

namespace REIstacks.Application.Repositories.Interfaces;

public interface ILeadRepository : IRepository<Lead>
{
    Task<IEnumerable<Lead>> GetByOrganizationIdAsync(string organizationId);
    Task<int> GetLeadCountByOrganizationIdAsync(string organizationId);
    Task<bool> ImportLeadsAsync(string organizationId, IEnumerable<Lead> leads);
}