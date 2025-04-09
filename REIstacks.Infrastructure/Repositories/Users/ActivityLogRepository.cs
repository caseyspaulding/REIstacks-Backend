using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Domain.Models;
using REIstacks.Infrastructure.Data;
using REIstacks.Infrastructure.Repositories.BaseRepository;

namespace REIstacks.Application.Services.Users
{
    public class ActivityLogRepository : Repository<ActivityLog>, IActivityLogRepository
    {
        private readonly AppDbContext _context;

        public ActivityLogRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActivityLog>> GetByOrganizationIdAsync(string organizationId)
        {
            return await _context.ActivityLogs
                .Where(log => log.OrganizationId == organizationId)
                .OrderByDescending(log => log.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityLog>> GetByProfileIdAsync(Guid profileId)
        {
            return await _context.ActivityLogs
                .Where(log => log.ProfileId == profileId)
                .OrderByDescending(log => log.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityLog>> GetByActionTypeAsync(string actionType)
        {
            return await _context.ActivityLogs
                .Where(log => log.ActionType == actionType)
                .OrderByDescending(log => log.CreatedAt)
                .ToListAsync();
        }

        // Replace this method with direct Add
        public async Task LogActivityAsync(string organizationId, Guid profileId, string actionType, string details)
        {
            var log = new ActivityLog
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                ProfileId = profileId,
                ActionType = actionType,
                Details = details,
                CreatedAt = DateTime.UtcNow
            };

            // Use the regular Add method from the base Repository
            await AddAsync(log);
            // Don't call SaveChanges - UnitOfWork will handle it
        }
    }

}