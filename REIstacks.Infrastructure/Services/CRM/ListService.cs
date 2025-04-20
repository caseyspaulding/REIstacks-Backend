using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Domain.Entities.Properties;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Infrastructure.Services.CRM;
public class ListService : IListService
{
    private readonly AppDbContext _db;

    public ListService(AppDbContext db)
    {
        _db = db;
    }

    // ─── List CRUD ───────────────────

    public async Task<int> CreateListAsync(List list)
    {
        _db.Lists.Add(list);
        await _db.SaveChangesAsync();
        return list.Id;
    }

    public async Task<bool> UpdateListAsync(List list)
    {
        var existing = await _db.Lists
            .FirstOrDefaultAsync(l => l.Id == list.Id && l.OrganizationId == list.OrganizationId);
        if (existing == null) return false;

        existing.Name = list.Name;
        existing.Description = list.Description;
        // …any other updatable props…

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteListAsync(int listId, string organizationId)
    {
        var list = await _db.Lists
            .FirstOrDefaultAsync(l => l.Id == listId && l.OrganizationId == organizationId);
        if (list == null) return false;

        _db.Lists.Remove(list);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<List>> GetListsAsync(string organizationId)
        => await _db.Lists
               .Where(l => l.OrganizationId == organizationId)
               .ToListAsync();

    public async Task<List?> GetListByIdAsync(int listId, string organizationId)
        => await _db.Lists
               .Include(l => l.PropertyLists)
                   .ThenInclude(pl => pl.Property)
               .FirstOrDefaultAsync(l => l.Id == listId
                                      && l.OrganizationId == organizationId);

    // ─── Manage Properties in a List ───────────────────

    public async Task AddPropertyToListAsync(int listId, int propertyId, string organizationId)
    {
        // prevent duplicates
        var exists = await _db.PropertyLists
            .AnyAsync(pl => pl.ListId == listId
                         && pl.PropertyId == propertyId
                         && pl.OrganizationId == organizationId);
        if (exists) return;

        var pl = new PropertyList
        {
            ListId = listId,
            PropertyId = propertyId,
            OrganizationId = organizationId,
            AddedAt = DateTime.UtcNow
        };

        _db.PropertyLists.Add(pl);
        await _db.SaveChangesAsync();
    }

    public async Task RemovePropertyFromListAsync(int listId, int propertyId, string organizationId)
    {
        var pl = await _db.PropertyLists
            .FirstOrDefaultAsync(x => x.ListId == listId
                                   && x.PropertyId == propertyId
                                   && x.OrganizationId == organizationId);
        if (pl == null) return;

        _db.PropertyLists.Remove(pl);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Property>> GetPropertiesByListAsync(int listId, string organizationId)
        => await _db.PropertyLists
               .Where(pl => pl.ListId == listId
                         && pl.OrganizationId == organizationId)
               .Include(pl => pl.Property)
               .Select(pl => pl.Property!)
               .ToListAsync();
}