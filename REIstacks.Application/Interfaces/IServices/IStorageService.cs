namespace REIstacks.Application.Interfaces.IServices;
public interface IStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string organizationId);
    Task<Stream> DownloadFileAsync(string blobUrl);
}