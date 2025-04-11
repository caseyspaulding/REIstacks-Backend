namespace REIstacks.Application.Contracts.Requests;
public class FinalizeImportRequest
{
    public int LeadListFileId { get; set; }
    // Example: { "CSV Column A": "FirstName", "Column B": "Phone" }
    public Dictionary<string, string> FieldMappings { get; set; }
}