
using REIstacks.Application.Interfaces.Models;

namespace REIstacks.Application.Interfaces;

public interface ICsvImportService
{
    Task<PreviewResult> GetPreviewAsync(Stream fileStream, int sampleRows = 5);
    Task<ImportResult> ProcessCsvFile(Stream fileStream, Dictionary<string, string> fieldMappings, string organizationId);
    Task<ImportResult> ProcessFileFromStorageAsync(string blobUrl, Dictionary<string, string> fieldMappings, string organizationId);
    Dictionary<string, string> SuggestFieldMappings(IReadOnlyList<string> csvHeaders);
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string organizationId);
    void ValidateRequiredColumns(IReadOnlyList<string> csvHeaders, IReadOnlyList<string> requiredColumns);
}