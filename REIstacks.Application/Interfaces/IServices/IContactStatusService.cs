using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Application.Interfaces.IServices;
public interface IContactStatusService
{
    Task<IEnumerable<ContactStatus>> GetContactStatusesAsync();
    Task<Contact> UpdateContactStatusAsync(int contactId, string statusId);
}
