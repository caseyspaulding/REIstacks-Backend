using REIstacks.Application.Common;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Common;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IDomainEventDispatcher _eventDispatcher;

        public IUserProfileRepository UserProfiles { get; }
        public IOrganizationRepository Organizations { get; }
        public ILeadRepository Leads { get; }
        public IInvitationRepository Invitations { get; }
        public IActivityLogRepository ActivityLogs { get; }
        public IRefreshTokenRepository RefreshTokens { get; }
        public IExternalAuthRepository ExternalAuths { get; }
        public IDomainVerificationRepository DomainVerifications { get; }
        public IStripeSubscriptionRepository StripeSubscriptions { get; }
        public IOrganizationRoleRepository organizationRoleRepository { get; }

        // Add event dispatcher to constructor
        public UnitOfWork(
            AppDbContext context,
            IUserProfileRepository userProfiles,
            IOrganizationRepository organizations,
            ILeadRepository leads,
            IInvitationRepository invitations,
            IActivityLogRepository activityLogs,
            IRefreshTokenRepository refreshTokens,
            IExternalAuthRepository externalAuths,
            IDomainVerificationRepository domainVerifications,
            IStripeSubscriptionRepository stripeSubscriptions,
            IOrganizationRoleRepository organizationRoleRepository,
            IDomainEventDispatcher eventDispatcher)
        {
            _context = context;
            _eventDispatcher = eventDispatcher;
            UserProfiles = userProfiles;
            Organizations = organizations;
            Leads = leads;
            Invitations = invitations;
            ActivityLogs = activityLogs;
            RefreshTokens = refreshTokens;
            ExternalAuths = externalAuths;
            DomainVerifications = domainVerifications;
            StripeSubscriptions = stripeSubscriptions;
            this.organizationRoleRepository = organizationRoleRepository;
        }

        public async Task<int> CompleteAsync()
        {
            // Find all entities with domain events
            var entitiesWithEvents = _context.ChangeTracker.Entries<Entity>()
                .Select(e => e.Entity)
                .Where(e => e.GetDomainEvents().Any())
                .ToArray();

            // Save changes first
            var result = await _context.SaveChangesAsync();

            // Then dispatch events
            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.GetDomainEvents();
                entity.ClearDomainEvents();

                foreach (var domainEvent in events)
                {
                    await _eventDispatcher.DispatchAsync(domainEvent);
                }
            }

            return result;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}