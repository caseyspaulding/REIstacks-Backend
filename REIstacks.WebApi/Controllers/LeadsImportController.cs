using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Interfaces.IServices;

namespace REIstacks.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeadsImportController : ControllerBase
    {
        private readonly ILeadsImportService _leadsImportService;

        public LeadsImportController(ILeadsImportService leadsImportService)
        {
            _leadsImportService = leadsImportService;
        }

        /// <summary>
        /// Accepts a CSV upload, stores it in blob storage, and records a LeadListFile.
        /// </summary>
        /// <param name="request">Contains the file to be uploaded.</param>
        /// <returns>A response indicating upload success, file ID, and blob URL.</returns>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadRequest request)
        {
            // Retrieve the OrganizationId from user claims.
            var organizationId = User.FindFirst("organization_id")?.Value;
            if (string.IsNullOrEmpty(organizationId))
            {
                return BadRequest("Organization ID not found in user claims.");
            }

            try
            {
                // Delegates the file uploading to the service
                var response = await _leadsImportService.UploadFileAsync(organizationId, request.File);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Optionally log the error here.
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Returns a preview of the CSV columns/rows to let the user map them.
        /// </summary>
        /// <param name="leadListFileId">The identifier for the uploaded file.</param>
        /// <returns>A preview of the CSV data.</returns>
        [HttpGet("preview/{leadListFileId}")]
        public async Task<IActionResult> PreviewCsv(int leadListFileId)
        {
            try
            {
                var preview = await _leadsImportService.GetPreviewAsync(leadListFileId);
                return Ok(preview);
            }
            catch (Exception ex)
            {
                // Optionally log the error here.
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Finalizes the CSV import by mapping columns, creating leads, and logging import details.
        /// </summary>
        /// <param name="request">The import finalization request containing field mappings and file identifier.</param>
        /// <returns>The result of the finalization process.</returns>
        [HttpPost("finalize")]
        public async Task<IActionResult> FinalizeImport([FromBody] FinalizeImportRequest request)
        {
            try
            {
                var result = await _leadsImportService.FinalizeImportAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Optionally log the error here.
                return BadRequest(ex.Message);
            }
        }
    }
}
