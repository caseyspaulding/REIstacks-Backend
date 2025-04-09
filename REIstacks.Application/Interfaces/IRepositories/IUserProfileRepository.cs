using REIstack.Domain.Models;
using REIstacks.Domain.Repositories;

namespace REIstacks.Application.Repositories.Interfaces;

public interface IUserProfileRepository : IRepository<UserProfile>
{
    Task<UserProfile> GetByEmailAsync(string email);
    Task<UserProfile> GetByExternalIdAsync(string externalId, string provider = "Google");
    Task<IEnumerable<UserProfile>> GetByOrganizationIdAsync(string organizationId);
    Task<bool> UpdateOrganizationAsync(Guid profileId, string organizationId);
    Task<bool> UpdateIsSetupCompleteAsync(Guid profileId, bool isSetupComplete);
    Task SaveRefreshTokenAsync(Guid profileId, string refreshToken);
    Task<UserProfile> GetByEmailAndOrganizationAsync(string email, string organizationId);
    Task<UserProfile> GetByIdWithOrganizationAsync(Guid id);
    Task<IEnumerable<UserProfile>> GetByOrganizationRoleIdAsync(int organizationRoleId);

}
