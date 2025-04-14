using CsvHelper;
using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Interfaces;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Application.Interfaces.Models;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Infrastructure.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace REIstacks.Infrastructure.Services.ListImport
{
    public class CsvImportService : ICsvImportService
    {
        private readonly AppDbContext _context;
        private readonly IStorageService _storageService;
        private readonly List<string> _possibleFields = new List<string>
        {
            "FirstName", "LastName", "Email", "Phone", "AlternatePhone",
            "Company", "Title", "PreferredContactMethod",
            "StreetAddress", "City", "State", "ZipCode",
            "ContactType", "LeadSource", "Tags", "Notes",
            "ConsentTextMessages", "ConsentEmailMarketing"
        };
        private const int BatchSize = 100;

        public CsvImportService(AppDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string organizationId)
        {
            return await _storageService.UploadFileAsync(fileStream, fileName, organizationId);
        }

        public async Task<PreviewResult> GetPreviewAsync(Stream fileStream, int sampleRows = 5)
        {
            var preview = new PreviewResult();

            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Read headers
            csv.Read();
            csv.ReadHeader();
            preview.Headers = csv.HeaderRecord?.ToList() ?? new List<string>();

            // Read sample rows
            int count = 0;
            preview.Rows = new List<Dictionary<string, string>>();
            while (csv.Read() && count < sampleRows)
            {
                var row = new Dictionary<string, string>();
                foreach (var header in preview.Headers)
                {
                    row[header] = csv.GetField(header) ?? string.Empty;
                }
                preview.Rows.Add(row);
                count++;
            }

            // Suggest field mappings
            preview.SuggestedMappings = SuggestFieldMappings(preview.Headers);

            return preview;
        }

        public void ValidateRequiredColumns(IReadOnlyList<string> csvHeaders, IReadOnlyList<string> requiredColumns)
        {
            var missing = requiredColumns
                .Where(rc => !csvHeaders.Contains(rc, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (missing.Any())
            {
                throw new InvalidOperationException(
                    "Missing required columns: " + string.Join(", ", missing));
            }
        }

        public Dictionary<string, string> SuggestFieldMappings(IReadOnlyList<string> csvHeaders)
        {
            var suggestions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var header in csvHeaders)
            {
                var bestMatch = MatchHeader(header, _possibleFields);
                suggestions[header] = !string.IsNullOrEmpty(bestMatch)
                    ? bestMatch
                    : ""; // Leave unmapped fields blank
            }
            return suggestions;
        }

        private string MatchHeader(string header, List<string> possibleFields)
        {
            // Direct match
            var directMatch = possibleFields.FirstOrDefault(f =>
                f.Equals(header, StringComparison.OrdinalIgnoreCase));

            if (directMatch != null)
                return directMatch;

            // Normalize header for better matching
            var normalizedHeader = header.ToLower().Replace(" ", "").Replace("_", "");

            // Check for common patterns
            if (normalizedHeader.Contains("first") && normalizedHeader.Contains("name"))
                return "FirstName";
            if (normalizedHeader.Contains("last") && normalizedHeader.Contains("name"))
                return "LastName";
            if (normalizedHeader.Contains("email"))
                return "Email";
            if (normalizedHeader.Contains("alt") && normalizedHeader.Contains("phone"))
                return "AlternatePhone";
            if (normalizedHeader.Contains("phone") || normalizedHeader.Contains("mobile") || normalizedHeader.Contains("cell"))
                return "Phone";
            if (normalizedHeader.Contains("company") || normalizedHeader.Contains("business"))
                return "Company";
            if (normalizedHeader.Contains("title") || normalizedHeader.Contains("position"))
                return "Title";
            if (normalizedHeader.Contains("preferred") && normalizedHeader.Contains("contact"))
                return "PreferredContactMethod";
            if (normalizedHeader.Contains("address") || normalizedHeader.Contains("street"))
                return "StreetAddress";
            if (normalizedHeader.Contains("city"))
                return "City";
            if (normalizedHeader.Contains("state") || normalizedHeader.Contains("province"))
                return "State";
            if (normalizedHeader.Contains("zip") || normalizedHeader.Contains("postal"))
                return "ZipCode";
            if (normalizedHeader.Contains("type") && normalizedHeader.Contains("contact"))
                return "ContactType";
            if (normalizedHeader.Contains("source") || (normalizedHeader.Contains("lead") && normalizedHeader.Contains("source")))
                return "LeadSource";
            if (normalizedHeader.Contains("tag"))
                return "Tags";
            if (normalizedHeader.Contains("note"))
                return "Notes";
            if ((normalizedHeader.Contains("consent") || normalizedHeader.Contains("opt")) && normalizedHeader.Contains("text"))
                return "ConsentTextMessages";
            if ((normalizedHeader.Contains("consent") || normalizedHeader.Contains("opt")) && normalizedHeader.Contains("email"))
                return "ConsentEmailMarketing";

            // No match found
            return "";
        }

        private bool ValidateRow(Contact contact, out string errorMessage, out string fieldName)
        {
            if (string.IsNullOrWhiteSpace(contact.Email) && string.IsNullOrWhiteSpace(contact.Phone))
            {
                errorMessage = "Missing both email and phone number";
                fieldName = "Email/Phone";
                return false;
            }

            if (string.IsNullOrWhiteSpace(contact.FirstName) && string.IsNullOrWhiteSpace(contact.LastName))
            {
                errorMessage = "Missing both first name and last name";
                fieldName = "FirstName/LastName";
                return false;
            }

            // Email format validation
            if (!string.IsNullOrWhiteSpace(contact.Email) && !IsValidEmail(contact.Email))
            {
                errorMessage = $"Invalid email format: {contact.Email}";
                fieldName = "Email";
                return false;
            }

            // Phone format validation
            if (!string.IsNullOrWhiteSpace(contact.Phone) && !IsValidPhone(contact.Phone))
            {
                errorMessage = $"Invalid phone format: {contact.Phone}";
                fieldName = "Phone";
                return false;
            }

            errorMessage = null;
            fieldName = null;
            return true;
        }

        private bool IsDuplicate(Contact contact, HashSet<string> seenKeys)
        {
            var key = $"{contact.Email ?? ""}#{contact.Phone ?? ""}";
            if (string.IsNullOrWhiteSpace(key.Replace("#", "")))
            {
                return false; // Can't determine duplication without email or phone
            }

            if (seenKeys.Contains(key))
            {
                return true;
            }

            seenKeys.Add(key);
            return false;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");
        }

        public async Task<ImportResult> ProcessFileFromStorageAsync(
            string blobUrl,
            Dictionary<string, string> fieldMappings,
            string organizationId)
        {
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

            // Read header row
            csv.Read();
            csv.ReadHeader();

            var duplicates = new HashSet<string>();
            var currentBatch = new List<Contact>(BatchSize);

            // Start a transaction for the entire import process
            using var transaction = await _context.Database.BeginTransactionAsync();

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

                    // Apply field mappings
                    foreach (var mapping in fieldMappings)
                    {
                        var csvColumn = mapping.Key;
                        var dbField = mapping.Value;

                        if (string.IsNullOrEmpty(dbField))
                            continue; // Skip unmapped columns

                        if (csv.TryGetField(csvColumn, out string value))
                        {
                            ApplyValueToContact(contact, dbField, value);
                        }
                    }

                    // Row-level validation
                    if (!ValidateRow(contact, out string validationError, out string fieldName))
                    {
                        result.Errors.Add($"Row {csv.Context.Parser.Row}: {validationError} (Field: {fieldName})");
                        continue; // Skip adding to contacts
                    }

                    // Duplication check
                    if (IsDuplicate(contact, duplicates))
                    {
                        result.Errors.Add($"Row {csv.Context.Parser.Row}: Duplicate Email/Phone: {contact.Email ?? "N/A"}/{contact.Phone ?? "N/A"} (Field: Email/Phone)");
                        continue; // Skip adding to contacts
                    }

                    currentBatch.Add(contact);

                    // Process batch if it reaches the batch size
                    if (currentBatch.Count >= BatchSize)
                    {
                        await ProcessBatch(currentBatch, result);
                        currentBatch.Clear();
                    }
                }

                // Process any remaining contacts
                if (currentBatch.Any())
                {
                    await ProcessBatch(currentBatch, result);
                }

                // Commit transaction if everything succeeded
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Rollback on error
                await transaction.RollbackAsync();
                result.Success = false;
                result.Errors.Add($"Database error: {ex.Message}");
            }

            return result;
        }

        private async Task ProcessBatch(List<Contact> batch, ImportResult result)
        {
            try
            {
                await _context.Contacts.AddRangeAsync(batch);
                await _context.SaveChangesAsync();
                result.TotalImported += batch.Count;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"Database error processing batch: {ex.Message}");
                // Re-throw to trigger transaction rollback
                throw;
            }
        }

        private void ApplyValueToContact(Contact contact, string field, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            // Trim whitespace from all values
            value = value.Trim();

            switch (field)
            {
                case "FirstName": contact.FirstName = value; break;
                case "LastName": contact.LastName = value; break;
                case "Email": contact.Email = value; break;
                case "Phone": contact.Phone = value; break;
                case "AlternatePhone": contact.AlternatePhone = value; break;
                case "Company": contact.Company = value; break;
                case "Title": contact.Title = value; break;
                case "PreferredContactMethod": contact.PreferredContactMethod = value; break;
                case "StreetAddress": contact.StreetAddress = value; break;
                case "City": contact.City = value; break;
                case "State": contact.State = value; break;
                case "ZipCode": contact.ZipCode = value; break;
                case "ContactType": contact.ContactType = value; break;
                case "LeadSource": contact.LeadSource = value; break;
                case "Tags": contact.Tags = value; break;
                case "Notes": contact.Notes = value; break;
                case "ConsentTextMessages":
                    bool.TryParse(value, out bool consentText);
                    contact.ConsentTextMessages = consentText;
                    break;
                case "ConsentEmailMarketing":
                    bool.TryParse(value, out bool consentEmail);
                    contact.ConsentEmailMarketing = consentEmail;
                    break;
            }
        }
    }
}