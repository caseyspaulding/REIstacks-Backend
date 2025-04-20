using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Infrastructure.Services.CRM;
public class SkipTraceService : ISkipTraceService
{
    private readonly AppDbContext _db;
    public SkipTraceService(AppDbContext db) => _db = db;

    public async Task<(IEnumerable<SkipTraceActivity>, int)> GetActivitiesAsync(
        int page, int pageSize, string orgId)
    {
        var query = _db.SkipTraceActivities
                       .Where(x => x.OrganizationId == orgId)
                       .OrderByDescending(x => x.Id);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IEnumerable<SkipTraceBreakdown>> GetBreakdownAsync(
        int activityId, string orgId)
    {
        var exists = await _db.SkipTraceActivities
                     .AnyAsync(a => a.Id == activityId && a.OrganizationId == orgId);
        if (!exists) throw new KeyNotFoundException("Activity not found");

        return await _db.SkipTraceBreakdowns
            .Where(b => b.SkipTraceActivityId == activityId)
            .ToListAsync();
    }

    public async Task<int> CreateActivityAsync(SkipTraceActivity activity)
    {
        _db.SkipTraceActivities.Add(activity);
        await _db.SaveChangesAsync();
        return activity.Id;
    }
}
