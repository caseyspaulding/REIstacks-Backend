// REIstacks.Api/Controllers/ContactsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Services.Interfaces;
using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Api.Controllers;

[ApiController]
[Route("api/contacts")]
[Authorize]
public class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<IActionResult> GetContacts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            // Get organization ID from user claims
            var organizationId = User.FindFirst("organization_id")?.Value;
            if (string.IsNullOrEmpty(organizationId))
                return Unauthorized(new { error = "Organization ID not found in user claims" });

            var (contacts, totalCount, totalPages) = await _contactService.GetPagedContactsAsync(page, pageSize, organizationId);

            // Map to DTOs to avoid circular references
            var contactDtos = contacts.Select(c => new ContactDto
            {
                Id = c.Id,
                OrganizationId = c.OrganizationId,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone,
                AlternatePhone = c.AlternatePhone,
                Company = c.Company,
                Title = c.Title,
                PreferredContactMethod = c.PreferredContactMethod,
                StreetAddress = c.StreetAddress,
                City = c.City,
                State = c.State,
                ZipCode = c.ZipCode,
                IsActive = c.IsActive,
                ContactType = c.ContactType,
                LeadSource = c.LeadSource,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                Tags = c.Tags,
                Notes = c.Notes,
                Clicks = c.Clicks,
                Opens = c.Opens,
                SMSResponses = c.SMSResponses,
                CallsMade = c.CallsMade,
                MessagesLeft = c.MessagesLeft,
                LastContacted = c.LastContacted,
                ConsentTextMessages = c.ConsentTextMessages,
                ConsentEmailMarketing = c.ConsentEmailMarketing,

                // Organization data
                OrganizationName = c.Organization?.Name,

                // Counts instead of collections
                PropertiesCount = c.Properties?.Count ?? 0,
                DealsCount = c.Deals?.Count ?? 0,
                CommunicationsCount = c.Communications?.Count ?? 0,
                TasksCount = c.Tasks?.Count ?? 0
            }).ToList();

            return Ok(new
            {
                data = contactDtos,
                total = totalCount,
                totalPages = totalPages,
                page = page,
                pageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            // Log the FULL exception details
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.WriteLine($"STACK: {ex.StackTrace}");

            if (ex.InnerException != null)
                Console.WriteLine($"INNER: {ex.InnerException.Message}");

            return StatusCode(500, new { error = $"Contact retrieval failed: {ex.Message}" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContact(int id)
    {
        try
        {
            var contact = await _contactService.GetContactByIdAsync(id);
            if (contact == null)
                return NotFound(new { error = $"Contact with ID {id} not found" });

            // Check if contact belongs to user's organization
            var organizationId = User.FindFirst("organization_id")?.Value;
            if (contact.OrganizationId != organizationId)
                return Forbid();

            return Ok(contact);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateContact([FromBody] Contact contact)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Set organization ID from user claims
            var organizationId = User.FindFirst("organization_id")?.Value;
            if (string.IsNullOrEmpty(organizationId))
                return Unauthorized(new { error = "Organization ID not found in user claims" });

            contact.OrganizationId = organizationId;

            var contactId = await _contactService.CreateContactAsync(contact);

            return CreatedAtAction(nameof(GetContact), new { id = contactId },
                new { success = true, contactId = contactId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContact(int id, [FromBody] Contact contact)
    {
        try
        {
            if (id != contact.Id)
                return BadRequest(new { error = "ID mismatch" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify the contact belongs to user's organization
            var existingContact = await _contactService.GetContactByIdAsync(id);
            if (existingContact == null)
                return NotFound(new { error = $"Contact with ID {id} not found" });

            var organizationId = User.FindFirst("organization_id")?.Value;
            if (existingContact.OrganizationId != organizationId)
                return Forbid();

            // Ensure organization ID doesn't change
            contact.OrganizationId = organizationId;

            await _contactService.UpdateContactAsync(contact);

            return Ok(true);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContact(int id)
    {
        try
        {
            // Verify the contact belongs to user's organization
            var contact = await _contactService.GetContactByIdAsync(id);
            if (contact == null)
                return NotFound(new { error = $"Contact with ID {id} not found" });

            var organizationId = User.FindFirst("organization_id")?.Value;
            if (contact.OrganizationId != organizationId)
                return Forbid();

            await _contactService.DeleteContactAsync(id);

            return Ok(true);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("bulk-delete")]
    public async Task<IActionResult> BulkDeleteContacts([FromBody] BulkDeleteRequest request)
    {
        try
        {
            if (request.Ids == null || request.Ids.Length == 0)
                return BadRequest(new { error = "No IDs provided" });

            var organizationId = User.FindFirst("organization_id")?.Value;

            foreach (var id in request.Ids)
            {
                var contact = await _contactService.GetContactByIdAsync(id);
                if (contact != null && contact.OrganizationId == organizationId)
                {
                    await _contactService.DeleteContactAsync(id);
                }
            }

            return Ok(true);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("type/{contactType}")]
    public async Task<IActionResult> GetContactsByType(string contactType, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var organizationId = User.FindFirst("organization_id")?.Value;
            if (string.IsNullOrEmpty(organizationId))
                return Unauthorized(new { error = "Organization ID not found in user claims" });

            var contacts = await _contactService.GetContactsByTypeAsync(contactType, organizationId);

            // Manual paging since our repository method doesn't support it directly
            var totalCount = contacts.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var pagedContacts = contacts.Skip((page - 1) * pageSize).Take(pageSize);

            return Ok(new
            {
                data = pagedContacts,
                total = totalCount,
                totalPages = totalPages,
                page = page,
                pageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchContacts([FromQuery] string searchTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest(new { error = "Search term is required" });

            var organizationId = User.FindFirst("organization_id")?.Value;
            if (string.IsNullOrEmpty(organizationId))
                return Unauthorized(new { error = "Organization ID not found in user claims" });

            var contacts = await _contactService.SearchContactsAsync(searchTerm, organizationId);

            // Manual paging
            var totalCount = contacts.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var pagedContacts = contacts.Skip((page - 1) * pageSize).Take(pageSize);

            return Ok(new
            {
                data = pagedContacts,
                total = totalCount,
                totalPages = totalPages,
                page = page,
                pageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> ToggleContactStatus(int id, [FromBody] StatusUpdateRequest request)
    {
        try
        {
            // Verify the contact belongs to user's organization
            var contact = await _contactService.GetContactByIdAsync(id);
            if (contact == null)
                return NotFound(new { error = $"Contact with ID {id} not found" });

            var organizationId = User.FindFirst("organization_id")?.Value;
            if (contact.OrganizationId != organizationId)
                return Forbid();

            await _contactService.ToggleContactStatusAsync(id, request.IsActive);

            return Ok(true);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class BulkDeleteRequest
{
    public int[] Ids { get; set; }
}

public class StatusUpdateRequest
{
    public bool IsActive { get; set; }
}