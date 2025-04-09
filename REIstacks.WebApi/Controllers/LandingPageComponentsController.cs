using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REIstacks.Domain.Models;
using REIstacks.Infrastructure.Data;

namespace reistacks_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LandingPageComponentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LandingPageComponentsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/LandingPageComponents
        [HttpPost]
        public async Task<ActionResult<LandingPageComponent>> PostLandingPageComponent(LandingPageComponent dto)
        {
            // 1. Validate the associated landing page exists
            if (!await _context.LandingPages.AnyAsync(lp => lp.Id == dto.LandingPageId))
            {
                return BadRequest("landingPageId does not reference a valid LandingPages record.");
            }

            // 2. Generate IDs and times
            dto.Id = Guid.NewGuid();
            dto.CreatedAt = DateTime.UtcNow;
            dto.UpdatedAt = DateTime.UtcNow;

            // 3. Save
            _context.LandingPageComponents.Add(dto);
            await _context.SaveChangesAsync();

            // 4. Return Created response
            return CreatedAtAction("GetLandingPageComponent",
               new { id = dto.Id },
               dto
            );
        }

        // Optionally define a GET by ID:
        [HttpGet("{id}")]
        public async Task<ActionResult<LandingPageComponent>> GetLandingPageComponent(Guid id)
        {
            var comp = await _context.LandingPageComponents.FindAsync(id);
            if (comp == null) return NotFound();
            return comp;
        }

        // ... PUT/DELETE, etc.
    }
}
