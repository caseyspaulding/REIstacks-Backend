// In REIstacks.Infrastructure/Services/Storage/BlobStorageService.cs
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using REIstacks.Application.Interfaces.IServices;

namespace REIstacks.Infrastructure.Services.Storage;

public class BlobStorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureStorage:ConnectionString"];
        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerName = configuration["AzureStorage:LeadListsContainer"];
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string organizationId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();

        // Create unique filename with organization prefix
        var uniqueFileName = $"{organizationId}/{Guid.NewGuid()}-{fileName}";
        var blobClient = containerClient.GetBlobClient(uniqueFileName);

        await blobClient.UploadAsync(fileStream, true);
        return blobClient.Uri.ToString();
    }

    public async Task<Stream> DownloadFileAsync(string blobUrl)
    {
        var uri = new Uri(blobUrl);
        var blobName = uri.Segments.Last();

        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var downloadInfo = await blobClient.DownloadAsync();
        return downloadInfo.Value.Content;
    }
}