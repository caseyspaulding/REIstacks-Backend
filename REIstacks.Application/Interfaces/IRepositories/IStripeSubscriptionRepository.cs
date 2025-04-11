using REIstacks.Domain.Entities.Billing;
using REIstacks.Domain.Repositories;

namespace REIstacks.Application.Repositories.Interfaces;

public interface IStripeSubscriptionRepository : IRepository<StripeSubscription>
{
    Task<StripeSubscription> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId);
    Task<IEnumerable<StripeSubscription>> GetByOrganizationIdAsync(string organizationId);
    Task<bool> UpdateStatusAsync(string stripeSubscriptionId, SubscriptionStatus status);
}
