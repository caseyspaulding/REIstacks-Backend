// Infrastructure/Services/CRM/ContactActivityService.cs
using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Infrastructure.Data;

public class ContactActivityService : IContactActivityService
{
    private readonly AppDbContext _db;
    public ContactActivityService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ContactActivity>> GetForContactAsync(
        int contactId,
        string organizationId,
        ActivityType? filterType = null,
        Guid? filterUserId = null,
        DateTime? from = null,
        DateTime? to = null
    )
    {
        // (ensure organization ownership)
        if (!await _db.Contacts.AnyAsync(c => c.Id == contactId && c.OrganizationId == organizationId))
            throw new KeyNotFoundException("Contact not found");

        var q = _db.ContactActivities
            .Include(a => a.CreatedByProfileId)
            .Where(a => a.ContactId == contactId);

        if (filterType.HasValue) q = q.Where(a => a.Type == filterType.Value);
        if (filterUserId.HasValue) q = q.Where(a => a.CreatedByProfileId == filterUserId.Value);
        if (from.HasValue) q = q.Where(a => a.Timestamp >= from.Value);
        if (to.HasValue) q = q.Where(a => a.Timestamp <= to.Value);

        return await q.OrderByDescending(a => a.Timestamp).ToListAsync();
    }

    public async Task<ContactActivity> LogAsync(ContactActivity act)
    {
        _db.ContactActivities.Add(act);
        await _db.SaveChangesAsync();
        return act;
    }

    public async Task<IEnumerable<UserSummaryDto>> GetActingUsersForContactAsync(
    int contactId,
    string organizationId
)
    {
        // first make sure the contact belongs to this org (optional)
        if (!await _db.Contacts.AnyAsync(c => c.Id == contactId && c.OrganizationId == organizationId))
            throw new KeyNotFoundException("Contact not found");

        var userIds = await _db.ContactActivities
            .Where(a => a.ContactId == contactId)
            .Select(a => (Guid?)a.CreatedByProfileId) // Cast to nullable Guid
            .Where(id => id.HasValue)
            .Select(id => id.GetValueOrDefault()) // Use GetValueOrDefault to safely access the value
            .Distinct()
            .ToListAsync();

        var profiles = await _db.UserProfiles
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new UserSummaryDto
            {
                Id = u.Id,
                Name = u.Name,
                AvatarUrl = u.AvatarUrl
            })
            .ToListAsync();

        return profiles;
    }
}

