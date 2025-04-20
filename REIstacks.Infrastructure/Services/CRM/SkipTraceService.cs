// REIstacks.Infrastructure.Services.CRM/SkipTraceService.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Infrastructure.Data;
using System.Net.Http.Json;

namespace REIstacks.Infrastructure.Services.CRM
{
    public class SkipTraceService : ISkipTraceService
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _http;
        private readonly string _apifyToken;

        public SkipTraceService(
            AppDbContext db,
            HttpClient http,
            IConfiguration config)
        {
            _db = db;
            _http = http;
            _apifyToken = config["APIFY_TOKEN"]
                       ?? throw new InvalidOperationException("APIFY_TOKEN is not configured");
        }

        public async Task<IEnumerable<AddressCsvDto>> GetAddressesByContactIdsAsync(
            IEnumerable<int> contactIds,
            string organizationId)
        {
            // pull out only the fields needed by your AddressCsvDto
            return await _db.Contacts
                .Where(c => contactIds.Contains(c.Id)
                         && c.OrganizationId == organizationId)
                .Select(c => new AddressCsvDto
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    StreetAddress = c.StreetAddress,
                    City = c.City,
                    State = c.State,
                    ZipCode = c.ZipCode
                })
                .ToListAsync();
        }

        public async Task<SkipTraceActivity> StartSkipTraceByAddressesAsync(
            IEnumerable<AddressCsvDto> addresses,
            string organizationId)
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

            // 2) build Apify payload
            var apifyInput = new
            {
                data = addresses.Select(a => new
                {
                    name = $"{a.FirstName} {a.LastName}",
                    address = new
                    {
                        streetAddress = a.StreetAddress,
                        addressLocality = a.City,
                        addressRegion = a.State,
                        postalCode = a.ZipCode
                    }
                })
            };

            // 3) call Apify
            var url = $"https://api.apify.com/v2/acts/sorower~skip-trace/"
                    + $"run-sync-get-dataset-items?token={_apifyToken}";
            var response = await _http.PostAsJsonAsync(url, apifyInput);
            response.EnsureSuccessStatusCode();

            // 4) read raw JSON + parse flat items
            var rawJson = await response.Content.ReadAsStringAsync();
            var items = await response.Content
                                     .ReadFromJsonAsync<List<SkipTraceItem>>();

            // 5) persist results
            activity.RawResponseJson = rawJson;
            activity.CompletedAt = DateTime.UtcNow;
            activity.ProcessedCount = items?.Count ?? 0;
            activity.Status = SkipTraceStatus.Completed;

            // if you need breakdowns by a property on SkipTraceItem, do it here:
            // e.g. group by i.SomeCategory and save SkipTraceBreakdown

            await _db.SaveChangesAsync();
            return activity;
        }

        public async Task<SkipTraceActivity> StartSkipTraceAsync(
            IEnumerable<int> contactIds,
            string organizationId)
        {
            // optional: you can now simply delegate to the above two methods:
            var rows = await GetAddressesByContactIdsAsync(contactIds, organizationId);
            var activity = await StartSkipTraceByAddressesAsync(rows, organizationId);
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
                .FirstOrDefaultAsync(a => a.Id == id
                                       && a.OrganizationId == organizationId);

        public async Task<bool> CancelActivityAsync(int id, string organizationId)
        {
            var activity = await _db.SkipTraceActivities
                .FirstOrDefaultAsync(a => a.Id == id
                                       && a.OrganizationId == organizationId);
            if (activity == null) return false;

            activity.Status = SkipTraceStatus.Cancelled;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
