using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

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

        // Inject repository interfaces instead of creating concrete implementations
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
            IOrganizationRoleRepository organizationRoleRepository)
        {
            _context = context;
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
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}