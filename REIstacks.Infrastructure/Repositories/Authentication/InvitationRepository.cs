using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Infrastructure.Data;
using REIstacks.Infrastructure.Repositories.BaseRepository;

namespace REIstacks.Infrastructure.Repositories.Authentication
{
    public class InvitationRepository : Repository<Invitation>, IInvitationRepository
    {
        private readonly AppDbContext _context;  // ✅ Ensure direct access to DbContext

        public InvitationRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Invitation> GetByTokenAsync(string token)
        {
            return await _context.Invitations  // ✅ Directly use _context
                .FirstOrDefaultAsync(i => i.Token == token);
        }

        public async Task<IEnumerable<Invitation>> GetByOrganizationIdAsync(string organizationId)
        {
            return await _context.Invitations
                .Where(i => i.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invitation>> GetByEmailAsync(string email)
        {
            return await _context.Invitations
                .Where(i => i.Email == email)
                .ToListAsync();
        }

        public async Task<bool> MarkAsAcceptedAsync(string token, Guid acceptedByProfileId)
        {
            var invitation = await GetByTokenAsync(token);
            if (invitation == null || invitation.Status != "pending") return false;

            // ✅ Update invite status and timestamps
            invitation.Status = "accepted";
            invitation.AcceptedAt = DateTime.UtcNow;
            invitation.AcceptedByProfileId = acceptedByProfileId;
            invitation.UpdatedAt = DateTime.UtcNow;

            // ❌ DO NOT call `_context.SaveChangesAsync();` here
            return true; // Changes will be saved via UnitOfWork
        }
    }
}





