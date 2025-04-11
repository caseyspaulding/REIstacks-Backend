namespace REIstacks.Application.Interfaces.Models;
/// <summary>
/// Result of processing a CSV file, with success/fail status and any errors recorded.
/// </summary>
public class ImportResult
{
    public int TotalImported { get; set; }
    public bool Success { get; set; }
    public int TotalRows { get; set; } = 0;
    public List<string> Errors { get; set; } = new List<string>();
}

/// <summary>
/// Preview object that includes CSV headers, sample rows, and auto-suggested mappings.
/// </summary>
public class PreviewResult
{
    public List<string> Headers { get; set; } = new List<string>();
    public List<Dictionary<string, string>> Rows { get; set; } = new List<Dictionary<string, string>>();
    public Dictionary<string, string> SuggestedMappings { get; set; } = new Dictionary<string, string>();
}