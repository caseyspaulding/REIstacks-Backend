using REIstacks.Domain.Entities.User;
using REIstacks.Domain.Repositories;

namespace REIstacks.Application.Repositories.Interfaces;

public interface IActivityLogRepository : IRepository<ActivityLog>
{
    Task<IEnumerable<ActivityLog>> GetByOrganizationIdAsync(string organizationId);
    Task<IEnumerable<ActivityLog>> GetByProfileIdAsync(Guid profileId);
    Task<IEnumerable<ActivityLog>> GetByActionTypeAsync(string actionType);
    Task LogActivityAsync(string organizationId, Guid profileId, string actionType, string details);
}