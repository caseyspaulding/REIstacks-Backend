// StripeSubscriptionRepository.cs
using Microsoft.EntityFrameworkCore;
using REIstack.Domain.Models;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Infrastructure.Data;
using REIstacks.Infrastructure.Repositories.BaseRepository;

namespace REIstacks.Infrastructure.Repositories.Organizations
{
    public class StripeSubscriptionRepository : Repository<StripeSubscription>, IStripeSubscriptionRepository
    {
        public StripeSubscriptionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<StripeSubscription> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId)
        {
            return await Context.StripeSubscriptions
                .FirstOrDefaultAsync(s => s.StripeSubscriptionId == stripeSubscriptionId);
        }

        public async Task<IEnumerable<StripeSubscription>> GetByOrganizationIdAsync(string organizationId)
        {
            return await Context.StripeSubscriptions
                .Where(s => s.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(string stripeSubscriptionId, SubscriptionStatus status)
        {
            var subscription = await GetByStripeSubscriptionIdAsync(stripeSubscriptionId);
            if (subscription == null) return false;

            subscription.Status = status;
            subscription.UpdatedAt = DateTime.UtcNow;

            return true;
        }
    }
}
