using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REIstacks.Domain.Entities.Marketing;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Api.Controllers.Websites
{
    [Route("api/[controller]")]
    [ApiController]
    public class LandingPagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LandingPagesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/LandingPages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LandingPages>>> GetLandingPages()
        {
            return await _context.LandingPages.ToListAsync();
        }

        public class ReorderDto
        {
            public Guid Id { get; set; }
            public int OrderIndex { get; set; }
        }


        [HttpGet("{id}/with-components")]
        public async Task<ActionResult<LandingPages>> GetLandingPageWithComponents(Guid id)
        {
            var page = await _context.LandingPages
                .Include(lp => lp.Components)  // loads blocks in the .Components nav property
                .FirstOrDefaultAsync(lp => lp.Id == id);

            if (page == null) return NotFound();

            // (Optional) sort the Components in memory by OrderIndex
            page.Components = page.Components
                .OrderBy(c => c.OrderIndex)
                .ToList();

            return page;
        }

        [HttpPut("/api/LandingPages/{landingPageId}/components/reorder")]
        public async Task<IActionResult> ReorderBlocks(Guid landingPageId, [FromBody] List<ReorderDto> reorderList)
        {
            // Grab all blocks that belong to the specified page
            var blocks = await _context.LandingPageComponents
                .Where(c => c.LandingPageId == landingPageId)
                .ToListAsync();

            // Update each block's OrderIndex
            foreach (var item in reorderList)
            {
                var block = blocks.FirstOrDefault(b => b.Id == item.Id);
                if (block != null)
                {
                    block.OrderIndex = item.OrderIndex;
                    block.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }


        // GET: api/LandingPages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LandingPages>> GetLandingPages(Guid id)
        {
            var landingPages = await _context.LandingPages.FindAsync(id);

            if (landingPages == null)
            {
                return NotFound();
            }

            return landingPages;
        }

        // PUT: api/LandingPages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLandingPages(Guid id, LandingPages landingPages)
        {
            if (id != landingPages.Id)
            {
                return BadRequest();
            }

            _context.Entry(landingPages).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LandingPagesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/LandingPages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LandingPages>> PostLandingPages(LandingPages landingPages)
        {
            _context.LandingPages.Add(landingPages);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLandingPages", new { id = landingPages.Id }, landingPages);
        }

        // DELETE: api/LandingPages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLandingPages(Guid id)
        {
            var landingPages = await _context.LandingPages.FindAsync(id);
            if (landingPages == null)
            {
                return NotFound();
            }

            _context.LandingPages.Remove(landingPages);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LandingPagesExists(Guid id)
        {
            return _context.LandingPages.Any(e => e.Id == id);
        }

        // GET /api/LandingPages/{landingPageId}/components
        [HttpGet("/api/LandingPages/{landingPageId}/components")]
        public async Task<ActionResult<IEnumerable<LandingPageComponent>>> GetComponentsForPage(Guid landingPageId)
        {
            var components = await _context.LandingPageComponents
                .Where(c => c.LandingPageId == landingPageId)
                // If you’re using OrderIndex to sort the blocks top-to-bottom:
                .OrderBy(c => c.OrderIndex)
                .ToListAsync();

            return components;
        }

        // POST: /api/LandingPages/{landingPageId}/components
        [HttpPost("/api/LandingPages/{landingPageId}/components")]
        public async Task<ActionResult<LandingPageComponent>> CreateLandingPageComponent(
            Guid landingPageId,
            [FromBody] LandingPageComponent dto
        )
        {
            // 1. Validate route ID vs. body ID (if needed).
            // Some folks let the client pass the LandingPageId in the body; 
            // others just ignore the body and rely on the route param.
            if (dto.LandingPageId != Guid.Empty && dto.LandingPageId != landingPageId)
            {
                return BadRequest("Mismatched landingPageId in body vs. URL.");
            }

            // 2. Make sure the LandingPage actually exists
            var landingPage = await _context.LandingPages.FindAsync(landingPageId);
            if (landingPage == null)
            {
                return NotFound($"No landing page found with ID {landingPageId}");
            }

            // 3. Fill out the fields properly
            dto.Id = Guid.NewGuid();         // or let the DB auto-generate if you prefer
            dto.LandingPageId = landingPageId; // ensure correct association
            dto.CreatedAt = DateTime.UtcNow;
            dto.UpdatedAt = DateTime.UtcNow;

            // 4. Add to DB and save
            _context.LandingPageComponents.Add(dto);
            await _context.SaveChangesAsync();

            // 5. Return the newly created component
            return CreatedAtAction(
                nameof(GetComponentsForPage),  // a reference to an existing GET route
                new { landingPageId },
                dto
            );
        }
    }
}
