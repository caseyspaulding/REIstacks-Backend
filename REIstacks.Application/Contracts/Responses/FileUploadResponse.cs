namespace REIstacks.Application.Contracts.Responses;
public class FileUploadResponse
{
    public string Message { get; set; }
    public int LeadListFileId { get; set; }
    public string BlobUrl { get; set; }
}
