using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Infrastructure.Data;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace REIstacks.Infrastructure.Services.CRM
{
    public class SkipTraceService : ISkipTraceService
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _http;
        private readonly string _apifyToken;
        private readonly ILogger<SkipTraceService> _logger;
        private const decimal BASE_COST = 10.00m;
        private const decimal COST_PER_THOUSAND = 10.00m;

        public SkipTraceService(
            AppDbContext db,
            HttpClient http,
            IConfiguration config,
            ILogger<SkipTraceService> logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _apifyToken = config["APIFY_TOKEN"]
                ?? throw new InvalidOperationException("APIFY_TOKEN is not configured");

            // Set default headers for all requests
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apifyToken);
        }

        public async Task<IEnumerable<AddressCsvDto>> GetAddressesByContactIdsAsync(
            IEnumerable<int> contactIds,
            string organizationId)
        {
            try
            {
                _logger.LogInformation("Retrieving addresses for {Count} contacts in organization {OrgId}",
                    contactIds.Count(), organizationId);

                return await _db.Contacts
                    .Where(c => contactIds.Contains(c.Id) &&
                             c.OrganizationId == organizationId)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve addresses for contacts in organization {OrgId}", organizationId);
                throw;
            }
        }

        public async Task<SkipTraceActivity> StartSkipTraceByAddressesAsync(
          IEnumerable<AddressCsvDto> addresses,
          string organizationId)
        {
            var allAddressList = addresses.ToList();
            var totalRowCount = allAddressList.Count;

            // Filter out invalid addresses
            var validAddressList = FilterValidAddresses(allAddressList).ToList();
            var validAddressCount = validAddressList.Count;

            _logger.LogInformation("Starting skip trace for {ValidCount} valid addresses out of {TotalCount} total rows in organization {OrgId}",
                validAddressCount, totalRowCount, organizationId);

            // Create activity record with both counts
            var activity = await CreateInitialActivityAsync(validAddressCount, organizationId);
            activity.TotalRowCount = totalRowCount;

            try
            {
                // Update to InProgress now that we've started
                activity.Status = SkipTraceStatus.InProgress;
                await _db.SaveChangesAsync();

                // Check if we have any valid addresses to process
                if (validAddressCount == 0)
                {
                    activity.Status = SkipTraceStatus.Completed;
                    activity.CompletedAt = DateTime.UtcNow;
                    activity.ErrorMessage = "No valid addresses found in input file";
                    await _db.SaveChangesAsync();
                    return activity;
                }

                // Call Apify service - using validAddressList instead of addressList
                var rawJson = await CallApifyServiceAsync(validAddressList);
                var items = ProcessApifyResults(rawJson, activity.Id, validAddressList);

                // Use validAddressCount instead of addressCount
                return await SaveActivityResultsAsync(activity, items, validAddressCount, rawJson);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "API request failed for skip trace activity {ActivityId}: {Message}",
                    activity.Id, ex.Message);

                // Use validAddressCount instead of addressCount
                return await MarkActivityAsFailedAsync(activity, validAddressCount,
                    $"API request failed: {ex.Message}");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse API response for skip trace activity {ActivityId}: {Message}",
                    activity.Id, ex.Message);

                // Use validAddressCount instead of addressCount
                return await MarkActivityAsFailedAsync(activity, validAddressCount,
                    $"Failed to parse API response: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in skip trace activity {ActivityId}: {Message}",
                    activity.Id, ex.Message);

                // Use validAddressCount instead of addressCount
                return await MarkActivityAsFailedAsync(activity, validAddressCount,
                    $"Unexpected error: {ex.Message}");
            }
        }

        private async Task<SkipTraceActivity> CreateInitialActivityAsync(int addressCount, string organizationId)
        {
            var activity = new SkipTraceActivity
            {
                OrganizationId = organizationId,
                Status = SkipTraceStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Total = addressCount,
                Pending = addressCount,
                Matched = 0,
                Failed = 0,
                ErrorMessage = string.Empty
            };

            _db.SkipTraceActivities.Add(activity);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created skip trace activity {ActivityId} with {Count} addresses",
                activity.Id, addressCount);

            return activity;
        }

        private async Task<string> CallApifyServiceAsync(
            List<AddressCsvDto> addressList)
        {
            // Initialize arrays for different search types
            var nameSearches = new List<string>();
            var addressSearches = new List<string>();
            var phoneSearches = new List<string>();

            // Fill arrays based on available data
            foreach (var addr in addressList)
            {
                // Name searches for both primary and secondary owners
                if (!string.IsNullOrWhiteSpace(addr.FirstName) && !string.IsNullOrWhiteSpace(addr.LastName))
                {
                    string fullName = $"{addr.FirstName} {addr.LastName}".Trim();
                    if (!string.IsNullOrWhiteSpace(fullName))
                    {
                        // Add name + location for better matching
                        if (!string.IsNullOrWhiteSpace(addr.City) && !string.IsNullOrWhiteSpace(addr.State))
                        {
                            nameSearches.Add($"{fullName}; {addr.City}, {addr.State} {addr.ZipCode}".Trim());
                        }
                        else
                        {
                            nameSearches.Add(fullName);
                        }
                    }
                }

                // Secondary owner searches
                if (!string.IsNullOrWhiteSpace(addr.Owner2FirstName) && !string.IsNullOrWhiteSpace(addr.Owner2LastName))
                {
                    string fullName = $"{addr.Owner2FirstName} {addr.Owner2LastName}".Trim();
                    if (!string.IsNullOrWhiteSpace(fullName))
                    {
                        // Add name + location for better matching
                        if (!string.IsNullOrWhiteSpace(addr.City) && !string.IsNullOrWhiteSpace(addr.State))
                        {
                            nameSearches.Add($"{fullName}; {addr.City}, {addr.State} {addr.ZipCode}".Trim());
                        }
                        else
                        {
                            nameSearches.Add(fullName);
                        }
                    }
                }

                // Property address search
                if (!string.IsNullOrWhiteSpace(addr.StreetAddress) &&
                    !string.IsNullOrWhiteSpace(addr.City) &&
                    !string.IsNullOrWhiteSpace(addr.State))
                {
                    string unit = !string.IsNullOrWhiteSpace(addr.Unit) ? $" {addr.Unit}" : "";
                    string fullAddress = $"{addr.StreetAddress}{unit}; {addr.City}, {addr.State} {addr.ZipCode}".Trim();
                    addressSearches.Add(fullAddress);
                }

                // Mailing address search (if different from property address)
                if (!string.IsNullOrWhiteSpace(addr.MailingAddress) &&
                    !string.IsNullOrWhiteSpace(addr.MailingCity) &&
                    !string.IsNullOrWhiteSpace(addr.MailingState))
                {
                    string unit = !string.IsNullOrWhiteSpace(addr.MailingUnit) ? $" {addr.MailingUnit}" : "";
                    string fullAddress = $"{addr.MailingAddress}{unit}; {addr.MailingCity}, {addr.MailingState} {addr.MailingZip}".Trim();

                    // Only add if it's different from property address
                    if (!addressSearches.Contains(fullAddress))
                    {
                        addressSearches.Add(fullAddress);
                    }
                }

                // Add all available phone numbers
                AddPhoneIfValid(addr.Phone1, phoneSearches);
                AddPhoneIfValid(addr.Phone2, phoneSearches);
                AddPhoneIfValid(addr.Phone3, phoneSearches);
                AddPhoneIfValid(addr.Phone4, phoneSearches);
                AddPhoneIfValid(addr.Phone5, phoneSearches);
                AddPhoneIfValid(addr.Phone6, phoneSearches);
                AddPhoneIfValid(addr.Phone7, phoneSearches);
                AddPhoneIfValid(addr.Phone8, phoneSearches);
                AddPhoneIfValid(addr.Phone9, phoneSearches);
                AddPhoneIfValid(addr.Phone10, phoneSearches);
            }

            // Build Apify payload in the format they expect
            var apifyInput = new
            {
                name = nameSearches.Distinct().ToList(),
                street_citystatezip = addressSearches.Distinct().ToList(),
                phone_number = phoneSearches.Distinct().ToList(),
                max_results = 1 // Limit to 1 result per search to control costs
            };

            // Call Apify API
            var url = "https://api.apify.com/v2/acts/sorower~skip-trace/run-sync-get-dataset-items";
            _logger.LogInformation("Calling Apify skip trace API with {NameCount} name searches, " +
                                  "{AddressCount} address searches, {PhoneCount} phone searches",
                nameSearches.Count, addressSearches.Count, phoneSearches.Count);

            var response = await _http.PostAsJsonAsync(url, apifyInput);
            response.EnsureSuccessStatusCode();

            // Read response data
            var rawJson = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Received skip‑trace payload ({Length} chars)", rawJson.Length);
            return rawJson;
        }
        private void AddPhoneIfValid(string phone, List<string> phoneSearches)
        {
            if (!string.IsNullOrWhiteSpace(phone) && phone.Count(c => char.IsDigit(c)) >= 10)
            {
                phoneSearches.Add(phone);
            }
        }
        private List<SkipTraceItem> ProcessApifyResults(
    string rawJson,
    int activityId,
    IEnumerable<AddressCsvDto> originalAddresses)
        {
            var items = new List<SkipTraceItem>();
            var addressMap = BuildAddressLookupMap(originalAddresses);
            using var doc = JsonDocument.Parse(rawJson);

            JsonElement array = doc.RootElement.ValueKind == JsonValueKind.Array
                ? doc.RootElement
                : doc.RootElement.GetProperty("results");

            foreach (var elem in array.EnumerateArray())
            {
                try
                {
                    items.Add(CreateSkipTraceItemFromResponse(elem, activityId, originalAddresses, addressMap));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing item: {Message}", ex.Message);
                    items.Add(new SkipTraceItem
                    {
                        SkipTraceActivityId = activityId,
                        Status = "Failed",
                        RawResponseJson = elem.GetRawText()
                    });
                }
            }
            return items;
        }

        private Dictionary<string, AddressCsvDto> BuildAddressLookupMap(
            IEnumerable<AddressCsvDto> originalAddresses)
        {
            return originalAddresses.ToDictionary(
                addr => $"{addr.FirstName} {addr.LastName}|{addr.StreetAddress}|{addr.City}|{addr.State}|{addr.ZipCode}"
                    .ToLowerInvariant(),
                addr => addr
            );
        }
        private AddressCsvDto FindMatchingOriginal(
            JsonElement je,
            string searchOption,
            IEnumerable<AddressCsvDto> originals,
            Dictionary<string, AddressCsvDto> map)
        {
            if (searchOption.Contains("Address", StringComparison.OrdinalIgnoreCase)
                && je.TryGetProperty("Street Address", out var sa))
            {
                var key = sa.GetString()?.ToLowerInvariant();
                if (key != null && map.TryGetValue(key, out var found))
                    return found;
            }
            return originals.First(); // fallback
        }
        private SkipTraceItem CreateSkipTraceItemFromResponse(
    JsonElement elem,
    int activityId,
    IEnumerable<AddressCsvDto> originals,
    Dictionary<string, AddressCsvDto> map)
        {
            var item = new SkipTraceItem
            {
                SkipTraceActivityId = activityId,
                RawResponseJson = elem.GetRawText(),
                Status = "Completed"
            };

            // helper to pull a string or "" if missing
            string G(string name)
              => elem.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.String
                 ? p.GetString()! : "";

            item.FirstName = G("First Name");
            item.LastName = G("Last Name");
            item.City = G("Address Locality");
            item.State = G("Address Region");
            item.ZipCode = G("Postal Code");

            // nested "addresses" array → first "street"
            if (elem.TryGetProperty("addresses", out var addrs) && addrs.ValueKind == JsonValueKind.Array)
            {
                foreach (var a in addrs.EnumerateArray())
                    if (a.TryGetProperty("street", out var sp) && sp.ValueKind == JsonValueKind.String)
                    {
                        item.StreetAddress = sp.GetString()!;
                        break;
                    }
            }

            // fallback flat field
            if (string.IsNullOrWhiteSpace(item.StreetAddress))
                item.StreetAddress = G("Street Address");

            // last resort: fall back to your original CSV
            var key = $"{item.FirstName} {item.LastName}|{item.StreetAddress}|{item.City}|{item.State}|{item.ZipCode}"
                      .ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(item.StreetAddress) && map.TryGetValue(key, out var orig))
                item.StreetAddress = orig.StreetAddress;

            DetermineMatchStatus(item);
            return item;
        }

        private AddressCsvDto? FindMatchingOriginalAddress(
            JsonElement responseItem,
            string searchOption,
            IEnumerable<AddressCsvDto> originalAddresses,
            Dictionary<string, AddressCsvDto> addressMap)
        {
            // name‐based searches
            if (searchOption.Contains("Name", StringComparison.OrdinalIgnoreCase))
            {
                var firstName = responseItem.TryGetProperty("First Name", out var fProp)
                    && fProp.ValueKind == JsonValueKind.String
                        ? fProp.GetString()!
                        : "";
                var lastName = responseItem.TryGetProperty("Last Name", out var lProp)
                    && lProp.ValueKind == JsonValueKind.String
                        ? lProp.GetString()!
                        : "";

                return originalAddresses.FirstOrDefault(a =>
                    a.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase)
                    && a.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
            }
            // address‐based searches
            else if (searchOption.Contains("Address", StringComparison.OrdinalIgnoreCase))
            {
                var street = responseItem.TryGetProperty("Street Address", out var sProp)
                    && sProp.ValueKind == JsonValueKind.String
                        ? sProp.GetString()!
                        : "";
                var city = responseItem.TryGetProperty("Address Locality", out var cProp)
                    && cProp.ValueKind == JsonValueKind.String
                        ? cProp.GetString()!
                        : "";
                var region = responseItem.TryGetProperty("Address Region", out var rProp)
                    && rProp.ValueKind == JsonValueKind.String
                        ? rProp.GetString()!
                        : "";
                var postal = responseItem.TryGetProperty("Postal Code", out var zProp)
                    && zProp.ValueKind == JsonValueKind.String
                        ? zProp.GetString()!
                        : "";

                var key = $"|{street}|{city}|{region}|{postal}".ToLowerInvariant();
                return addressMap
                    .Where(kv => kv.Key.Contains(key))
                    .Select(kv => kv.Value)
                    .FirstOrDefault();
            }

            return null;
        }


        private void DetermineMatchStatus(SkipTraceItem item)
        {
            // Set match status based on available data
            if (!string.IsNullOrEmpty(item.PhoneNumber) && !string.IsNullOrEmpty(item.Email))
            {
                item.MatchStatus = "Matched"; // Both phone and email found
            }
            else if (!string.IsNullOrEmpty(item.PhoneNumber) || !string.IsNullOrEmpty(item.Email))
            {
                item.MatchStatus = "Matched"; // Either phone or email found
            }
            else if (!string.IsNullOrEmpty(item.StreetAddress) || !string.IsNullOrEmpty(item.Age))
            {
                item.MatchStatus = "PartialMatch"; // Got address or age but no contact info
            }
            else
            {
                item.MatchStatus = "NoMatch"; // No useful data found
            }
        }


        private async Task<SkipTraceActivity> SaveActivityResultsAsync(
            SkipTraceActivity activity,
            List<SkipTraceItem> items,
            int addressCount,
            string rawJson)
        {
            _db.SkipTraceItems.AddRange(items);

            // Update activity with results
            activity.RawResponseJson = rawJson;
            activity.CompletedAt = DateTime.UtcNow;
            activity.ProcessedCount = items?.Count ?? 0;

            // Calculate result statistics
            if (items != null && items.Count > 0)
            {
                activity.Matched = items.Count(i => i.MatchStatus == "Matched");
                activity.Failed = addressCount - activity.Matched;
            }
            else
            {
                activity.Failed = addressCount;
                activity.Matched = 0;
            }

            // No more pending items
            activity.Pending = 0;
            activity.Status = SkipTraceStatus.Completed;

            // Calculate cost
            activity.Cost = CalculateCost(addressCount);

            await _db.SaveChangesAsync();
            _logger.LogInformation("Skip trace activity {ActivityId} completed with {MatchCount} matches " +
                                  "and {FailCount} failures",
                activity.Id, activity.Matched, activity.Failed);

            return activity;
        }

        private async Task<SkipTraceActivity> MarkActivityAsFailedAsync(
            SkipTraceActivity activity,
            int addressCount,
            string errorMessage)
        {
            activity.Status = SkipTraceStatus.Failed;
            activity.CompletedAt = DateTime.UtcNow;
            activity.Failed = addressCount;
            activity.Pending = 0;
            // Since we don't have ErrorMessage property in SkipTraceActivity,
            // log the error but don't try to save it to the entity

            await _db.SaveChangesAsync();
            _logger.LogWarning("Skip trace activity {ActivityId} marked as failed: {ErrorMessage}",
                activity.Id, errorMessage);

            return activity;
        }

        private decimal CalculateCost(int addressCount)
        {
            return BASE_COST + (COST_PER_THOUSAND * (addressCount / 1000.0m));
        }

        public async Task<SkipTraceActivity> StartSkipTraceAsync(
            IEnumerable<int> contactIds,
            string organizationId)
        {
            try
            {
                _logger.LogInformation("Starting skip trace for {Count} contacts in organization {OrgId}",
                    contactIds.Count(), organizationId);

                var rows = await GetAddressesByContactIdsAsync(contactIds, organizationId);
                var activity = await StartSkipTraceByAddressesAsync(rows, organizationId);
                return activity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start skip trace for contacts in organization {OrgId}",
                    organizationId);
                throw;
            }
        }

        public async Task<IEnumerable<SkipTraceActivity>> GetActivitiesAsync(string organizationId)
        {
            try
            {
                _logger.LogInformation("Retrieving skip trace activities for organization {OrgId}",
                    organizationId);

                return await _db.SkipTraceActivities
                    .Where(a => a.OrganizationId == organizationId)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve skip trace activities for organization {OrgId}",
                    organizationId);
                throw;
            }
        }

        public async Task<SkipTraceActivity?> GetActivityByIdAsync(int id, string organizationId)
        {
            try
            {
                _logger.LogInformation("Retrieving skip trace activity {ActivityId} for organization {OrgId}",
                    id, organizationId);

                return await _db.SkipTraceActivities
                    .Include(a => a.Breakdown)
                    .Include(a => a.Items)
                    .FirstOrDefaultAsync(a => a.Id == id && a.OrganizationId == organizationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve skip trace activity {ActivityId} for organization {OrgId}",
                    id, organizationId);
                throw;
            }
        }

        private IEnumerable<AddressCsvDto> FilterValidAddresses(IEnumerable<AddressCsvDto> addresses)
        {
            return addresses.Where(a =>
                // Require at least some identifying information (name or address)
                (!string.IsNullOrWhiteSpace(a.FirstName) || !string.IsNullOrWhiteSpace(a.LastName)) &&
                (!string.IsNullOrWhiteSpace(a.StreetAddress) ||
                 (!string.IsNullOrWhiteSpace(a.City) && !string.IsNullOrWhiteSpace(a.State)))
            );
        }

        public async Task<bool> CancelActivityAsync(int id, string organizationId)
        {
            try
            {
                _logger.LogInformation("Attempting to cancel skip trace activity {ActivityId} for organization {OrgId}",
                    id, organizationId);

                var activity = await _db.SkipTraceActivities
                    .FirstOrDefaultAsync(a => a.Id == id && a.OrganizationId == organizationId);

                if (activity == null)
                {
                    _logger.LogWarning("Skip trace activity {ActivityId} not found for organization {OrgId}",
                        id, organizationId);
                    return false;
                }

                activity.Status = SkipTraceStatus.Cancelled;
                await _db.SaveChangesAsync();

                _logger.LogInformation("Skip trace activity {ActivityId} successfully cancelled", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel skip trace activity {ActivityId} for organization {OrgId}",
                    id, organizationId);
                throw;
            }
        }
    }


}