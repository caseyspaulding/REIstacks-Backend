using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REIstacks.Domain.Entities.Marketing;
using REIstacks.Infrastructure.Data;

namespace reistacks_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LandingPageLeadsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LandingPageLeadsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/LandingPageLeads
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LandingPageLead>>> GetLandingPageLeads()
        {
            return await _context.LandingPageLeads.ToListAsync();
        }

        // GET: api/LandingPageLeads/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LandingPageLead>> GetLandingPageLead(int id)
        {
            var landingPageLead = await _context.LandingPageLeads.FindAsync(id);

            if (landingPageLead == null)
            {
                return NotFound();
            }

            return landingPageLead;
        }

        // PUT: api/LandingPageLeads/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLandingPageLead(int id, LandingPageLead landingPageLead)
        {
            if (id != landingPageLead.Id)
            {
                return BadRequest();
            }

            _context.Entry(landingPageLead).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LandingPageLeadExists(id))
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

        // POST: api/LandingPageLeads
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LandingPageLead>> PostLandingPageLead(LandingPageLead landingPageLead)
        {
            _context.LandingPageLeads.Add(landingPageLead);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLandingPageLead", new { id = landingPageLead.Id }, landingPageLead);
        }

        // DELETE: api/LandingPageLeads/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLandingPageLead(int id)
        {
            var landingPageLead = await _context.LandingPageLeads.FindAsync(id);
            if (landingPageLead == null)
            {
                return NotFound();
            }

            _context.LandingPageLeads.Remove(landingPageLead);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LandingPageLeadExists(int id)
        {
            return _context.LandingPageLeads.Any(e => e.Id == id);
        }
    }
}
