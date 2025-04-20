using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Infrastructure.Services.CRM;
public class PhoneStatusService : IPhoneStatusService
{
    private readonly AppDbContext _context;

    public PhoneStatusService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ContactPhone> UpdatePhoneStatusAsync(int phoneId, string statusId)
    {
        var phone = await _context.ContactPhones.FindAsync(phoneId);
        if (phone == null)
            throw new KeyNotFoundException($"Phone with ID {phoneId} not found");

        // If statusId is null, it clears the status
        phone.StatusId = statusId;
        phone.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return phone;
    }

    public async Task<IEnumerable<PhoneStatus>> GetPhoneStatusesAsync()
    {
        return await _context.PhoneStatuses.ToListAsync();
    }
}
