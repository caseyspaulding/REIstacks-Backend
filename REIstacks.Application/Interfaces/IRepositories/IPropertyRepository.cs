using REIstacks.Domain.Entities.Properties;
using REIstacks.Domain.Repositories;

namespace REIstacks.Application.Interfaces.IRepositories;

public interface IPropertyRepository : IRepository<Property>
{
    Task<(IEnumerable<Property> Properties, int TotalCount)> GetPagedAsync(int page, int pageSize, string organizationId);
    Task<IEnumerable<Property>> GetByOrganizationIdAsync(string organizationId);
    Task<IEnumerable<Property>> GetByPropertyTypeAsync(string propertyType, string organizationId);
    Task<IEnumerable<Property>> GetByPropertyStatusAsync(string propertyStatus, string organizationId);
    Task<Property> GetByIdAsync(int id);
    Task<Property> GetPropertyWithDetailsAsync(int id);
    Task<IEnumerable<Property>> SearchPropertiesAsync(string searchTerm, string organizationId);

    // Interaction counts methods
    Task<PropertyInteractionCount> GetInteractionCountsAsync(int propertyId);
    Task<PropertyInteractionCount> IncrementInteractionCountAsync(int propertyId, string interactionType);

    // Property notes methods (Message Board)
    Task<IEnumerable<PropertyNote>> GetPropertyNotesAsync(int propertyId);
    Task<PropertyNote> AddPropertyNoteAsync(PropertyNote note);
    Task<PropertyNote> UpdatePropertyNoteAsync(PropertyNote note);
    Task DeletePropertyNoteAsync(int noteId);

    // Property document methods
    Task<IEnumerable<PropertyDocument>> GetPropertyDocumentsAsync(int propertyId);
    Task<PropertyDocument> AddPropertyDocumentAsync(PropertyDocument document);
    Task DeletePropertyDocumentAsync(int documentId);

    // Contact-Property relationship
    Task<IEnumerable<Property>> GetPropertiesByContactIdAsync(int contactId);

    // Base repository methods
    Task<int> AddAsync(Property property);
    Task UpdateAsync(Property property);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}