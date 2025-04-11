using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Infrastructure.Repositories.Organizations
{
    public class OrganizationRoleRepository : IOrganizationRoleRepository
    {
        private readonly AppDbContext _context;

        public OrganizationRoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrganizationRole>> GetByOrganizationIdAsync(string organizationId)
        {
            return await _context.OrganizationRoles
                .Where(r => r.OrganizationId == organizationId)
                .OrderBy(r => r.IsCustom) // System roles first
                .ThenBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<OrganizationRole> GetByIdAsync(int id)
        {
            return await _context.OrganizationRoles
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<OrganizationRole> GetByNameAndOrganizationAsync(string name, string organizationId)
        {
            return await _context.OrganizationRoles
                .FirstOrDefaultAsync(r => r.Name == name && r.OrganizationId == organizationId);
        }

        public async Task<bool> ExistsByNameAsync(string name, string organizationId, int? excludeId = null)
        {
            var query = _context.OrganizationRoles
                .Where(r => r.Name == name && r.OrganizationId == organizationId);

            if (excludeId.HasValue)
            {
                query = query.Where(r => r.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task AddAsync(OrganizationRole role)
        {
            await _context.OrganizationRoles.AddAsync(role);
        }

        public void Update(OrganizationRole role)
        {
            _context.OrganizationRoles.Update(role);
        }

        public async Task DeleteAsync(int id)
        {
            var role = await GetByIdAsync(id);
            if (role != null)
            {
                _context.OrganizationRoles.Remove(role);
            }
        }

        public async Task<bool> HasUsersAssignedAsync(int roleId)
        {
            return await _context.UserProfiles
                .AnyAsync(p => p.OrganizationRoleId == roleId);
        }

        public async Task<IEnumerable<string>> GetSystemRoleNamesAsync()
        {
            return await _context.OrganizationRoles
                .Where(r => !r.IsCustom)
                .Select(r => r.Name)
                .Distinct()
                .ToListAsync();
        }

        public async Task<int> GetMemberRoleIdForOrganizationAsync(string organizationId)
        {
            var memberRole = await _context.OrganizationRoles
                .FirstOrDefaultAsync(r => r.OrganizationId == organizationId &&
                                          r.Name == "Member" &&
                                          !r.IsCustom);

            return memberRole?.Id ?? 0;
        }

        public async Task SeedDefaultRolesAsync(string organizationId)
        {
            // Skip if organization already has system roles
            var hasSystemRoles = await _context.OrganizationRoles
                .AnyAsync(r => r.OrganizationId == organizationId && !r.IsCustom);

            if (hasSystemRoles)
                return;

            var defaultRoles = new[]
            {
                new OrganizationRole
                {
                    OrganizationId = organizationId,
                    Name = "Owner",
                    Description = "Full access to all organization settings",
                    IsCustom = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new OrganizationRole
                {
                    OrganizationId = organizationId,
                    Name = "Admin",
                    Description = "Administrative access to organization",
                    IsCustom = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new OrganizationRole
                {
                    OrganizationId = organizationId,
                    Name = "Member",
                    Description = "Standard member access",
                    IsCustom = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new OrganizationRole
                {
                    OrganizationId = organizationId,
                    Name = "SuperAdmin",
                    Description = "Super administrative access",
                    IsCustom = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await _context.OrganizationRoles.AddRangeAsync(defaultRoles);
            await _context.SaveChangesAsync();
        }
    }
}