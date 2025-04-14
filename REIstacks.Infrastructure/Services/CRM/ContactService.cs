// REIstacks.Infrastructure/Services/CRM/ContactService.cs
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Application.Services.Interfaces;
using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Infrastructure.Services.CRM;

public class ContactService : IContactService
{
    private readonly IUnitOfWork _unitOfWork;

    public ContactService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Contact> GetContactByIdAsync(int id)
    {
        return await _unitOfWork.Contacts.GetByIdAsync(id);
    }

    public async Task<(IEnumerable<Contact> Contacts, int TotalCount, int TotalPages)> GetPagedContactsAsync(
        int page, int pageSize, string organizationId)
    {
        var (contacts, totalCount) = await _unitOfWork.Contacts.GetPagedAsync(page, pageSize, organizationId);
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return (contacts, totalCount, totalPages);
    }

    public async Task<IEnumerable<Contact>> GetContactsByOrganizationAsync(string organizationId)
    {
        return await _unitOfWork.Contacts.GetByOrganizationIdAsync(organizationId);
    }

    public async Task<IEnumerable<Contact>> GetContactsByTypeAsync(string contactType, string organizationId)
    {
        return await _unitOfWork.Contacts.GetByContactTypeAsync(contactType, organizationId);
    }

    public async Task<IEnumerable<Contact>> SearchContactsAsync(string searchTerm, string organizationId)
    {
        return await _unitOfWork.Contacts.SearchContactsAsync(searchTerm, organizationId);
    }

    public async Task<int> CreateContactAsync(Contact contact)
    {
        contact.CreatedAt = DateTime.UtcNow;
        contact.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Contacts.AddAsync(contact);
        await _unitOfWork.CompleteAsync();

        return contact.Id;
    }

    public async Task UpdateContactAsync(Contact contact)
    {
        var existingContact = await _unitOfWork.Contacts.GetByIdAsync(contact.Id);

        if (existingContact == null)
            throw new KeyNotFoundException($"Contact with ID {contact.Id} not found");

        // Update properties but preserve CreatedAt
        contact.CreatedAt = existingContact.CreatedAt;
        contact.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Contacts.Update(contact);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteContactAsync(int id)
    {
        var contact = await _unitOfWork.Contacts.GetByIdAsync(id);

        if (contact == null)
            throw new KeyNotFoundException($"Contact with ID {id} not found");

        _unitOfWork.Contacts.Delete(contact);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<bool> ToggleContactStatusAsync(int id, bool isActive)
    {
        var contact = await _unitOfWork.Contacts.GetByIdAsync(id);

        if (contact == null)
            throw new KeyNotFoundException($"Contact with ID {id} not found");

        contact.IsActive = isActive;
        contact.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Contacts.Update(contact);
        await _unitOfWork.CompleteAsync();

        return true;
    }
}