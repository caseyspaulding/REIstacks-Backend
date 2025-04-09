using REIstacks.Domain.Models;
using REIstacks.Domain.Repositories;

namespace REIstacks.Application.Repositories.Interfaces;

public interface IExternalAuthRepository : IRepository<ExternalAuth>
{
    Task<ExternalAuth> GetByExternalIdAsync(string externalId, string provider);
    Task<ExternalAuth> GetByProfileIdAndProviderAsync(Guid profileId, string provider);
    Task<bool> UpdateTokensAsync(Guid id, string accessToken, string refreshToken, DateTime? expiresAt);
}