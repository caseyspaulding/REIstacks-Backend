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

    public Task<Contact> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    Task<int> IContactRepository.AddAsync(Contact contact)
    {
        throw new NotImplementedException();
    }

    public void Delete(Contact contact)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(int id)
    {
        throw new NotImplementedException();
    }
}