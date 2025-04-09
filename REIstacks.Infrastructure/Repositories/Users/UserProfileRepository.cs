using Microsoft.EntityFrameworkCore;
using REIstack.Domain.Models;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Infrastructure.Data;
using REIstacks.Infrastructure.Repositories.BaseRepository;

namespace REIstacks.Infrastructure.Repositories.Users
{
    public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<UserProfile> GetByEmailAsync(string email)
        {
            return await Context.UserProfiles
                .FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<UserProfile> GetByEmailAndOrganizationAsync(string email, string organizationId)
        {
            return await Context.UserProfiles
               .FirstOrDefaultAsync(p => p.Email == email && p.OrganizationId == organizationId);
        }
        public async Task<UserProfile> GetByExternalIdAsync(string externalId, string provider = "Google")
        {
            return await Context.UserProfiles
                .FirstOrDefaultAsync(p => p.ExternalId == externalId && p.ExternalProvider == provider);
        }

        public async Task<IEnumerable<UserProfile>> GetByOrganizationRoleIdAsync(int organizationRoleId)
        {
            return await Context.UserProfiles
                .Where(p => p.OrganizationRoleId == organizationRoleId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserProfile>> GetByOrganizationIdAsync(string organizationId)
        {
            return await Context.UserProfiles
                .Where(p => p.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrganizationAsync(Guid profileId, string organizationId)
        {
            var profile = await GetByIdAsync(profileId);
            if (profile == null) return false;

            profile.OrganizationId = organizationId;
            profile.UpdatedAt = DateTime.UtcNow;
            return true;
        }

        public async Task<bool> UpdateIsSetupCompleteAsync(Guid profileId, bool isSetupComplete)
        {
            var profile = await GetByIdAsync(profileId);
            if (profile == null) return false;

            profile.IsSetupComplete = isSetupComplete;
            profile.UpdatedAt = DateTime.UtcNow;
            return true;
        }

        public async Task SaveRefreshTokenAsync(Guid profileId, string refreshToken)
        {
            var profile = await GetByIdAsync(profileId);
            if (profile == null)
                throw new Exception($"User with ID {profileId} not found");

            var tokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                ProfileId = profileId,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

            await Context.RefreshTokens.AddAsync(tokenEntity);

            // Also update the single RefreshToken property for backward compatibility
            profile.RefreshToken = refreshToken;
            profile.UpdatedAt = DateTime.UtcNow;
        }
        public async Task<UserProfile> GetByIdWithOrganizationAsync(Guid id)
        {
            return await Context.UserProfiles
                .Include(p => p.Organization)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // We can access the AppDbContext through the Context property
        private AppDbContext AppDbContext => Context;
    }
}