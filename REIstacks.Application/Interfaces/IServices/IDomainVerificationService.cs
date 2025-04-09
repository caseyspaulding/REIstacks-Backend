using REIstacks.Domain.Models;

namespace REIstacks.Application.Interfaces;

public interface IDomainVerificationService
{
    Task<string> GenerateVerificationTokenAsync(string organizationId, string domain);
    Task<bool> VerifyDomainAsync(string organizationId, string domain, string token);
    Task<IEnumerable<DomainVerification>> GetPendingVerificationsAsync(string organizationId);
}