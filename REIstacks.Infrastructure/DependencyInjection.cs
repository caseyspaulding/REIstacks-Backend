// REIstacks.Infrastructure/DependencyInjection.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using REIstacks.Application.Interfaces;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Application.Services.Interfaces;
using REIstacks.Application.Services.Users;
using REIstacks.Infrastructure.Data;
using REIstacks.Infrastructure.Repositories;
using REIstacks.Infrastructure.Repositories.Authentication;
using REIstacks.Infrastructure.Repositories.LeadGeneration;
using REIstacks.Infrastructure.Repositories.Marketing;
using REIstacks.Infrastructure.Repositories.Organizations;
using REIstacks.Infrastructure.Repositories.Users;
using REIstacks.Infrastructure.Services;
using REIstacks.Infrastructure.Services.Authentication;
using REIstacks.Infrastructure.Services.Organizations;

namespace REIstacks.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddPersistence(services, configuration);

        // Add other infrastructure registrations as needed
        // AddAuthentication(services, configuration);
        // AddCaching(services, configuration);
        // etc.

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection") ??
                                 throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register repositories
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<IInvitationRepository, InvitationRepository>();
        services.AddScoped<IActivityLogger, ActivityLogger>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IExternalAuthRepository, ExternalAuthRepository>();
        services.AddScoped<IDomainVerificationRepository, DomainVerificationRepository>();
        services.AddScoped<IStripeSubscriptionRepository, StripeSubscriptionRepository>();
        services.AddScoped<IOrganizationRoleRepository, OrganizationRoleRepository>();
        services.AddScoped<IBlogRepository, BlogRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
        services.AddHttpClient();
        services.AddScoped<EventGridPublisher>(sp =>
        {
            var topicEndpoint = configuration["EventGrid:TopicEndpoint"];
            var topicKey = configuration["EventGrid:TopicKey"];

            if (string.IsNullOrEmpty(topicEndpoint))
                throw new InvalidOperationException("EventGrid:TopicEndpoint is missing in configuration");

            if (string.IsNullOrEmpty(topicKey))
                throw new InvalidOperationException("EventGrid:TopicKey is missing in configuration");

            return new EventGridPublisher(topicEndpoint, topicKey);
        });
    }
}