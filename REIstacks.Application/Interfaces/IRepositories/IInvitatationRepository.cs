using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Repositories;

namespace REIstacks.Application.Repositories.Interfaces;

public interface IInvitationRepository : IRepository<Invitation>
{
    Task<Invitation> GetByTokenAsync(string token);
    Task<IEnumerable<Invitation>> GetByOrganizationIdAsync(string organizationId);
    Task<IEnumerable<Invitation>> GetByEmailAsync(string email);
    Task<bool> MarkAsAcceptedAsync(string token, Guid acceptedByProfileId);
}