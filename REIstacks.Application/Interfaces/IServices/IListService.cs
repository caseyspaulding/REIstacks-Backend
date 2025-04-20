using REIstacks.Domain.Entities.CRM;
using REIstacks.Domain.Entities.Properties;

namespace REIstacks.Application.Interfaces.IServices;
public interface IListService
{
    // List management
    Task<int> CreateListAsync(List list);
    Task<bool> UpdateListAsync(List list);
    Task<bool> DeleteListAsync(int listId, string organizationId);
    Task<IEnumerable<List>> GetListsAsync(string organizationId);
    Task<List?> GetListByIdAsync(int listId, string organizationId);

    // Property ↔ List membership
    Task AddPropertyToListAsync(int listId, int propertyId, string organizationId);
    Task RemovePropertyFromListAsync(int listId, int propertyId, string organizationId);
    Task<IEnumerable<Property>> GetPropertiesByListAsync(int listId, string organizationId);
}
