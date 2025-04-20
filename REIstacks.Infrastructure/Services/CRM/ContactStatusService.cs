using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Infrastructure.Services.CRM;
public class ContactStatusService : IContactStatusService
{
    private readonly AppDbContext _context;

    public ContactStatusService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ContactStatus>> GetContactStatusesAsync()
    {
        return await _context.ContactStatuses.ToListAsync();
    }

    public async Task<Contact> UpdateContactStatusAsync(int contactId, string statusId)
    {
        var contact = await _context.Contacts.FindAsync(contactId);
        if (contact == null)
            throw new KeyNotFoundException($"Contact with ID {contactId} not found");

        contact.StatusId = statusId;
        contact.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return contact;
    }
}