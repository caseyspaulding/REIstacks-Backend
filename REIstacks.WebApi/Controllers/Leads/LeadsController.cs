using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Entities.CRM;
using System.Security.Claims;

namespace REIstacks.Api.Controllers.Leads
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsController : TenantController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILeadsImportService _leadService;

        public LeadsController(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILeadsImportService leadService,
            IHttpClientFactory httpClientFactory)
        {
            _leadService = leadService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPut("{id:int}/assign")]
        public async Task<IActionResult> Assign(int id, [FromBody] AssignLeadRequest req)
        {
            var ok = await _leadService.AssignLeadAsync(id, req.ProfileId, OrgId);
            return ok
                ? NoContent()
                : NotFound(new { error = "Lead not found or not in your org." });
        }


        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            // grab the tenant/org from the JWT
            var orgId = User.FindFirstValue("organization_id");
            if (string.IsNullOrEmpty(orgId))
                return Unauthorized();

            // fetch the lead
            var lead = await _unitOfWork.Leads.GetByIdAsync(id);
            if (lead == null || lead.OrganizationId != orgId)
                return NotFound();

            return Ok(lead);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateLead([FromBody] LeadRequest request)
        {
            // Use the organization id supplied with the request.
            // Validate that it's a valid Guid or fallback if necessary.
            if (!Guid.TryParse(request.OrganizationId, out var organizationId))
            {
                return BadRequest(new { error = "Invalid organization id" });
            }

            // Create and save the lead using the provided organization id.
            var lead = new Lead
            {
                OrganizationId = organizationId.ToString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Name = $"{request.FirstName} {request.LastName}".Trim(),
                Email = request.Email,
                Phone = request.Phone,
                PreferredContactMethod = request.PreferredContactMethod,
                PropertyStreetAddress = request.PropertyStreetAddress,
                PropertyCity = request.PropertyCity ?? request.City,
                City = request.City ?? request.PropertyCity,
                PropertyState = request.PropertyState ?? request.State,
                State = request.State ?? request.PropertyState,
                PropertyZipCode = request.PropertyZipCode ?? request.ZipCode,
                ZipCode = request.ZipCode ?? request.PropertyZipCode,
                PropertyType = request.PropertyType,
                TargetPrice = request.TargetPrice,
                AdditionalInfo = request.AdditionalInfo,
                ConsentTextMessages = request.ConsentTextMessages,
                ConsentPrivacyPolicy = request.ConsentPrivacyPolicy,
                Step = request.Step,
                Message = request.Message,
                Source = request.Source,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (request.PropertyCondition != null)
                lead.SetPropertyCondition(request.PropertyCondition);

            if (request.PropertyIssues != null)
                lead.SetPropertyIssues(request.PropertyIssues);

            if (request.ReasonForSelling != null)
                lead.SetReasonForSelling(request.ReasonForSelling);

            if (request.Timeline != null)
                lead.SetTimeline(request.Timeline);

            await _unitOfWork.Leads.AddAsync(lead);
            await _unitOfWork.CompleteAsync();

            return Ok(new { success = true, message = "Lead submitted successfully", leadId = lead.Id });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLeads([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var organizationId = User.FindFirstValue("organization_id");

            if (string.IsNullOrEmpty(organizationId))
            {
                return Unauthorized(new { error = "User is not associated with an organization" });
            }

            // Get paginated leads
            var leads = await _unitOfWork.Leads.GetPaginatedAsync(
                filter: l => l.OrganizationId == organizationId,
                page: page,
                pageSize: pageSize,
                orderBy: q => q.OrderByDescending(l => l.CreatedAt)
            );

            return Ok(new
            {
                data = leads.Items,
                total = leads.TotalCount,
                page = leads.Page,
                pageSize = leads.PageSize,
                totalPages = leads.TotalPages
            });
        }
    }
}
