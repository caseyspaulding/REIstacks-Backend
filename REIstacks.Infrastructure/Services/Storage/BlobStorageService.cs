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
        // Ensure container name is in lower-case (as required by Azure)
        _containerName = configuration["AzureStorage:LeadListsContainer"].ToLowerInvariant();
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string organizationId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();

        // Create unique filename with organization prefix
        var uniqueFileName = $"{organizationId}/{Guid.NewGuid()}-{fileName}";
        var blobClient = containerClient.GetBlobClient(uniqueFileName);

        await blobClient.UploadAsync(fileStream, overwrite: true);
        return blobClient.Uri.ToString();
    }

    public async Task<Stream> DownloadFileAsync(string blobUrl)
    {
        // Get the base URI of the blob service (e.g., "https://reistacksstorage.blob.core.windows.net")
        var baseUri = _blobServiceClient.Uri.ToString().TrimEnd('/');

        // Build the expected prefix: "https://reistacksstorage.blob.core.windows.net/lead-list-container/"
        var expectedPrefix = $"{baseUri}/{_containerName.ToLowerInvariant()}/";

        if (!blobUrl.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("The blob URL does not match the expected format.");
        }

        // Extract the blob name which includes the virtual directory (e.g., "622c4013-4d3e-47e2-b1ed-efc904e0223b/...")
        var blobName = blobUrl.Substring(expectedPrefix.Length);

        // URL-decode the blob name so that %20 is converted back to a space, etc.
        blobName = Uri.UnescapeDataString(blobName);

        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var downloadInfo = await blobClient.DownloadAsync();
        return downloadInfo.Value.Content;
    }

}
