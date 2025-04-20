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
    [HttpPost]
    public async Task<IActionResult> UploadImage(
            int propertyId,
            IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No image provided.");

        // 1) Upload to your blob container (assumes your BlobStorageService 
        //    uses a "property‑images" container from config)
        var url = await _storage.UploadFileAsync(
            file.OpenReadStream(),
            file.FileName,
            OrgId
        );

        // 2) Determine next sort order
        var nextSort = await _db.PropertyImages
            .Where(pi => pi.PropertyId == propertyId)
            .MaxAsync(pi => (int?)pi.SortOrder) ?? 0;
        nextSort++;

        // 3) Persist a PropertyImage record
        var image = new PropertyImage
        {
            PropertyId = propertyId,
            Url = url,
            SortOrder = nextSort,
            UploadedAt = DateTime.UtcNow
        };
        _db.PropertyImages.Add(image);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            id = image.Id,
            url = image.Url,
            sortOrder = image.SortOrder
        });
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

