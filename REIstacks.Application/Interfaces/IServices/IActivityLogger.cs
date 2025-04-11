using REIstacks.Domain.Entities.User;

namespace REIstacks.Application.Interfaces;

public interface IActivityLogger
{
    Task LogActivityAsync(string organizationId, Guid userId, ActivityType activityType, string ipAddress = "Unknown");
}
