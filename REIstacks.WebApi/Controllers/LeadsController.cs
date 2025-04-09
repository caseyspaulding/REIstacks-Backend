using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstack.Domain.Models;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Repositories.Interfaces;

namespace reistacks_api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class LeadsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public LeadsController(


        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
    }
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateLead([FromBody] LeadRequest request)
    {
        // Get the subdomain from the header
        var subdomain = Request.Headers["X-Subdomain"].FirstOrDefault();
        if (string.IsNullOrEmpty(subdomain))
        {
            return BadRequest(new { error = "Subdomain is required" });
        }

        // Find the organization by subdomain
        var organization = await _unitOfWork.Organizations.GetBySubdomainAsync(subdomain);
        if (organization == null)
        {
            return NotFound(new { error = "Organization not found" });
        }

        // Create and save the lead
        var lead = new Lead
        {
            OrganizationId = organization.Id,
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

        // Handle JSON serialization for arrays and objects
        if (request.PropertyCondition != null)
            lead.SetPropertyCondition(request.PropertyCondition);

        if (request.PropertyIssues != null)
            lead.SetPropertyIssues(request.PropertyIssues);

        if (request.ReasonForSelling != null)
            lead.SetReasonForSelling(request.ReasonForSelling);

        if (request.Timeline != null)
            lead.SetTimeline(request.Timeline);

        // Persist the lead
        await _unitOfWork.Leads.AddAsync(lead);
        await _unitOfWork.CompleteAsync();

        return Ok(new { success = true, message = "Lead submitted successfully", leadId = lead.Id });
    }
}
