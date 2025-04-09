using REIstack.Domain.Models;

namespace REIstacks.Application.Repositories.Interfaces;

public interface IOrganizationRoleRepository
{
    Task<IEnumerable<OrganizationRole>> GetByOrganizationIdAsync(string organizationId);
    Task<OrganizationRole> GetByIdAsync(int id);
    Task<OrganizationRole> GetByNameAndOrganizationAsync(string name, string organizationId);
    Task<bool> ExistsByNameAsync(string name, string organizationId, int? excludeId = null);
    Task AddAsync(OrganizationRole role);
    void Update(OrganizationRole role);
    Task DeleteAsync(int id);
    Task<bool> HasUsersAssignedAsync(int roleId);
    Task<IEnumerable<string>> GetSystemRoleNamesAsync();
    Task<int> GetMemberRoleIdForOrganizationAsync(string organizationId);
    Task SeedDefaultRolesAsync(string organizationId);
}
