using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Infrastructure.Services.CRM;
public class SalesLeaderBoardService : ISalesLeaderBoardService
{
    private readonly AppDbContext _db;
    public SalesLeaderBoardService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<SalesLeaderBoardRequest>> GetLeaderBoardAsync(string organizationId)
    {
        return await _db.ContactActivities
            .Where(a => a.OrganizationId == organizationId)
            .GroupBy(a => a.CreatedByProfileId)
            .Where(g => g.Key != Guid.Empty) // Fix: Check if Guid is not empty instead of using HasValue
            .Select(g => new SalesLeaderBoardRequest
            {
                UserId = g.Key,
                UserName = g.First().CreatedByProfile.Name,
                AvatarUrl = g.First().CreatedByProfile.AvatarUrl,
                CallsMade = g.Count(a => a.Type == ActivityType.Call),
                AppointmentsSet = g.Count(a => a.Type == ActivityType.AppointmentSet),
                ContactsMade = g.Count(a => a.Type == ActivityType.Contacted)
            })
            .OrderByDescending(x => x.CallsMade + x.AppointmentsSet + x.ContactsMade)
            .ToListAsync();
    }
}
