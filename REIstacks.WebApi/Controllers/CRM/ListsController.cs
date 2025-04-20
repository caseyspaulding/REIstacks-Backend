// REIstacks.Api/Controllers/CRM/ListsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Interfaces.IServices;
using ListEntity = REIstacks.Domain.Entities.CRM.List;

namespace REIstacks.Api.Controllers.CRM
{
    [ApiController]
    [Route("api/lists")]
    [Authorize]
    public class ListsController : TenantController
    {
        private readonly IListService _listService;

        public ListsController(IListService listService)
        {
            _listService = listService;
        }

        // GET api/lists
        [HttpGet]
        public async Task<IActionResult> GetLists()
        {
            try
            {
                var lists = await _listService.GetListsAsync(OrgId);
                return Ok(lists);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        // GET api/lists/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetListById(int id)
        {
            try
            {
                var list = await _listService.GetListByIdAsync(id, OrgId);
                if (list == null)
                    return NotFound(new { error = $"List with ID {id} not found" });

                return Ok(list);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        // POST api/lists
        [HttpPost]
        public async Task<IActionResult> CreateList([FromBody] ListEntity list)
        {
            try
            {
                list.OrganizationId = OrgId;
                var newId = await _listService.CreateListAsync(list);
                return CreatedAtAction(nameof(GetListById),
                                       new { id = newId },
                                       list);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        // PUT api/lists/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateList(int id, [FromBody] ListEntity list)
        {
            try
            {
                if (id != list.Id)
                    return BadRequest(new { error = "ID in URL does not match ID in body" });

                list.OrganizationId = OrgId;
                var updated = await _listService.UpdateListAsync(list);
                if (!updated)
                    return NotFound(new { error = $"List with ID {id} not found or not in your organization" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        // DELETE api/lists/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteList(int id)
        {
            try
            {
                var deleted = await _listService.DeleteListAsync(id, OrgId);
                if (!deleted)
                    return NotFound(new { error = $"List with ID {id} not found or not in your organization" });

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        // POST api/lists/{listId}/properties/{propertyId}
        [HttpPost("{listId:int}/properties/{propertyId:int}")]
        public async Task<IActionResult> AddPropertyToList(int listId, int propertyId)
        {
            try
            {
                await _listService.AddPropertyToListAsync(listId, propertyId, OrgId);
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        // DELETE api/lists/{listId}/properties/{propertyId}
        [HttpDelete("{listId:int}/properties/{propertyId:int}")]
        public async Task<IActionResult> RemovePropertyFromList(int listId, int propertyId)
        {
            try
            {
                await _listService.RemovePropertyFromListAsync(listId, propertyId, OrgId);
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        // GET api/lists/{listId}/properties
        [HttpGet("{listId:int}/properties")]
        public async Task<IActionResult> GetPropertiesByList(int listId)
        {
            try
            {
                var properties = await _listService.GetPropertiesByListAsync(listId, OrgId);
                return Ok(properties);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
}
