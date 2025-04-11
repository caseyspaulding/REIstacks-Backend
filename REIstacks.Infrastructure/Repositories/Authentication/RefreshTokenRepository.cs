using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Entities.Auth;
using REIstacks.Infrastructure.Data;
using REIstacks.Infrastructure.Repositories.BaseRepository;


namespace REIstacks.Infrastructure.Repositories.Authentication
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            return await Context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<IEnumerable<RefreshToken>> GetByProfileIdAsync(Guid profileId)
        {
            return await Context.RefreshTokens
                .Where(rt => rt.ProfileId == profileId)
                .ToListAsync();
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var refreshToken = await GetByTokenAsync(token);
            if (refreshToken == null) return false;

            refreshToken.RevokedAt = DateTime.UtcNow;
            return true;
        }

        public async Task<bool> RevokeAllTokensForProfileAsync(Guid profileId)
        {
            var tokens = await GetByProfileIdAsync(profileId);

            foreach (var token in tokens)
            {
                if (token.RevokedAt == null)
                {
                    token.RevokedAt = DateTime.UtcNow;
                }
            }

            return true;
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            var refreshToken = await GetByTokenAsync(token);

            if (refreshToken == null) return false;

            return refreshToken.RevokedAt == null && refreshToken.ExpiresAt > DateTime.UtcNow;
        }
    }
}