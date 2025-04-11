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

        public CsvImportService(AppDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        /// <summary>
        /// Uploads a CSV file to blob storage and returns its Blob URL.
        /// </summary>
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string organizationId)
        {
            return await _storageService.UploadFileAsync(fileStream, fileName, organizationId);
        }

        /// <summary>
        /// Reads CSV headers and a few sample rows, then auto-suggests column mappings.
        /// </summary>
        public async Task<PreviewResult> GetPreviewAsync(Stream fileStream, int sampleRows = 5)
        {
            var preview = new PreviewResult();

            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // 1) Read headers
            csv.Read();
            csv.ReadHeader();
            preview.Headers = csv.HeaderRecord.ToList();

            // 2) Read sample rows
            int count = 0;
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

            // 3) Suggest field mappings for the user
            preview.SuggestedMappings = SuggestFieldMappings(preview.Headers);

            return preview;
        }

        /// <summary>
        /// Checks whether required columns exist in CSV headers.
        /// Throws an exception if any required column is missing.
        /// </summary>
        public void ValidateRequiredColumns(
            IReadOnlyList<string> csvHeaders,
            IReadOnlyList<string> requiredColumns)
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

        /// <summary>
        /// Suggests default mappings for CSV column headers to known contact fields.
        /// </summary>
        public Dictionary<string, string> SuggestFieldMappings(IReadOnlyList<string> csvHeaders)
        {
            var suggestions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // A list of known contact fields in your DB.
            var possibleFields = new List<string>
            {
                "FirstName", "LastName", "Email", "Phone", "AlternatePhone",
                "Company", "Title", "PreferredContactMethod",
                "StreetAddress", "City", "State", "ZipCode",
                "ContactType", "LeadSource", "Tags", "Notes",
                "ConsentTextMessages", "ConsentEmailMarketing"
            };

            foreach (var header in csvHeaders)
            {
                // Implement more sophisticated matching logic
                var bestMatch = MatchHeader(header, possibleFields);
                suggestions[header] = !string.IsNullOrEmpty(bestMatch)
                    ? bestMatch
                    : ""; // Leave unmapped fields blank
            }
            return suggestions;
        }

        /// <summary>
        /// Match a CSV header to the closest field in our contacts table
        /// </summary>
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
            if (normalizedHeader.Contains("source") || normalizedHeader.Contains("lead") && normalizedHeader.Contains("source"))
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

        /// <summary>
        /// Basic row-level validation. For example, require that at least one contact field is present.
        /// </summary>
        private bool ValidateRow(Contact contact, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(contact.Email) && string.IsNullOrWhiteSpace(contact.Phone))
            {
                errorMessage = "Row missing both email and phone number.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(contact.FirstName) && string.IsNullOrWhiteSpace(contact.LastName))
            {
                errorMessage = "Row missing both first name and last name.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Checks if the given contact is a duplicate by using a composite key (Email + Phone).
        /// </summary>
        private bool IsDuplicate(Contact contact, HashSet<string> seenKeys)
        {
            var key = $"{(contact.Email ?? "")}#{(contact.Phone ?? "")}";
            if (seenKeys.Contains(key))
            {
                return true;
            }

            seenKeys.Add(key);
            return false;
        }

        /// <summary>
        /// Downloads the CSV from storage, then processes it into the database using the provided field mappings.
        /// </summary>
        public async Task<ImportResult> ProcessFileFromStorageAsync(
            string blobUrl,
            Dictionary<string, string> fieldMappings,
            string organizationId)
        {
            using var fileStream = await _storageService.DownloadFileAsync(blobUrl);
            return await ProcessCsvFile(fileStream, fieldMappings, organizationId);
        }

        /// <summary>
        /// Main CSV import logic – reads CSV rows, applies mappings, and inserts contacts into DB.
        /// </summary>
        public async Task<ImportResult> ProcessCsvFile(
            Stream fileStream,
            Dictionary<string, string> fieldMappings,
            string organizationId)
        {
            var result = new ImportResult();
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Read header row
            csv.Read();
            csv.ReadHeader();

            var contacts = new List<Contact>();
            var duplicates = new HashSet<string>();
            var totalRows = 0;

            try
            {
                while (csv.Read())
                {
                    totalRows++;
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

                    // Optional row-level validation
                    if (!ValidateRow(contact, out string validationError))
                    {
                        result.Errors.Add($"Row {csv.Context.Parser.Row}: {validationError}");
                        continue; // skip adding to contacts
                    }

                    // Optional duplication check
                    if (IsDuplicate(contact, duplicates))
                    {
                        result.Errors.Add($"Row {csv.Context.Parser.Row}: Duplicate Email/Phone: {contact.Email}/{contact.Phone}");
                        continue; // skip adding to contacts
                    }

                    contacts.Add(contact);
                }

                await _context.Contacts.AddRangeAsync(contacts);
                await _context.SaveChangesAsync();

                result.TotalImported = contacts.Count;
                result.Success = true;
                result.TotalRows = totalRows;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"Database error: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Maps a single CSV column's value onto the correct field in the Contact entity.
        /// </summary>
        private void ApplyValueToContact(Contact contact, string field, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

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
                    if (bool.TryParse(value, out bool consentText))
                        contact.ConsentTextMessages = consentText;
                    break;
                case "ConsentEmailMarketing":
                    if (bool.TryParse(value, out bool consentEmail))
                        contact.ConsentEmailMarketing = consentEmail;
                    break;
            }
        }
    }
}