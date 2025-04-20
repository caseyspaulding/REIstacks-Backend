using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Contracts.Responses;
using REIstacks.Application.Interfaces.IServices;

namespace REIstacks.Api.Controllers.CRM
{
    [ApiController]
    [Route("api/skiptrace")]
    [Authorize]
    public class SkipTraceController : TenantController
    {
        private readonly IFileProcessingService _files;
        private readonly ISkipTraceService _skip;

        public SkipTraceController(
            IFileProcessingService files,
            ISkipTraceService skip)
        {
            _files = files;
            _skip = skip;
        }
        /// <summary>
        /// 1) Upload a CSV/XLSX and return its column headers so the UI can prompt the user to map them.
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<FileUploadResult>> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            var headers = await _files.GetHeadersAsync(
                file.OpenReadStream(),
                file.FileName
            );

            return Ok(new FileUploadResult
            {
                FileName = file.FileName,
                Headers = headers
            });
        }
        /// <summary>
        /// 2) User submits the same file + their column→field mapping; we parse & kick off skip‑trace by raw addresses.
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("run")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<SkipTraceActivity>> RunMapped(
            [FromForm] IFormFile file,
            [FromForm] FieldMappingRequest mapping)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            // build the map: DTO property name → column header name
            var columnMap = new Dictionary<string, string>
            {
                ["FirstName"] = mapping.FirstNameColumn,
                ["LastName"] = mapping.LastNameColumn,
                ["StreetAddress"] = mapping.StreetColumn,
                ["City"] = mapping.CityColumn,
                ["State"] = mapping.StateColumn,
                ["ZipCode"] = mapping.ZipColumn,
            };

            // parse into your AddressDto list
            var rows = await _files.ParseAddressesAsync(
                file.OpenReadStream(),
                file.FileName,
                columnMap
            );

            // kick off skip‑trace by raw addresses
            var activity = await _skip.StartSkipTraceByAddressesAsync(rows, OrgId);

            return CreatedAtAction(nameof(GetById), new { id = activity.Id }, activity);
        }
        /// <summary>
        /// 3) Starts a skip‑trace run over an existing set of contact IDs.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SkipTraceActivity>> Start([FromBody] int[] contactIds)
        {
            if (contactIds == null || contactIds.Length == 0)
                return BadRequest("No contact IDs provided.");

            // **here** call the contact‑ID based skip‑trace
            var rows = await _skip.GetAddressesByContactIdsAsync(contactIds, OrgId); // Added this line to define 'rows'
            var activity = await _skip.StartSkipTraceByAddressesAsync(rows, OrgId);

            return CreatedAtAction(nameof(GetById), new { id = activity.Id }, activity);
        }

        /// <summary>
        /// 4) List all skip‑trace runs for this tenant.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var list = await _skip.GetActivitiesAsync(OrgId);
            return Ok(list);
        }

        /// <summary>
        /// 5) Fetch a single run by its ID (including breakdown & items).
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var activity = await _skip.GetActivityByIdAsync(id, OrgId);
            if (activity == null) return NotFound();
            return Ok(activity);
        }

        /// <summary>
        /// 6) Cancel an in‑progress run.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var ok = await _skip.CancelActivityAsync(id, OrgId);
            return ok ? NoContent() : NotFound();
        }
    }



}
