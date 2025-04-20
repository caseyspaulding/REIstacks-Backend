using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Application.Interfaces.IServices;
public interface ISkipTraceService
{
    Task<(IEnumerable<SkipTraceActivity> Items, int TotalCount)>
        GetActivitiesAsync(int page, int pageSize, string orgId);

    Task<IEnumerable<SkipTraceBreakdown>>
        GetBreakdownAsync(int activityId, string orgId);

    Task<int> CreateActivityAsync(SkipTraceActivity activity);
    // … other methods (update status, etc.) as needed …
}