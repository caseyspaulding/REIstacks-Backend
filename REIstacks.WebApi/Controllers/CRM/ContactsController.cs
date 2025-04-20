// REIstacks.Api/Controllers/CRM/ContactsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Contracts.Requests;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Application.Services.Interfaces;
using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Api.Controllers.CRM
{
    [ApiController]
    [Route("api/contacts")]
    [Authorize]
    public class ContactsController : TenantController
    {
        private readonly IContactService _contactService;
        private readonly IPhoneStatusService _phoneService;
        private readonly IContactStatusService _contactStatusService;

        public ContactsController(
            IContactService contactService,
            IPhoneStatusService phoneService,
            IContactStatusService contactStatusService)
        {
            _contactService = contactService;
            _phoneService = phoneService;
            _contactStatusService = contactStatusService;
        }

        // GET api/contacts/statuses
        [HttpGet("statuses")]
        public async Task<ActionResult<IEnumerable<ContactStatus>>> GetContactStatuses()
        {
            var statuses = await _contactStatusService.GetContactStatusesAsync();
            return Ok(statuses);
        }

        // PUT api/contacts/{id}/status
        [HttpPut("{id:int}/status")]
        public async Task<ActionResult<Contact>> UpdateContactStatus(int id, [FromBody] string statusId)
        {
            try
            {
                var updated = await _contactStatusService.UpdateContactStatusAsync(id, statusId);
                return Ok(updated);
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

        // GET api/contacts/phone-statuses
        [HttpGet("phone-statuses")]
        public async Task<ActionResult<IEnumerable<PhoneStatus>>> GetPhoneStatuses()
        {
            var statuses = await _phoneService.GetPhoneStatusesAsync();
            return Ok(statuses);
        }

        // PUT api/contacts/phones/{id}/status
        [HttpPut("phones/{id:int}/status")]
        public async Task<ActionResult<ContactPhone>> UpdatePhoneStatus(int id, [FromBody] string statusId)
        {
            try
            {
                var updated = await _phoneService.UpdatePhoneStatusAsync(id, statusId);
                return Ok(updated);
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

        // GET api/contacts
        [HttpGet]
        public async Task<IActionResult> GetContacts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (contacts, totalCount, totalPages) =
                    await _contactService.GetPagedContactsAsync(page, pageSize, OrgId);

                var dtos = contacts.Select(c => new ContactDto
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
                    OrganizationName = c.Organization?.Name,
                    PropertiesCount = c.Properties?.Count ?? 0,
                    DealsCount = c.Deals?.Count ?? 0,
                    CommunicationsCount = c.Communications?.Count ?? 0,
                    TasksCount = c.Tasks?.Count ?? 0
                }).ToList();

                return Ok(new
                {
                    data = dtos,
                    total = totalCount,
                    totalPages,
                    page,
                    pageSize
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "User is not associated with an organization" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Contact retrieval failed: {ex.Message}" });
            }
        }

        // GET api/contacts/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetContact(int id)
        {
            try
            {
                var contact = await _contactService.GetContactByIdAsync(id);
                if (contact == null)
                    return NotFound(new { error = $"Contact {id} not found" });

                if (contact.OrganizationId != OrgId)
                    return Forbid();

                return Ok(contact);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "User is not associated with an organization" });
            }
        }

        // POST api/contacts
        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] Contact contact)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                contact.OrganizationId = OrgId;
                var newId = await _contactService.CreateContactAsync(contact);

                return CreatedAtAction(nameof(GetContact), new { id = newId }, new { success = true, contactId = newId });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "User is not associated with an organization" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // PUT api/contacts/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] Contact contact)
        {
            try
            {
                if (id != contact.Id)
                    return BadRequest(new { error = "ID mismatch" });

                var existing = await _contactService.GetContactByIdAsync(id);
                if (existing == null)
                    return NotFound(new { error = $"Contact {id} not found" });

                if (existing.OrganizationId != OrgId)
                    return Forbid();

                contact.OrganizationId = OrgId;
                await _contactService.UpdateContactAsync(contact);

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "User is not associated with an organization" });
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

        // DELETE api/contacts/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            try
            {
                var contact = await _contactService.GetContactByIdAsync(id);
                if (contact == null)
                    return NotFound(new { error = $"Contact {id} not found" });

                if (contact.OrganizationId != OrgId)
                    return Forbid();

                await _contactService.DeleteContactAsync(id);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "User is not associated with an organization" });
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

        // POST api/contacts/bulk-delete
        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDeleteContacts([FromBody] BulkDeleteRequest req)
        {
            try
            {
                if (req.Ids == null || req.Ids.Length == 0)
                    return BadRequest(new { error = "No IDs provided" });

                foreach (var id in req.Ids)
                {
                    var c = await _contactService.GetContactByIdAsync(id);
                    if (c != null && c.OrganizationId == OrgId)
                        await _contactService.DeleteContactAsync(id);
                }

                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "User is not associated with an organization" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // PUT api/contacts/{id}/status/toggle
        [HttpPut("{id:int}/status/toggle")]
        public async Task<IActionResult> ToggleContactStatus(int id, [FromBody] StatusUpdateRequest req)
        {
            try
            {
                var contact = await _contactService.GetContactByIdAsync(id);
                if (contact == null)
                    return NotFound(new { error = $"Contact {id} not found" });

                if (contact.OrganizationId != OrgId)
                    return Forbid();

                await _contactService.ToggleContactStatusAsync(id, req.IsActive);
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "User is not associated with an organization" });
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
}
