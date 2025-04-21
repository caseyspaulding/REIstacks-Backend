using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Infrastructure.Services.CRM;
public class ProductivityService : IProductivityService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public ProductivityService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<ProductivityDto> GetProductivityAsync(string organizationId, Guid profileId)
    {
        // 1) figure out your time window (last 7 days, starting on Monday, whatever you prefer)
        var utcNow = DateTime.UtcNow;
        var weekAgo = utcNow.AddDays(-7);

        // 2) pull counts from your tables
        var newLeads = await _db.Leads
            .Where(l => l.OrganizationId == organizationId
                     && l.CreatedAt >= weekAgo)
            .CountAsync();

        // “Contacted” = number of distinct leads that had a “Contact” activity logged?
        var leadsContacted = await _db.ContactActivities
            .Where(a => a.OrganizationId == organizationId
                     && a.Type == ActivityType.Contacted
                     && a.Timestamp >= weekAgo
                     && a.CreatedByProfileId == profileId
                   )
            .Select(a => a.ContactId)
            .Distinct()
            .CountAsync();

        // appointments =
        var appointments = await _db.ContactActivities
            .Where(a => a.OrganizationId == organizationId
                     && a.Type == ActivityType.Appointment
                     && a.Timestamp >= weekAgo
                     && a.CreatedByProfileId == profileId
                   )
            .CountAsync();

        // offers =
        var offers = await _db.Offers
            .Where(o => o.OrganizationId == organizationId
                     && o.CreatedAt >= weekAgo

                   )
            .CountAsync();

        // 3) targets: you can hard‑code, or read from config/appsettings, or from a table
        var dto = new ProductivityDto
        {
            NewLeadsThisWeek = newLeads,
            NewLeadsTarget = _config.GetValue<int>("Targets:NewLeadsThisWeek", 50),

            LeadsContacted = leadsContacted,
            LeadsContactedTarget = _config.GetValue<int>("Targets:LeadsContacted", 20),

            AppointmentsMade = appointments,
            AppointmentsTarget = _config.GetValue<int>("Targets:AppointmentsMade", 30),

            OffersMade = offers,
            OffersTarget = _config.GetValue<int>("Targets:OffersMade", 100),
        };

        return dto;
    }
}