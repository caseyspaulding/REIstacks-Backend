using REIstacks.Application.Contracts.Requests;

namespace REIstacks.Application.Interfaces.IServices;
public interface IFileProcessingService
{
    /// <summary>
    /// Given an uploaded file stream (xlsx, csv), return the list of column headings.
    /// </summary>
    Task<IReadOnlyList<string>> GetHeadersAsync(Stream fileStream, string fileName);

    /// <summary>
    /// Parse every row into an AddressCsv (or full‐name variant) according to the user’s mapping.
    /// </summary>
    Task<IReadOnlyList<AddressCsvDto>> ParseAddressesAsync(
        Stream fileStream,
        string fileName,
        IReadOnlyDictionary<string, string> columnMap);


}
