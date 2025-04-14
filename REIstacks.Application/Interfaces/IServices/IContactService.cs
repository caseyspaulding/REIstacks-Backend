// REIstacks.Application/Services/Interfaces/IContactService.cs
using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Application.Services.Interfaces;

public interface IContactService
{
    Task<Contact> GetContactByIdAsync(int id);
    Task<(IEnumerable<Contact> Contacts, int TotalCount, int TotalPages)> GetPagedContactsAsync(int page, int pageSize, string organizationId);
    Task<IEnumerable<Contact>> GetContactsByOrganizationAsync(string organizationId);
    Task<IEnumerable<Contact>> GetContactsByTypeAsync(string contactType, string organizationId);
    Task<IEnumerable<Contact>> SearchContactsAsync(string searchTerm, string organizationId);
    Task<int> CreateContactAsync(Contact contact);
    Task UpdateContactAsync(Contact contact);
    Task DeleteContactAsync(int id);
    Task<bool> ToggleContactStatusAsync(int id, bool isActive);
}