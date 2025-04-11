namespace REIstacks.Application.Repositories.Interfaces;
public interface IUnitOfWork
{
    IActivityLogRepository ActivityLogs { get; }
    IDomainVerificationRepository DomainVerifications { get; }
    IExternalAuthRepository ExternalAuths { get; }
    IInvitationRepository Invitations { get; }
    ILeadRepository Leads { get; }
    IOrganizationRoleRepository organizationRoleRepository { get; }
    IOrganizationRepository Organizations { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IStripeSubscriptionRepository StripeSubscriptions { get; }
    IUserProfileRepository UserProfiles { get; }

    Task<int> CompleteAsync();
    void Dispose();
}