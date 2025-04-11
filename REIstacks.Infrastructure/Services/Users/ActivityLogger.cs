using REIstacks.Application.Interfaces;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Entities.User;


namespace REIstacks.Application.Services.Users;

public class ActivityLogger : IActivityLogger
{
    private readonly IActivityLogRepository _activityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivityLogger(IActivityLogRepository activityRepository, IUnitOfWork unitOfWork)
    {
        _activityRepository = activityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task LogActivityAsync(string organizationId, Guid userId,
        ActivityType activityType, string ipAddress = "Unknown")
    {
        if (string.IsNullOrEmpty(organizationId)) return;

        var activity = new ActivityLog
        {
            OrganizationId = organizationId,
            UserId = userId,
            Action = activityType.ToString(),
            Timestamp = DateTime.UtcNow,
            IpAddress = ipAddress,
            ActionType = activityType.ToString(),
        };

        await _activityRepository.AddAsync(activity);
        await _unitOfWork.CompleteAsync();
    }
}