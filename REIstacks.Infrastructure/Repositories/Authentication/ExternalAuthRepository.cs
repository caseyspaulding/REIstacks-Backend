// ExternalAuthRepository.cs
using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Entities.Auth;
using REIstacks.Infrastructure.Data;
using REIstacks.Infrastructure.Repositories.BaseRepository;


namespace REIstacks.Infrastructure.Repositories.Authentication
{
    public class ExternalAuthRepository : Repository<ExternalAuth>, IExternalAuthRepository
    {
        public ExternalAuthRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ExternalAuth> GetByExternalIdAsync(string externalId, string provider)
        {
            return await Context.ExternalAuths
                .FirstOrDefaultAsync(ea => ea.ExternalId == externalId && ea.Provider == provider);
        }

        public async Task<ExternalAuth> GetByProfileIdAndProviderAsync(Guid profileId, string provider)
        {
            return await Context.ExternalAuths
                .FirstOrDefaultAsync(ea => ea.ProfileId == profileId && ea.Provider == provider);
        }

        public async Task<bool> UpdateTokensAsync(Guid id, string accessToken, string refreshToken, DateTime? expiresAt)
        {
            var externalAuth = await GetByIdAsync(id);
            if (externalAuth == null) return false;

            externalAuth.AccessToken = accessToken;
            externalAuth.RefreshToken = refreshToken;
            externalAuth.ExpiresAt = expiresAt;
            externalAuth.UpdatedAt = DateTime.UtcNow;

            return true;
        }
    }
}