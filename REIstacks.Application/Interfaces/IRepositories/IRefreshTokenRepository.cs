using REIstacks.Domain.Entities.Auth;
using REIstacks.Domain.Repositories;

namespace REIstacks.Application.Repositories.Interfaces;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken> GetByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> GetByProfileIdAsync(Guid profileId);
    Task<bool> RevokeTokenAsync(string token);
    Task<bool> RevokeAllTokensForProfileAsync(Guid profileId);
    Task<bool> IsTokenValidAsync(string token);
}