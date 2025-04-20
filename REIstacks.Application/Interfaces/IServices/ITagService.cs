using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Application.Interfaces.IServices;
public interface ITagService
{
    // Get tags by type
    Task<IEnumerable<Tag>> GetTagsByTypeAsync(string type, string organizationId);

    // Create tag
    Task<int> CreateTagAsync(Tag tag);

    // Delete tag
    Task DeleteTagAsync(int id);

    // Add tag to entity

    Task AddTagToPropertyAsync(int propertyId, int tagId, string organizationId);
    Task AddTagToContactAsync(int contactId, int tagId);
    Task AddTagToPhoneAsync(int phoneId, int tagId);

    // Remove tag from entity
    Task RemoveTagFromPropertyAsync(int propertyId, int tagId);
    Task RemoveTagFromContactAsync(int contactId, int tagId);
    Task RemoveTagFromPhoneAsync(int phoneId, int tagId);
}