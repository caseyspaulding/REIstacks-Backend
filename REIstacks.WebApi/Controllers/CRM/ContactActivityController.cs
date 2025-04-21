// Api/Controllers/CRM/ContactActivityController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;
using System.Security.Claims;

namespace REIstacks.Api.Controllers.CRM
{
    [ApiController]
    [Route("api/contacts/{contactId}/activity")]
    [Authorize]
    public class ContactActivityController : TenantController
    {
        private readonly IContactActivityService _svc;

        public ContactActivityController(IContactActivityService svc)
        {
            _svc = svc;
        }

        /// <summary>
        /// 1) List activities for a contact, with optional filters.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> List(
            int contactId,
            [FromQuery] ActivityType? type = null,
            [FromQuery] Guid? userId = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var acts = await _svc.GetForContactAsync(
                contactId, OrgId, type, userId, from, to);

            return Ok(acts);
        }

        /// <summary>
        /// 2) Append a new activity to this contact.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Log(
            int contactId,
            [FromBody] ContactActivity dto)
        {
            dto.ContactId = contactId;
            dto.OrganizationId = OrgId;
            dto.CreatedByProfileId = Guid.Parse(User.FindFirstValue("sub")!);

            var created = await _svc.LogAsync(dto);
            return CreatedAtAction(
                nameof(Log),
                new { contactId = contactId, id = created.Id },
                created);
        }

        /// <summary>
        /// 3) Return the list of all ActivityType names.
        /// </summary>
        [HttpGet("types")]
        public IActionResult GetActivityTypes()
            => Ok(Enum.GetNames(typeof(ActivityType)));

        /// <summary>
        /// 4) Return the list of users who have logged activities on this contact.
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetActingUsers(int contactId)
        {
            var users = await _svc.GetActingUsersForContactAsync(contactId, OrgId);
            return Ok(users);
        }
    }
}
