namespace REIstacks.Application.Interfaces.IServices;
public interface ISkipTraceService
{
    // Start a new skip trace for a batch of contact IDs
    Task<SkipTraceActivity> StartSkipTraceAsync(IEnumerable<int> contactIds, string organizationId);

    // Get a list of all skip trace activities for the current org
    Task<IEnumerable<SkipTraceActivity>> GetActivitiesAsync(string organizationId);

    // Get a single activity (with breakdown & items)
    Task<SkipTraceActivity?> GetActivityByIdAsync(int id, string organizationId);

    // (optionally) Cancel or retry a failed activity
    Task<bool> CancelActivityAsync(int id, string organizationId);
}