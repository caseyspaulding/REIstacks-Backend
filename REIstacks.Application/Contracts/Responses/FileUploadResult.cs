namespace REIstacks.Application.Contracts.Responses;
public class FileUploadResult
{
    public string FileName { get; set; }
    public IReadOnlyList<string> Headers { get; set; }
}