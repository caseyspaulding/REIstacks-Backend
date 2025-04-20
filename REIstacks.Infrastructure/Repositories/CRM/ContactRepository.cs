// REIstacks.Infrastructure/Repositories/CRM/ContactRepository.cs
using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Interfaces.IRepositories;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Infrastructure.Data;
using REIstacks.Infrastructure.Repositories.BaseRepository;

namespace REIstacks.Infrastructure.Repositories.CRM;

public class ContactRepository : Repository<Contact>, IContactRepository
{
    private readonly AppDbContext _context;

    public ContactRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Contact> Contacts, int TotalCount)> GetPagedAsync(int page, int pageSize, string organizationId)
    {
        var query = _context.Contacts
            .Where(c => c.OrganizationId == organizationId)
            .OrderByDescending(c => c.CreatedAt);

        var totalCount = await query.CountAsync();
        var contacts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(c => c.Organization)
            .ToListAsync();

        return (contacts, totalCount);
    }

    public async Task<IEnumerable<Contact>> GetByOrganizationIdAsync(string organizationId)
    {
        return await _context.Contacts
            .Where(c => c.OrganizationId == organizationId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ContactPhone>> GetContactPhonesAsync(int contactId)
    {
        return await _context.ContactPhones
            .Where(p => p.ContactId == contactId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<ContactPhone> GetContactPhoneByIdAsync(int phoneId)
    {
        return await _context.ContactPhones.FindAsync(phoneId);
    }

    public async Task<ContactPhone> AddContactPhoneAsync(ContactPhone phone)
    {
        await _context.ContactPhones.AddAsync(phone);
        await _context.SaveChangesAsync();
        return phone;
    }

    public async Task<ContactPhone> UpdateContactPhoneAsync(ContactPhone phone)
    {
        _context.ContactPhones.Update(phone);
        await _context.SaveChangesAsync();
        return phone;
    }

    public async Task DeleteContactPhoneAsync(int phoneId)
    {
        var phone = await _context.ContactPhones.FindAsync(phoneId);
        if (phone != null)
        {
            _context.ContactPhones.Remove(phone);
            await _context.SaveChangesAsync();
        }
    }

    // New methods for contact emails
    public async Task<IEnumerable<ContactEmail>> GetContactEmailsAsync(int contactId)
    {
        return await _context.ContactEmails
            .Where(e => e.ContactId == contactId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<ContactEmail> GetContactEmailByIdAsync(int emailId)
    {
        return await _context.ContactEmails.FindAsync(emailId);
    }

    public async Task<ContactEmail> AddContactEmailAsync(ContactEmail email)
    {
        await _context.ContactEmails.AddAsync(email);
        await _context.SaveChangesAsync();
        return email;
    }

    public async Task<ContactEmail> UpdateContactEmailAsync(ContactEmail email)
    {
        _context.ContactEmails.Update(email);
        await _context.SaveChangesAsync();
        return email;
    }

    public async Task DeleteContactEmailAsync(int emailId)
    {
        var email = await _context.ContactEmails.FindAsync(emailId);
        if (email != null)
        {
            _context.ContactEmails.Remove(email);
            await _context.SaveChangesAsync();
        }
    }

    // Enhanced Get methods that include phones and emails
    public async Task<Contact> GetContactWithDetailsAsync(int contactId)
    {
        return await _context.Contacts
            .Include(c => c.PhoneNumbers)
            .Include(c => c.EmailAddresses)
            .FirstOrDefaultAsync(c => c.Id == contactId);
    }

    // Method to search contacts with phone numbers or emails containing the search term
    public async Task<IEnumerable<Contact>> AdvancedSearchContactsAsync(string searchTerm, string organizationId)
    {
        searchTerm = searchTerm.ToLower();

        // Get contacts matching the search term in their main fields
        var directMatches = await _context.Contacts
            .Where(c => c.OrganizationId == organizationId &&
                (c.FirstName.ToLower().Contains(searchTerm) ||
                 c.LastName.ToLower().Contains(searchTerm) ||
                 c.Email.ToLower().Contains(searchTerm) ||
                 c.Phone.Contains(searchTerm) ||
                 c.Company.ToLower().Contains(searchTerm)))
            .ToListAsync();

        // Get contacts matching the search term in their phone numbers
        var phoneMatches = await _context.ContactPhones
            .Where(p => p.PhoneNumber.Contains(searchTerm))
            .Select(p => p.Contact)
            .Where(c => c.OrganizationId == organizationId)
            .ToListAsync();

        // Get contacts matching the search term in their email addresses
        var emailMatches = await _context.ContactEmails
            .Where(e => e.EmailAddress.ToLower().Contains(searchTerm))
            .Select(e => e.Contact)
            .Where(c => c.OrganizationId == organizationId)
            .ToListAsync();

        // Combine and de-duplicate results
        return directMatches
            .Union(phoneMatches)
            .Union(emailMatches)
            .OrderByDescending(c => c.CreatedAt);
    }

    public async Task<IEnumerable<Contact>> GetByContactTypeAsync(string contactType, string organizationId)
    {
        return await _context.Contacts
            .Where(c => c.ContactType == contactType && c.OrganizationId == organizationId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contact>> SearchContactsAsync(string searchTerm, string organizationId)
    {
        searchTerm = searchTerm.ToLower();
        return await _context.Contacts
            .Where(c => c.OrganizationId == organizationId &&
                (c.FirstName.ToLower().Contains(searchTerm) ||
                 c.LastName.ToLower().Contains(searchTerm) ||
                 c.Email.ToLower().Contains(searchTerm) ||
                 c.Phone.Contains(searchTerm) ||
                 c.Company.ToLower().Contains(searchTerm)))
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Contact> GetByIdAsync(int id)
    {
        return await _context.Contacts.FindAsync(id);
    }

    async Task<int> IContactRepository.AddAsync(Contact contact)
    {
        await _context.Contacts.AddAsync(contact);
        return contact.Id;
    }

    public void Delete(Contact contact)
    {
        _context.Contacts.Remove(contact);
    }

    public Task<bool> ExistsAsync(int id)
    {
        throw new NotImplementedException();
    }
}