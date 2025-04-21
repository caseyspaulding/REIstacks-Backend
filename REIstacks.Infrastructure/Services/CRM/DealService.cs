using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.Deals;
using REIstacks.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REIstacks.Infrastructure.Services.CRM;
public class DealService : IDealService
{
    private readonly AppDbContext _db;

    public DealService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Deal>> GetDealsAsync(string organizationId)
    {
        return await _db.Deals
            .Where(d => d.OrganizationId == organizationId)
            .ToListAsync();
    }

    public async Task<Deal?> GetDealByIdAsync(int id, string organizationId)
    {
        return await _db.Deals
            .FirstOrDefaultAsync(d => d.Id == id && d.OrganizationId == organizationId);
    }

    public async Task<Deal> CreateDealAsync(Deal deal)
    {
        _db.Deals.Add(deal);
        await _db.SaveChangesAsync();
        return deal;
    }

    public async Task<bool> UpdateDealAsync(Deal deal, string organizationId)
    {
        if (deal.OrganizationId != organizationId) return false;
        var exists = await _db.Deals.AnyAsync(d => d.Id == deal.Id && d.OrganizationId == organizationId);
        if (!exists) return false;

        _db.Deals.Update(deal);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDealAsync(int id, string organizationId)
    {
        var deal = await _db.Deals.FirstOrDefaultAsync(d => d.Id == id && d.OrganizationId == organizationId);
        if (deal == null) return false;
        _db.Deals.Remove(deal);
        await _db.SaveChangesAsync();
        return true;
    }
}