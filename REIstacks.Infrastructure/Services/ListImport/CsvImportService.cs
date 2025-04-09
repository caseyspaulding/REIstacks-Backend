using CsvHelper;
using REIstack.Domain.Models;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Infrastructure.Data;
using System.Globalization;

namespace REIstacks.Infrastructure.Services.ListImport;

public class CsvImportService
{
    private readonly AppDbContext _context;
    private readonly IStorageService _storageService;

    public CsvImportService(AppDbContext context, IStorageService storageService)
    {
        _context = context;
        _storageService = storageService;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string organizationId)
    {
        // Upload the file to blob storage first
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
        preview.Headers = csv.HeaderRecord.ToList();

        // Read sample rows
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

        return preview;
    }

    public async Task<ImportResult> ProcessFileFromStorageAsync(string blobUrl, Dictionary<string, string> fieldMappings, string organizationId)
    {
        // Download the file from storage
        using var fileStream = await _storageService.DownloadFileAsync(blobUrl);
        return await ProcessCsvFile(fileStream, fieldMappings, organizationId);
    }

    public async Task<ImportResult> ProcessCsvFile(Stream fileStream, Dictionary<string, string> fieldMappings, string organizationId)
    {
        var result = new ImportResult();

        using var reader = new StreamReader(fileStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Read header row
        csv.Read();
        csv.ReadHeader();

        // Process each row
        var leads = new List<Lead>();

        while (csv.Read())
        {
            var lead = new Lead
            {
                OrganizationId = organizationId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LeadSource = "CSV Import" // Track the source
            };

            // Apply field mappings
            foreach (var mapping in fieldMappings)
            {
                var csvColumn = mapping.Key;
                var dbField = mapping.Value;

                if (csv.TryGetField(csvColumn, out string value))
                {
                    ApplyValueToLead(lead, dbField, value);
                }
            }

            leads.Add(lead);
        }

        try
        {
            // Save to database
            await _context.Leads.AddRangeAsync(leads);
            await _context.SaveChangesAsync();

            result.TotalImported = leads.Count;
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Database error: {ex.Message}");
        }

        return result;
    }

    private void ApplyValueToLead(Lead lead, string field, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        switch (field)
        {
            case "FirstName": lead.FirstName = value; break;
            case "LastName": lead.LastName = value; break;
            case "PropertyStreetAddress": lead.PropertyStreetAddress = value; break;
            case "PropertyCity": lead.PropertyCity = value; break;
            case "PropertyState": lead.PropertyState = value; break;
            case "PropertyZipCode": lead.PropertyZipCode = value; break;
            case "PropertyCounty": lead.County = value; break;
            case "Email": lead.Email = value; break;
            case "Phone": lead.Phone = value; break;
            case "SquareFootage":
                if (int.TryParse(value, out int sqft))
                    lead.SquareFootage = sqft;
                break;
            case "Bedrooms":
                if (int.TryParse(value, out int beds))
                    lead.Bedrooms = beds;
                break;
            case "Bathrooms":
                if (int.TryParse(value, out int baths))
                    lead.Bathrooms = baths;
                break;
            case "YearBuilt":
                if (int.TryParse(value, out int year))
                    lead.YearBuilt = year;
                break;
            case "EstimatedValue":
                if (decimal.TryParse(value, out decimal estValue))
                    lead.EstimatedValue = estValue;
                break;
                // Add mappings for all other fields
        }
    }
}

public class ImportResult
{
    public int TotalImported { get; set; }
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}

public class PreviewResult
{
    public List<string> Headers { get; set; } = new List<string>();
    public List<Dictionary<string, string>> Rows { get; set; } = new List<Dictionary<string, string>>();
}