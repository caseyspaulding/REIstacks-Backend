using REIstacks.Application.Interfaces.IRepositories;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;

        public IContactRepository Contacts { get; }
        public IUserProfileRepository UserProfiles { get; }
        public IOrganizationRoleRepository OrganizationRoles { get; set; }
        public IOrganizationRepository Organizations { get; }
        public ILeadRepository Leads { get; }
        public IInvitationRepository Invitations { get; }
        public IActivityLogRepository ActivityLogs { get; }
        public IRefreshTokenRepository RefreshTokens { get; }
        public IExternalAuthRepository ExternalAuths { get; }
        public IDomainVerificationRepository DomainVerifications { get; }
        public IStripeSubscriptionRepository StripeSubscriptions { get; }


        public UnitOfWork(
            AppDbContext context,
            IContactRepository contacts,
            IUserProfileRepository userProfiles,
            IOrganizationRepository organizations,
            ILeadRepository leads,
            IInvitationRepository invitations,
            IActivityLogRepository activityLogs,
            IRefreshTokenRepository refreshTokens,
            IExternalAuthRepository externalAuths,
            IDomainVerificationRepository domainVerifications,
            IStripeSubscriptionRepository stripeSubscriptions,
            IOrganizationRoleRepository organizationRoles)
        {
            _context = context;
            Contacts = contacts;
            UserProfiles = userProfiles;
            Organizations = organizations;
            Leads = leads;
            Invitations = invitations;
            ActivityLogs = activityLogs;
            RefreshTokens = refreshTokens;
            ExternalAuths = externalAuths;
            DomainVerifications = domainVerifications;
            StripeSubscriptions = stripeSubscriptions;
            OrganizationRoles = organizationRoles;
        }

        /// <summary>
        /// Persist everything and (via the DbContext override) dispatch any raised domain events.
        /// </summary>
        public Task<int> CompleteAsync()
            => _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
