namespace REIstacks.Application.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserProfileRepository UserProfiles { get; }
        IOrganizationRepository Organizations { get; }
        ILeadRepository Leads { get; }
        IInvitationRepository Invitations { get; }
        IActivityLogRepository ActivityLogs { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IExternalAuthRepository ExternalAuths { get; }
        IDomainVerificationRepository DomainVerifications { get; }
        IStripeSubscriptionRepository StripeSubscriptions { get; }
        IOrganizationRoleRepository organizationRoleRepository { get; }

        Task<int> CompleteAsync();
    }
}