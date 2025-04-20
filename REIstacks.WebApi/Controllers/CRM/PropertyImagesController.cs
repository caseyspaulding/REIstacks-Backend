using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.Properties;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Api.Controllers.CRM;

[ApiController]
[Route("api/properties/{propertyId:int}/images")]
[Authorize]
public class PropertyImagesController : TenantController
{
    private readonly AppDbContext _db;
    private readonly IStorageService _storage; // e.g. S3 / Azure Blob abstraction

    public PropertyImagesController(AppDbContext db, IStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int propertyId)
    {
        var images = await _db.PropertyImages
            .Where(pi => pi.PropertyId == propertyId)
            .OrderBy(pi => pi.SortOrder)
            .ToListAsync();
        return Ok(images);
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost, DisableRequestSizeLimit]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(
     int propertyId,
     [FromForm] IFormFile file,
     [FromForm] int sortOrder = 0)
    {
        if (file == null) return BadRequest();

        // upload to your blob container, namespaced by org
        var url = await _storage.UploadFileAsync(
                      file.OpenReadStream(),
                      file.FileName,
                      OrgId        // your TenantController property
                  );

        var img = new PropertyImage
        {
            PropertyId = propertyId,
            Url = url,
            SortOrder = sortOrder
        };
        _db.PropertyImages.Add(img);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { propertyId }, img);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int propertyId, int id)
    {
        var img = await _db.PropertyImages.FindAsync(id);
        if (img == null || img.PropertyId != propertyId)
            return NotFound();

        // optionally: await _storage.DeleteAsync(img.Url);
        _db.PropertyImages.Remove(img);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

