using Microsoft.AspNetCore.Http;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Contracts.Responses;

namespace REIstacks.Application.Interfaces.IServices;
public interface ILeadsImportService
{
    Task<FileUploadResponse> UploadFileAsync(string organizationId, IFormFile file);
    Task<object> GetPreviewAsync(int leadListFileId, int sampleRows = 5);
    Task<object> FinalizeImportAsync(FinalizeImportRequest request);
}