using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Infrastructure.Data;
using System.Net.Http.Json;

namespace REIstacks.Infrastructure.Services.CRM;
public class SkipTraceService : ISkipTraceService
{
    private readonly AppDbContext _db;
    private readonly HttpClient _http;
    private readonly string _apifyToken;

    public SkipTraceService(
           AppDbContext db,
           HttpClient http,
           IConfiguration config)      // ← new
    {
        _db = db;
        _http = http;
        // read from config (falls back to environment variable APIFY_TOKEN)
        _apifyToken = config["APIFY_TOKEN"]
                     ?? throw new InvalidOperationException("APIFY_TOKEN is not configured");
    }

    public async Task<SkipTraceActivity> StartSkipTraceAsync(IEnumerable<int> contactIds, string organizationId)
    {
        // 1) create a new activity
        var activity = new SkipTraceActivity
        {
            OrganizationId = organizationId,
            Status = SkipTraceStatus.Pending,
            CompletedAt = DateTime.UtcNow
        };
        _db.SkipTraceActivities.Add(activity);
        await _db.SaveChangesAsync();

        // 2) lookup the contacts, build the Apify payload
        var contacts = await _db.Contacts
            .Where(c => contactIds.Contains(c.Id) && c.OrganizationId == organizationId)
            .Select(c => new { c.FirstName, c.LastName, c.StreetAddress, c.City, c.State, c.ZipCode })
            .ToListAsync();

        var apifyInput = new
        {
            /* transform each contact into the input Apify expects */
            data = contacts.Select(c => new
            {
                name = $"{c.FirstName} {c.LastName}",
                address = new
                {
                    streetAddress = c.StreetAddress,
                    addressLocality = c.City,
                    addressRegion = c.State,
                    postalCode = c.ZipCode
                }
            })
        };

        // 3) fire off the Apify actor asynchronously:
        var response = await _http.PostAsJsonAsync(
            $"https://api.apify.com/v2/acts/sorower~skip-trace/run-sync-get-dataset-items?token={_apifyToken}",
            apifyInput);

        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<SkipTraceItem>>();

        // 4) write everything back into your database
        activity.Status = SkipTraceStatus.Completed;
        activity.RawResponseJson = await response.Content.ReadAsStringAsync();
        activity.CompletedAt = DateTime.UtcNow;
        activity.ProcessedCount = items?.Count ?? 0;

        // breakdown by category
        var breakdowns = items
           .GroupBy(i => i.Category)
           .Select(g => new SkipTraceBreakdown
           {
               SkipTraceActivityId = activity.Id,
               Category = g.Key,
               Count = g.Count()
           });

        await _db.SkipTraceBreakdowns.AddRangeAsync(breakdowns);
        await _db.SkipTraceItems.AddRangeAsync(items.Select(i =>
        {
            i.SkipTraceActivityId = activity.Id;
            return i;
        }));

        await _db.SaveChangesAsync();
        return activity;
    }

    public async Task<IEnumerable<SkipTraceActivity>> GetActivitiesAsync(string organizationId) =>
        await _db.SkipTraceActivities
            .Where(a => a.OrganizationId == organizationId)
            .ToListAsync();

    public async Task<SkipTraceActivity?> GetActivityByIdAsync(int id, string organizationId) =>
        await _db.SkipTraceActivities
            .Include(a => a.Breakdown)
            .Include(a => a.Items)
            .FirstOrDefaultAsync(a => a.Id == id && a.OrganizationId == organizationId);

    public async Task<bool> CancelActivityAsync(int id, string organizationId)
    {
        var activity = await _db.SkipTraceActivities
            .FirstOrDefaultAsync(a => a.Id == id && a.OrganizationId == organizationId);
        if (activity == null) return false;
        activity.Status = SkipTraceStatus.Cancelled;
        await _db.SaveChangesAsync();
        return true;
    }
}