using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Application.Interfaces.IServices;
public interface IOpportunityService
{
    Task<IEnumerable<Opportunity>> GetAllAsync(string orgId);
    Task<Opportunity?> GetByIdAsync(int id, string orgId);
    Task<int> CreateAsync(Opportunity opp);
    Task<bool> UpdateAsync(Opportunity opp);
    Task<bool> DeleteAsync(int id, string orgId);

    // Assignment
    Task<bool> AssignToUserAsync(int opportunityId, Guid userProfileId, string organizationId);
    Task<bool> UnassignAsync(int opportunityId, string organizationId);
}