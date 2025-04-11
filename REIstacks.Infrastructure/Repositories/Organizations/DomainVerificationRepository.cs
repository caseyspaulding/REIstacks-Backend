// DomainVerificationRepository.cs
using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Infrastructure.Data;
using REIstacks.Infrastructure.Repositories.BaseRepository;


namespace REIstacks.Infrastructure.Repositories.Organizations
{
    public class DomainVerificationRepository : Repository<DomainVerification>, IDomainVerificationRepository
    {
        public DomainVerificationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<DomainVerification> GetByDomainAsync(string domain)
        {
            return await Context.DomainVerifications
                .FirstOrDefaultAsync(dv => dv.Domain == domain);
        }

        public async Task<IEnumerable<DomainVerification>> GetByOrganizationIdAsync(string organizationId)
        {
            return await Context.DomainVerifications
                .Where(dv => dv.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<bool> MarkAsVerifiedAsync(string domain, string organizationId)
        {
            var verification = await Context.DomainVerifications
                .FirstOrDefaultAsync(dv => dv.Domain == domain && dv.OrganizationId == organizationId);

            if (verification == null) return false;

            verification.IsVerified = true;
            verification.VerifiedAt = DateTime.UtcNow;
            verification.UpdatedAt = DateTime.UtcNow;

            return true;
        }
    }
}