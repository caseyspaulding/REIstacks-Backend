using CsvHelper;
using REIstacks.Application.Interfaces;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Application.Interfaces.Models;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Infrastructure.Data;
using System.Globalization;

namespace REIstacks.Infrastructure.Services.ListImport
{
    public class CsvImportService : ICsvImportService
    {
        private readonly AppDbContext _context;
        private readonly IStorageService _storageService;
        private const int BatchSize = 100;

        public CsvImportService(AppDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public Task<string> UploadFileAsync(Stream fileStream, string fileName, string organizationId)
            => _storageService.UploadFileAsync(fileStream, fileName, organizationId);

        public async Task<PreviewResult> GetPreviewAsync(Stream fileStream, int sampleRows = 5)
        {
            var preview = new PreviewResult();

            // Use synchronous 'using' here
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();
            preview.Headers = csv.HeaderRecord?.ToList() ?? new List<string>();

            preview.Rows = new List<Dictionary<string, string>>();
            for (int i = 0; i < sampleRows && csv.Read(); i++)
            {
                var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var header in preview.Headers)
                {
                    row[header] = csv.GetField(header) ?? string.Empty;
                }
                preview.Rows.Add(row);
            }

            preview.SuggestedMappings = SuggestFieldMappings(preview.Headers);
            return preview;
        }

        // Stubbed to satisfy the interface
        public void ValidateRequiredColumns(
            IReadOnlyList<string> csvHeaders,
            IReadOnlyList<string> requiredColumns)
        {
            // no-op
        }

        public async Task<ImportResult> ProcessFileFromStorageAsync(
            string blobUrl,
            Dictionary<string, string> fieldMappings,
            string organizationId)
        {
            // DownloadFileAsync returns a Stream (IDisposable), so use 'using'
            using var fileStream = await _storageService.DownloadFileAsync(blobUrl);
            return await ProcessCsvFile(fileStream, fieldMappings, organizationId);
        }

        public async Task<ImportResult> ProcessCsvFile(
            Stream fileStream,
            Dictionary<string, string> fieldMappings,
            string organizationId)
        {
            var result = new ImportResult
            {
                Success = true,
                TotalImported = 0,
                TotalRows = 0,
                Errors = new List<string>()
            };

            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();

            var batch = new List<Contact>(BatchSize);
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                while (csv.Read())
                {
                    result.TotalRows++;

                    var contact = new Contact
                    {
                        OrganizationId = organizationId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        LeadSource = "CSV Import",
                        IsActive = true
                    };

                    foreach (var kvp in fieldMappings)
                    {
                        if (string.IsNullOrEmpty(kvp.Value)) continue;
                        if (csv.TryGetField(kvp.Key, out string val))
                            ApplyValueToContact(contact, kvp.Value, val);
                    }

                    batch.Add(contact);
                    if (batch.Count >= BatchSize)
                    {
                        await _context.Contacts.AddRangeAsync(batch);
                        await _context.SaveChangesAsync();
                        result.TotalImported += batch.Count;
                        batch.Clear();
                    }
                }

                if (batch.Any())
                {
                    await _context.Contacts.AddRangeAsync(batch);
                    await _context.SaveChangesAsync();
                    result.TotalImported += batch.Count;
                }

                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                result.Success = false;
                result.Errors.Add($"Import failed: {ex.Message}");
            }

            return result;
        }

        public Dictionary<string, string> SuggestFieldMappings(IReadOnlyList<string> csvHeaders)
        {
            var suggestions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in csvHeaders)
                suggestions[header] = MatchHeader(header);
            return suggestions;
        }

        private string MatchHeader(string header)
        {
            var n = header.ToLowerInvariant().Replace(" ", "").Replace("_", "");
            if (n.Contains("first") && n.Contains("name")) return "FirstName";
            if (n.Contains("last") && n.Contains("name")) return "LastName";
            if (n.Contains("email")) return "Email";
            if (n.Contains("phone")) return "Phone";
            if (n.Contains("company")) return "Company";
            if (n.Contains("title")) return "Title";
            if (n.Contains("city")) return "City";
            if (n.Contains("state")) return "State";
            if (n.Contains("zip")) return "ZipCode";
            return string.Empty;
        }

        private void ApplyValueToContact(Contact c, string field, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            value = value.Trim();

            switch (field)
            {
                case "FirstName": c.FirstName = value; break;
                case "LastName": c.LastName = value; break;
                case "Email": c.Email = value; break;
                case "Phone": c.Phone = value; break;
                case "Company": c.Company = value; break;
                case "Title": c.Title = value; break;
                case "StreetAddress": c.StreetAddress = value; break;
                case "City": c.City = value; break;
                case "State": c.State = value; break;
                case "ZipCode": c.ZipCode = value; break;
                case "ContactType": c.ContactType = value; break;
                case "LeadSource": c.LeadSource = value; break;
                case "Tags": c.Tags = value; break;
                case "Notes": c.Notes = value; break;
                case "ConsentTextMessages":
                    if (bool.TryParse(value, out var t)) c.ConsentTextMessages = t;
                    break;
                case "ConsentEmailMarketing":
                    if (bool.TryParse(value, out var e)) c.ConsentEmailMarketing = e;
                    break;
            }
        }
    }
}
