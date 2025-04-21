using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Infrastructure.Services.CRM;
public class OpportunityService : IOpportunityService
{
    private readonly AppDbContext _db;
    public OpportunityService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Opportunity>> GetAllAsync(string orgId) =>
        await _db.Opportunities
                 .Include(o => o.Stage)
                 .Include(o => o.Contact)
                 .Include(o => o.Property)
                 .Where(o => o.OrganizationId == orgId)
                 .ToListAsync();

    public async Task<Opportunity?> GetByIdAsync(int id, string orgId) =>
        await _db.Opportunities
                 .Include(o => o.Stage)
                 .FirstOrDefaultAsync(o => o.Id == id && o.OrganizationId == orgId);

    public async Task<int> CreateAsync(Opportunity opp)
    {
        _db.Opportunities.Add(opp);
        await _db.SaveChangesAsync();
        return opp.Id;
    }

    public async Task<bool> UpdateAsync(Opportunity opp)
    {
        _db.Opportunities.Update(opp);
        return await _db.SaveChangesAsync() > 0;
    }
    public async Task<bool> AssignToUserAsync(int opportunityId, Guid userProfileId, string organizationId)
    {
        var opp = await GetByIdAsync(opportunityId, organizationId);
        if (opp == null) return false;
        opp.OwnerProfileId = userProfileId;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnassignAsync(int opportunityId, string organizationId)
    {
        var opp = await GetByIdAsync(opportunityId, organizationId);
        if (opp == null) return false;
        opp.OwnerProfileId = null;
        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteAsync(int id, string orgId)
    {
        var o = await _db.Opportunities.FirstOrDefaultAsync(x => x.Id == id && x.OrganizationId == orgId);
        if (o == null) return false;
        _db.Opportunities.Remove(o);
        await _db.SaveChangesAsync();
        return true;
    }
}