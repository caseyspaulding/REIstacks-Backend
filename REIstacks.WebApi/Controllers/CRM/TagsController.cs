// REIstacks.Api/Controllers/CRM/TagsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Api.Controllers.CRM
{
    [ApiController]
    [Route("api/tags")]
    [Authorize]
    public class TagsController : TenantController
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // GET api/tags/{type}
        [HttpGet("{type}")]
        public async Task<IActionResult> GetTagsByType(string type)
        {
            try
            {
                var tags = await _tagService.GetTagsByTypeAsync(type, OrgId);
                return Ok(tags);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        // POST api/tags
        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] Tag tag)
        {
            try
            {
                tag.OrganizationId = OrgId;
                var tagId = await _tagService.CreateTagAsync(tag);
                return Ok(new { id = tagId });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DELETE api/tags/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                await _tagService.DeleteTagAsync(id);
                return Ok();
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

        // POST api/tags/{propertyId}/tags/{tagId}
        [HttpPost("{propertyId:int}/tags/{tagId:int}")]
        public async Task<IActionResult> AddTagToProperty(int propertyId, int tagId)
        {
            try
            {
                await _tagService.AddTagToPropertyAsync(propertyId, tagId, OrgId);
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        // POST api/tags/contact/{contactId}/tags/{tagId}
        [HttpPost("contact/{contactId:int}/tags/{tagId:int}")]
        public async Task<IActionResult> AddTagToContact(int contactId, int tagId)
        {
            try
            {
                await _tagService.AddTagToContactAsync(contactId, tagId);
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST api/tags/phone/{phoneId}/tags/{tagId}
        [HttpPost("phone/{phoneId:int}/tags/{tagId:int}")]
        public async Task<IActionResult> AddTagToPhone(int phoneId, int tagId)
        {
            try
            {
                await _tagService.AddTagToPhoneAsync(phoneId, tagId);
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DELETE api/tags/{propertyId}/tags/{tagId}
        [HttpDelete("{propertyId:int}/tags/{tagId:int}")]
        public async Task<IActionResult> RemoveTagFromProperty(int propertyId, int tagId)
        {
            try
            {
                await _tagService.RemoveTagFromPropertyAsync(propertyId, tagId);
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DELETE api/tags/contact/{contactId}/tags/{tagId}
        [HttpDelete("contact/{contactId:int}/tags/{tagId:int}")]
        public async Task<IActionResult> RemoveTagFromContact(int contactId, int tagId)
        {
            try
            {
                await _tagService.RemoveTagFromContactAsync(contactId, tagId);
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DELETE api/tags/phone/{phoneId}/tags/{tagId}
        [HttpDelete("phone/{phoneId:int}/tags/{tagId:int}")]
        public async Task<IActionResult> RemoveTagFromPhone(int phoneId, int tagId)
        {
            try
            {
                await _tagService.RemoveTagFromPhoneAsync(phoneId, tagId);
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
