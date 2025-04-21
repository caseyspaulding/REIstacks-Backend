using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Contracts.Responses;
using REIstacks.Application.Interfaces.IServices;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace REIstacks.Api.Controllers.CRM
{
    [ApiController]
    [Route("api/skiptrace")]
    [Authorize]
    public class SkipTraceController : TenantController
    {
        private readonly IFileProcessingService _files;
        private readonly ISkipTraceService _skip;
        private readonly ILogger<SkipTraceController> _logger;
        private readonly string _apifyToken;
        private readonly HttpClient _apifyClient; // This is missing


        public SkipTraceController(
            IFileProcessingService files,
            ISkipTraceService skip,
            ILogger<SkipTraceController> logger,
            IHttpClientFactory httpFactory, // Add this
            IConfiguration config)
        {
            _apifyToken = config["APIFY_TOKEN"]
                ?? throw new ArgumentNullException("Missing Apify token in config");

            // Initialize both HTTP clients

            _apifyClient = httpFactory.CreateClient();
            _apifyClient.BaseAddress = new Uri("https://api.apify.com/v2/");

            // Set default headers for all requests

            _apifyClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apifyToken);

            _files = files ?? throw new ArgumentNullException(nameof(files));
            _skip = skip ?? throw new ArgumentNullException(nameof(skip));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        /// <summary>
        /// 1) Upload a CSV/XLSX and return its column headers so the UI can prompt the user to map them.
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<FileUploadResult>> Upload([FromForm] IFormFile file)
        {
            try
            {
                _logger.LogInformation("Processing file upload: {FileName}, Size: {Size} bytes",
                    file?.FileName, file?.Length);

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("File upload rejected: No file or empty file provided");
                    return BadRequest("No file provided or file is empty.");
                }

                if (!IsValidFileExtension(file.FileName))
                {
                    _logger.LogWarning("File upload rejected: Invalid file type: {FileName}", file.FileName);
                    return BadRequest("Invalid file type. Only CSV and XLSX files are supported.");
                }

                var headers = await _files.GetHeadersAsync(
                    file.OpenReadStream(),
                    file.FileName
                );

                _logger.LogInformation("File uploaded successfully: {FileName}, Found {HeaderCount} headers",
                    file.FileName, headers?.Count ?? 0);

                return Ok(new FileUploadResult
                {
                    FileName = file.FileName,
                    Headers = headers
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file upload: {FileName}", file?.FileName);
                return StatusCode(500, "An error occurred while processing your file. Please try again.");
            }
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
            try
            {
                _logger.LogInformation("Starting mapped skip trace run: File: {FileName}", file?.FileName);

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Mapped skip trace rejected: No file or empty file provided");
                    return BadRequest("No file provided or file is empty.");
                }

                if (!IsValidFileExtension(file.FileName))
                {
                    _logger.LogWarning("Mapped skip trace rejected: Invalid file type: {FileName}", file.FileName);
                    return BadRequest("Invalid file type. Only CSV and XLSX files are supported.");
                }

                if (!IsValidMapping(mapping))
                {
                    _logger.LogWarning("Mapped skip trace rejected: Invalid column mapping");
                    return BadRequest("Invalid column mapping. Required fields must be mapped to columns.");
                }

                // Build the map: DTO property name → column header name
                var columnMap = new Dictionary<string, string>
                {
                    ["FirstName"] = mapping.FirstNameColumn,
                    ["LastName"] = mapping.LastNameColumn,
                    ["StreetAddress"] = mapping.StreetColumn,
                    ["City"] = mapping.CityColumn,
                    ["State"] = mapping.StateColumn,
                    ["ZipCode"] = mapping.ZipColumn,
                };

                // Parse into your AddressDto list
                var rows = await _files.ParseAddressesAsync(
                    file.OpenReadStream(),
                    file.FileName,
                    columnMap
                );

                if (rows == null || !rows.Any())
                {
                    _logger.LogWarning("Mapped skip trace: No valid addresses found in file {FileName}", file.FileName);
                    return BadRequest("No valid addresses found in the file.");
                }

                _logger.LogInformation("Parsed {Count} addresses from file {FileName}", rows.Count(), file.FileName);

                // Kick off skip‑trace by raw addresses
                var activity = await _skip.StartSkipTraceByAddressesAsync(rows, OrgId);

                _logger.LogInformation("Skip trace activity created: {ActivityId} with {Count} addresses",
                    activity.Id, rows.Count());

                return CreatedAtAction(nameof(GetById), new { id = activity.Id }, activity);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Format error parsing file {FileName}", file?.FileName);
                return BadRequest($"Error parsing file: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running mapped skip trace: {FileName}", file?.FileName);
                return StatusCode(500, "An error occurred while processing your request. Please try again.");
            }
        }

        /// <summary>
        /// 3) Starts a skip‑trace run over an existing set of contact IDs.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SkipTraceActivity>> Start([FromBody] int[] contactIds)
        {
            try
            {
                _logger.LogInformation("Starting skip trace for {Count} contacts", contactIds?.Length ?? 0);

                if (contactIds == null || contactIds.Length == 0)
                {
                    _logger.LogWarning("Skip trace rejected: No contact IDs provided");
                    return BadRequest("No contact IDs provided.");
                }

                // This is a more direct method that does both steps in one call
                var activity = await _skip.StartSkipTraceAsync(contactIds, OrgId);

                _logger.LogInformation("Skip trace activity created: {ActivityId} for {Count} contacts",
                    activity.Id, contactIds.Length);

                return CreatedAtAction(nameof(GetById), new { id = activity.Id }, activity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting skip trace for {Count} contacts", contactIds?.Length ?? 0);
                return StatusCode(500, "An error occurred while processing your request. Please try again.");
            }
        }

        /// <summary>
        /// 4) List all skip‑trace runs for this tenant.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            try
            {
                _logger.LogInformation("Retrieving skip trace activities for organization {OrgId}", OrgId);

                var list = await _skip.GetActivitiesAsync(OrgId);

                _logger.LogInformation("Retrieved {Count} skip trace activities for organization {OrgId}",
                    list?.Count() ?? 0, OrgId);

                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving skip trace activities for organization {OrgId}", OrgId);
                return StatusCode(500, "An error occurred while retrieving skip trace activities.");
            }
        }

        /// <summary>
        /// 5) Fetch a single run by its ID (including breakdown & items).
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving skip trace activity {ActivityId} for organization {OrgId}",
                    id, OrgId);

                var activity = await _skip.GetActivityByIdAsync(id, OrgId);

                if (activity == null)
                {
                    _logger.LogWarning("Skip trace activity {ActivityId} not found for organization {OrgId}",
                        id, OrgId);
                    return NotFound();
                }

                _logger.LogInformation("Successfully retrieved skip trace activity {ActivityId}", id);
                return Ok(activity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving skip trace activity {ActivityId} for organization {OrgId}",
                    id, OrgId);
                return StatusCode(500, "An error occurred while retrieving the skip trace activity.");
            }
        }

        /// <summary>
        /// 6) Cancel an in‑progress run.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to cancel skip trace activity {ActivityId} for organization {OrgId}",
                    id, OrgId);

                var ok = await _skip.CancelActivityAsync(id, OrgId);

                if (!ok)
                {
                    _logger.LogWarning("Skip trace activity {ActivityId} not found for cancellation", id);
                    return NotFound();
                }

                _logger.LogInformation("Successfully cancelled skip trace activity {ActivityId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling skip trace activity {ActivityId} for organization {OrgId}",
                    id, OrgId);
                return StatusCode(500, "An error occurred while attempting to cancel the skip trace activity.");
            }
        }

        /// <summary>
        /// Get current pricing information for skip trace operations
        /// </summary>
        [HttpGet("pricing")]
        public IActionResult GetPricing()
        {
            try
            {
                _logger.LogInformation("Retrieving skip trace pricing information");

                // You could store these in configuration if they change frequently
                // or pull them from a database if they vary by tenant
                var pricing = new
                {
                    BaseCost = 10.00m,
                    PerThousandRecords = 10.00m,
                    Currency = "USD"
                };

                _logger.LogInformation("Retrieved skip trace pricing information");
                return Ok(pricing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving skip trace pricing information");
                return StatusCode(500, "An error occurred while retrieving pricing information.");
            }
        }

        /// <summary>
        /// List Apify Actor runs
        /// </summary>
        [HttpGet("apify/runs")]
        public async Task<IActionResult> ListApifyRuns()
        {
            var url = $"runs?token={_apifyToken}";
            var response = await _apifyClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Apify list runs failed: {Status}", response.StatusCode);
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            var runs = await response.Content.ReadFromJsonAsync<JsonElement>();
            return Ok(runs);
        }

        /// <summary>
        /// Get the last Apify Actor run
        /// </summary>
        [HttpGet("apify/runs/last")]
        public async Task<IActionResult> GetLastApifyRun([FromQuery] string status = null)
        {
            var query = status != null ? $"runs/last?status={status}&token={_apifyToken}" : $"runs/last?token={_apifyToken}";
            var response = await _apifyClient.GetAsync(query);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Apify get last run failed: {Status}", response.StatusCode);
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            var lastRun = await response.Content.ReadFromJsonAsync<JsonElement>();
            return Ok(lastRun);
        }

        /// <summary>
        /// Get dataset items from the last Apify Actor run
        /// </summary>
        [HttpGet("apify/runs/last/dataset/items")]
        public async Task<IActionResult> GetLastApifyRunDatasetItems(
            [FromQuery] string status = null,
            [FromQuery] int? limit = null)
        {
            var builder = new StringBuilder("runs/last/dataset/items?");
            if (status != null) builder.Append($"status={status}&");
            if (limit.HasValue) builder.Append($"limit={limit.Value}&");
            builder.Append($"token={_apifyToken}");

            var response = await _apifyClient.GetAsync(builder.ToString());
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Apify get last dataset items failed: {Status}", response.StatusCode);
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            var items = await response.Content.ReadFromJsonAsync<JsonElement>();
            return Ok(items);
        }

        private bool IsValidFileExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            return extension == ".csv" || extension == ".xlsx" || extension == ".xls";
        }

        private bool IsValidMapping(FieldMappingRequest mapping)
        {
            if (mapping == null)
                return false;

            // Check required fields - adjust based on your application requirements
            return !string.IsNullOrEmpty(mapping.StreetColumn) &&
                   !string.IsNullOrEmpty(mapping.CityColumn) &&
                   !string.IsNullOrEmpty(mapping.StateColumn) &&
                   !string.IsNullOrEmpty(mapping.ZipColumn);
        }


    }
}