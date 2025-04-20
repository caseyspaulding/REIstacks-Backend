using CsvHelper;
using ExcelDataReader;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Interfaces.IServices;
using System.Globalization;

namespace REIstacks.Infrastructure.Services.CRM;
public class FileProcessingService : IFileProcessingService
{
    public async Task<IReadOnlyList<string>> GetHeadersAsync(Stream fileStream, string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (ext == ".csv")
            return await GetCsvHeadersAsync(fileStream);
        else
            return GetExcelHeaders(fileStream);
    }

    private async Task<IReadOnlyList<string>> GetCsvHeadersAsync(Stream stream)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        await csv.ReadAsync();
        csv.ReadHeader();
        return csv.HeaderRecord;
    }

    private IReadOnlyList<string> GetExcelHeaders(Stream stream)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        using var reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration() { LeaveOpen = true });
        if (!reader.Read()) return Array.Empty<string>();
        var headers = new List<string>();
        for (int i = 0; i < reader.FieldCount; i++)
            headers.Add(reader.GetValue(i)?.ToString() ?? "");
        return headers;
    }

    public async Task<IReadOnlyList<AddressCsvDto>> ParseAddressesAsync(
        Stream fileStream,
        string fileName,
        IReadOnlyDictionary<string, string> columnMap)
    {
        // normalize columnMap: keys = DTO property names ("FirstName","StreetAddress", …),
        // values = the header name in the file.

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (ext == ".csv")
            return await ParseCsvRowsAsync(fileStream, columnMap);
        else
            return ParseExcelRows(fileStream, columnMap);
    }

    private async Task<IReadOnlyList<AddressCsvDto>> ParseCsvRowsAsync(
        Stream stream,
        IReadOnlyDictionary<string, string> columnMap)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        await csv.ReadAsync();
        csv.ReadHeader();

        var list = new List<AddressCsvDto>();
        while (await csv.ReadAsync())
        {
            list.Add(new AddressCsvDto
            {
                FirstName = csv.GetField(columnMap["FirstName"]),
                LastName = csv.GetField(columnMap["LastName"]),
                StreetAddress = csv.GetField(columnMap["StreetAddress"]),
                City = csv.GetField(columnMap["City"]),
                State = csv.GetField(columnMap["State"]),
                ZipCode = csv.GetField(columnMap["ZipCode"])
            });
        }
        return list;
    }

    private IReadOnlyList<AddressCsvDto> ParseExcelRows(
        Stream stream,
        IReadOnlyDictionary<string, string> columnMap)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        using var reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration() { LeaveOpen = true });
        var sheet = reader.AsDataSet().Tables[0];
        var headers = sheet.Rows[0].ItemArray.Select(o => o?.ToString() ?? "").ToList();
        var mapIdx = columnMap.ToDictionary(
            kv => kv.Key,
            kv => headers.IndexOf(kv.Value)
        );

        var result = new List<AddressCsvDto>();
        for (int r = 1; r < sheet.Rows.Count; r++)
        {
            var row = sheet.Rows[r].ItemArray;
            result.Add(new AddressCsvDto
            {
                FirstName = row[mapIdx["FirstName"]].ToString(),
                LastName = row[mapIdx["LastName"]].ToString(),
                StreetAddress = row[mapIdx["StreetAddress"]].ToString(),
                City = row[mapIdx["City"]].ToString(),
                State = row[mapIdx["State"]].ToString(),
                ZipCode = row[mapIdx["ZipCode"]].ToString(),
            });
        }
        return result;
    }
}