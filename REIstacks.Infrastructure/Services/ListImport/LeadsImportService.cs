using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Contracts.Requests; // Contains FinalizeImportRequest DTO.
using REIstacks.Application.Contracts.Responses;
using REIstacks.Application.Interfaces;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.UploadLeads;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Infrastructure.Services.ListImport
{
    public class LeadsImportService : ILeadsImportService
    {
        private readonly ICsvImportService _csvImportService;
        private readonly AppDbContext _db;
        private readonly IStorageService _storageService;

        public LeadsImportService(ICsvImportService csvImportService, AppDbContext db, IStorageService storageService)
        {
            _csvImportService = csvImportService;
            _db = db;
            _storageService = storageService;
        }

        /// <summary>
        /// Uploads the CSV file to blob storage and records the file upload in the database.
        /// </summary>
        public async Task<FileUploadResponse> UploadFileAsync(string organizationId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No CSV file provided.", nameof(file));

            // Upload the file to blob storage using the CSV import service.
            string blobUrl;
            using (var stream = file.OpenReadStream())
            {
                blobUrl = await _csvImportService.UploadFileAsync(stream, file.FileName, organizationId);
            }

            // Record the file upload in the database.
            var leadListFile = new LeadListFile
            {
                OrganizationId = organizationId,
                FileName = file.FileName,
                BlobUrl = blobUrl,
                MappingConfig = "{}",
                Tags = "",
                UploadedAt = DateTime.UtcNow
            };

            await _db.LeadListFiles.AddAsync(leadListFile);
            await _db.SaveChangesAsync();

            return new FileUploadResponse
            {
                Message = "File uploaded successfully",
                LeadListFileId = leadListFile.Id,
                BlobUrl = blobUrl
            };
        }

        /// <summary>
        /// Downloads the CSV file from blob storage and returns a preview of its content.
        /// </summary>
        public async Task<object> GetPreviewAsync(int leadListFileId, int sampleRows = 5)
        {
            // Retrieve the file record from the database.
            var leadListFile = await _db.LeadListFiles.FindAsync(leadListFileId);
            if (leadListFile == null)
                throw new Exception("LeadListFile not found.");

            // Download the file from blob storage.
            using var fileStream = await _storageService.DownloadFileAsync(leadListFile.BlobUrl);

            // Get a preview of the CSV data.
            var preview = await _csvImportService.GetPreviewAsync(fileStream, sampleRows);
            return preview;
        }
        public async Task<bool> AssignLeadAsync(int leadId, string profileId, string organizationId)
        {
            var lead = await _db.Leads
                .FirstOrDefaultAsync(l => l.Id == leadId && l.OrganizationId == organizationId);
            if (lead == null) return false;

            // Convert profileId to Guid for comparison
            if (!Guid.TryParse(profileId, out var profileGuid))
                throw new ArgumentException("Invalid profileId format");

            // Verify the target user exists and belongs to the organization
            var user = await _db.UserProfiles
                .FirstOrDefaultAsync(u => u.Id == profileGuid && u.OrganizationId == organizationId);
            if (user == null) throw new ArgumentException("Invalid user");

            // Fix: Assign the Guid value to the nullable Guid property
            lead.AssignedToProfileId = profileGuid;
            await _db.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Finalizes the CSV import by processing the file, mapping the fields, and updating the job record.
        /// </summary>
        public async Task<object> FinalizeImportAsync(FinalizeImportRequest request)
        {
            // Retrieve the file record from the database.
            var leadListFile = await _db.LeadListFiles.FindAsync(request.LeadListFileId);
            if (leadListFile == null)
                throw new Exception("LeadListFile not found.");

            // Create an import job record.
            var importJob = new ImportJob
            {
                FileId = request.LeadListFileId,
                OrganizationId = leadListFile.OrganizationId,
                Status = "InProgress",
                CreatedAt = DateTime.UtcNow,
                StartedAt = DateTime.UtcNow,
                ErrorMessage = ""
            };

            await _db.ImportJobs.AddAsync(importJob);
            await _db.SaveChangesAsync();

            // Process the CSV file using the provided field mappings.
            var result = await _csvImportService.ProcessFileFromStorageAsync(
                leadListFile.BlobUrl,
                request.FieldMappings,
                leadListFile.OrganizationId
            );

            // Update the import job based on the result.
            importJob.Status = result.Success ? "Completed" : "Failed";
            importJob.CompletedAt = DateTime.UtcNow;
            importJob.TotalImported = result.TotalImported;
            importJob.RecordsProcessed = result.TotalRows;
            importJob.RecordsImported = result.TotalImported;
            importJob.RecordsRejected = result.TotalRows - result.TotalImported;

            // Record each error (if any) in the import error log.
            if (!result.Success || result.Errors?.Count > 0)
            {
                foreach (var errorMsg in result.Errors)
                {
                    // Extract row number and field from error message
                    int rowNumber = 0;
                    string fieldName = "General";

                    // Extract row number if available
                    if (errorMsg.Contains("Row "))
                    {
                        var startIndex = errorMsg.IndexOf("Row ") + 4;
                        var endIndex = errorMsg.IndexOf(":", startIndex);
                        if (endIndex > startIndex && int.TryParse(errorMsg.Substring(startIndex, endIndex - startIndex), out int parsedRow))
                        {
                            rowNumber = parsedRow;
                        }
                    }

                    // Extract field name if available
                    if (errorMsg.Contains("Field: "))
                    {
                        var startIndex = errorMsg.IndexOf("Field: ") + 7;
                        var endIndex = errorMsg.IndexOf(")", startIndex);
                        if (endIndex > startIndex)
                        {
                            fieldName = errorMsg.Substring(startIndex, endIndex - startIndex);
                        }
                    }

                    var importError = new ImportError
                    {
                        JobId = importJob.Id,
                        ErrorMessage = errorMsg,
                        Field = fieldName,
                        RowNumber = rowNumber,
                        OrganizationId = leadListFile.OrganizationId,
                        OccurredAt = DateTime.UtcNow
                    };

                    await _db.ImportErrors.AddAsync(importError);
                }
            }

            _db.ImportJobs.Update(importJob);
            await _db.SaveChangesAsync();

            // Mark file as processed
            leadListFile.IsProcessed = true;
            leadListFile.ProcessedAt = DateTime.UtcNow;
            leadListFile.RecordsCount = result.TotalImported;
            _db.LeadListFiles.Update(leadListFile);
            await _db.SaveChangesAsync();

            // Return a response object for the finalize import operation.
            return new
            {
                success = result.Success,
                totalImported = result.TotalImported,
                totalProcessed = result.TotalRows,
                errors = result.Errors,
                jobId = importJob.Id
            };
        }
    }
}
