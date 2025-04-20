// REIstacks.Api/Controllers/Properties/PropertiesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Interfaces.IRepositories;
using REIstacks.Domain.Entities.Properties;

namespace REIstacks.Api.Controllers.Properties
{
    [ApiController]
    [Route("api/properties")]
    [Authorize]
    public class PropertiesController : TenantController
    {
        private readonly IPropertyRepository _repo;

        public PropertiesController(IPropertyRepository repo)
        {
            _repo = repo;
        }

        // GET api/properties?page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var (items, totalCount) = await _repo.GetPagedAsync(page, pageSize, OrgId);
                return Ok(new
                {
                    data = items,
                    totalCount,
                    page,
                    pageSize
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "User is not associated with an organization" });
            }
        }

        // GET api/properties/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var prop = await _repo.GetByIdAsync(id);
            if (prop == null)
                return NotFound();

            if (prop.OrganizationId != OrgId)
                return Forbid();

            return Ok(prop);
        }

        // GET api/properties/{id}/details
        [HttpGet("{id:int}/details")]
        public async Task<IActionResult> GetWithDetails(int id)
        {
            var prop = await _repo.GetPropertyWithDetailsAsync(id);
            if (prop == null)
                return NotFound();

            if (prop.OrganizationId != OrgId)
                return Forbid();

            return Ok(prop);
        }

        // GET api/properties/search?term=Oak&page=1&pageSize=20
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string term,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var all = await _repo.SearchPropertiesAsync(term, OrgId);
                var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                return Ok(new
                {
                    data = paged,
                    totalCount = all.Count(),
                    page,
                    pageSize
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "User is not associated with an organization" });
            }
        }

        // GET api/properties/type/{propertyType}
        [HttpGet("type/{propertyType}")]
        public async Task<IActionResult> ByType(string propertyType)
        {
            try
            {
                var list = await _repo.GetByPropertyTypeAsync(propertyType, OrgId);
                return Ok(list);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "User is not associated with an organization" });
            }
        }

        // GET api/properties/status/{propertyStatus}
        [HttpGet("status/{propertyStatus}")]
        public async Task<IActionResult> ByStatus(string propertyStatus)
        {
            try
            {
                var list = await _repo.GetByPropertyStatusAsync(propertyStatus, OrgId);
                return Ok(list);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "User is not associated with an organization" });
            }
        }

        // POST api/properties
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Property property)
        {
            try
            {
                property.OrganizationId = OrgId;
                var id = await _repo.AddAsync(property);
                return CreatedAtAction(nameof(GetById), new { id }, property);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "User is not associated with an organization" });
            }
        }

        // PUT api/properties/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Property property)
        {
            if (id != property.Id)
                return BadRequest();

            var exists = await _repo.ExistsAsync(id);
            if (!exists) return NotFound();

            if (property.OrganizationId != OrgId)
                return Forbid();

            await _repo.UpdateAsync(property);
            return NoContent();
        }

        // DELETE api/properties/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _repo.ExistsAsync(id);
            if (!exists) return NotFound();

            var prop = await _repo.GetByIdAsync(id);
            if (prop.OrganizationId != OrgId)
                return Forbid();

            await _repo.DeleteAsync(id);
            return NoContent();
        }

        // GET api/properties/{id}/interactions
        [HttpGet("{id:int}/interactions")]
        public async Task<IActionResult> GetInteractionCounts(int id)
        {
            var prop = await _repo.GetByIdAsync(id);
            if (prop.OrganizationId != OrgId)
                return Forbid();

            var counts = await _repo.GetInteractionCountsAsync(id);
            return Ok(counts);
        }

        // POST api/properties/{id}/interactions?type=view
        [HttpPost("{id:int}/interactions")]
        public async Task<IActionResult> IncrementInteraction(
            int id,
            [FromQuery] string interactionType)
        {
            var prop = await _repo.GetByIdAsync(id);
            if (prop.OrganizationId != OrgId)
                return Forbid();

            var updated = await _repo.IncrementInteractionCountAsync(id, interactionType);
            return Ok(updated);
        }

        // GET api/properties/{id}/notes
        [HttpGet("{id:int}/notes")]
        public async Task<IActionResult> GetNotes(int id)
        {
            var prop = await _repo.GetByIdAsync(id);
            if (prop.OrganizationId != OrgId)
                return Forbid();

            var notes = await _repo.GetPropertyNotesAsync(id);
            return Ok(notes);
        }

        // POST api/properties/{id}/notes
        [HttpPost("{id:int}/notes")]
        public async Task<IActionResult> AddNote(int id, [FromBody] PropertyNote note)
        {
            note.PropertyId = id;
            note.OrganizationId = OrgId;
            var created = await _repo.AddPropertyNoteAsync(note);
            return CreatedAtAction(nameof(GetNotes), new { id }, created);
        }

        // PUT api/properties/notes/{noteId}
        [HttpPut("notes/{noteId:int}")]
        public async Task<IActionResult> UpdateNote(int noteId, [FromBody] PropertyNote note)
        {
            if (note.Id != noteId) return BadRequest();
            note.OrganizationId = OrgId;
            var updated = await _repo.UpdatePropertyNoteAsync(note);
            return Ok(updated);
        }

        // DELETE api/properties/notes/{noteId}
        [HttpDelete("notes/{noteId:int}")]
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            // (Assumes note has its orgId set—else you'd fetch it first)
            await _repo.DeletePropertyNoteAsync(noteId);
            return NoContent();
        }

        // GET api/properties/{id}/documents
        [HttpGet("{id:int}/documents")]
        public async Task<IActionResult> GetDocuments(int id)
        {
            var prop = await _repo.GetByIdAsync(id);
            if (prop.OrganizationId != OrgId)
                return Forbid();

            var docs = await _repo.GetPropertyDocumentsAsync(id);
            return Ok(docs);
        }

        // POST api/properties/{id}/documents
        [HttpPost("{id:int}/documents")]
        public async Task<IActionResult> AddDocument(int id, [FromBody] PropertyDocument doc)
        {
            doc.PropertyId = id;
            doc.OrganizationId = OrgId;
            var created = await _repo.AddPropertyDocumentAsync(doc);
            return CreatedAtAction(nameof(GetDocuments), new { id }, created);
        }

        // DELETE api/properties/documents/{documentId}
        [HttpDelete("documents/{documentId:int}")]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {
            // (Assumes document has its orgId set—else you'd fetch it to verify)
            await _repo.DeletePropertyDocumentAsync(documentId);
            return NoContent();
        }

        // GET api/properties/contact/{contactId}
        [HttpGet("contact/{contactId:int}")]
        public async Task<IActionResult> ByContact(int contactId)
        {
            var list = await _repo.GetPropertiesByContactIdAsync(contactId);
            return Ok(list);
        }
    }
}
