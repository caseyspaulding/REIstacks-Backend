using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Application.Interfaces.IRepositories;
public interface IContactRepository
{
    Task<Contact> GetByIdAsync(int id);
    Task<IEnumerable<Contact>> GetAllAsync();
    Task<(IEnumerable<Contact> Contacts, int TotalCount)> GetPagedAsync(int page, int pageSize, string organizationId);
    Task<IEnumerable<Contact>> GetByOrganizationIdAsync(string organizationId);
    Task<IEnumerable<Contact>> GetByContactTypeAsync(string contactType, string organizationId);
    Task<IEnumerable<Contact>> SearchContactsAsync(string searchTerm, string organizationId);
    Task<int> AddAsync(Contact contact);
    void Update(Contact contact);
    void Delete(Contact contact);
    Task<bool> ExistsAsync(int id);
}
