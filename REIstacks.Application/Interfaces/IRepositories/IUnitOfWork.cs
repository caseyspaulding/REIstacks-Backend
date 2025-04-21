using REIstacks.Application.Interfaces.IRepositories;

namespace REIstacks.Application.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IContactRepository Contacts { get; }
        IActivityLogRepository ActivityLogs { get; }
        IDomainVerificationRepository DomainVerifications { get; }
        IExternalAuthRepository ExternalAuths { get; }
        IInvitationRepository Invitations { get; }
        ILeadRepository Leads { get; }
        IOrganizationRoleRepository OrganizationRoles { get; set; }
        IOrganizationRepository Organizations { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IStripeSubscriptionRepository StripeSubscriptions { get; }
        IUserProfileRepository UserProfiles { get; }

        Task<int> CompleteAsync();
    }
}
