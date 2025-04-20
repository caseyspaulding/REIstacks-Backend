using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Interfaces.IRepositories;
using REIstacks.Domain.Entities.Properties;
using REIstacks.Infrastructure.Data;
using REIstacks.Infrastructure.Repositories.BaseRepository;

namespace REIstacks.Infrastructure.Repositories.Properties;

public class PropertyRepository : Repository<Property>, IPropertyRepository
{
    private readonly AppDbContext _context;

    public PropertyRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Property> Properties, int TotalCount)> GetPagedAsync(int page, int pageSize, string organizationId)
    {
        var query = _context.Properties
            .Where(p => p.OrganizationId == organizationId)
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync();
        var properties = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.OwnerContact)
            .ToListAsync();

        return (properties, totalCount);
    }

    public async Task<IEnumerable<Property>> GetByOrganizationIdAsync(string organizationId)
    {
        return await _context.Properties
            .Where(p => p.OrganizationId == organizationId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetByPropertyTypeAsync(string propertyType, string organizationId)
    {
        return await _context.Properties
            .Where(p => p.PropertyType == propertyType && p.OrganizationId == organizationId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetByPropertyStatusAsync(string propertyStatus, string organizationId)
    {
        return await _context.Properties
            .Where(p => p.PropertyStatus == propertyStatus && p.OrganizationId == organizationId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Property> GetByIdAsync(int id)
    {
        return await _context.Properties
            .Include(p => p.OwnerContact)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Property> GetPropertyWithDetailsAsync(int id)
    {
        return await _context.Properties
            .Include(p => p.OwnerContact)
            .Include(p => p.Documents)
            .Include(p => p.Deals)
            .Include(p => p.TaskItems)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Property>> SearchPropertiesAsync(string searchTerm, string organizationId)
    {
        searchTerm = searchTerm.ToLower();

        return await _context.Properties
            .Where(p => p.OrganizationId == organizationId &&
                (p.StreetAddress.ToLower().Contains(searchTerm) ||
                 p.City.ToLower().Contains(searchTerm) ||
                 p.State.ToLower().Contains(searchTerm) ||
                 p.ZipCode.Contains(searchTerm) ||
                 p.PropertyType.ToLower().Contains(searchTerm) ||
                 p.PropertyStatus.ToLower().Contains(searchTerm)))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    // Methods for PropertyInteractionCount

    public async Task<PropertyInteractionCount> GetInteractionCountsAsync(int propertyId)
    {
        var counts = await _context.PropertyInteractionCounts
            .FirstOrDefaultAsync(ic => ic.PropertyId == propertyId);

        if (counts == null)
        {
            // Create a new record if none exists
            counts = new PropertyInteractionCount
            {
                PropertyId = propertyId,
                CallAttempts = 0,
                DirectMailAttempts = 0,
                SMSAttempts = 0,
                RVMAttempts = 0
            };

            _context.PropertyInteractionCounts.Add(counts);
            await _context.SaveChangesAsync();
        }

        return counts;
    }

    public async Task<PropertyInteractionCount> IncrementInteractionCountAsync(
        int propertyId,
        string interactionType)
    {
        var counts = await GetInteractionCountsAsync(propertyId);

        switch (interactionType.ToLower())
        {
            case "call":
            case "calls":
                counts.CallAttempts++;
                break;
            case "directmail":
                counts.DirectMailAttempts++;
                break;
            case "sms":
                counts.SMSAttempts++;
                break;
            case "rvm":
                counts.RVMAttempts++;
                break;
            default:
                throw new ArgumentException("Invalid interaction type", nameof(interactionType));
        }

        counts.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Record the communication (can be implemented later)
        // await RecordPropertyCommunicationAsync(propertyId, null, interactionType);

        return counts;
    }

    // Methods for PropertyNote (Message Board)

    public async Task<IEnumerable<PropertyNote>> GetPropertyNotesAsync(int propertyId)
    {
        return await _context.PropertyNotes
            .Where(n => n.PropertyId == propertyId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<PropertyNote> AddPropertyNoteAsync(PropertyNote note)
    {
        await _context.PropertyNotes.AddAsync(note);
        await _context.SaveChangesAsync();
        return note;
    }

    public async Task<PropertyNote> UpdatePropertyNoteAsync(PropertyNote note)
    {
        _context.PropertyNotes.Update(note);
        await _context.SaveChangesAsync();
        return note;
    }

    public async Task DeletePropertyNoteAsync(int noteId)
    {
        var note = await _context.PropertyNotes.FindAsync(noteId);
        if (note != null)
        {
            _context.PropertyNotes.Remove(note);
            await _context.SaveChangesAsync();
        }
    }

    // Methods for PropertyDocument

    public async Task<IEnumerable<PropertyDocument>> GetPropertyDocumentsAsync(int propertyId)
    {
        return await _context.PropertyDocuments
            .Where(d => d.PropertyId == propertyId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<PropertyDocument> AddPropertyDocumentAsync(PropertyDocument document)
    {
        await _context.PropertyDocuments.AddAsync(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task DeletePropertyDocumentAsync(int documentId)
    {
        var document = await _context.PropertyDocuments.FindAsync(documentId);
        if (document != null)
        {
            _context.PropertyDocuments.Remove(document);
            await _context.SaveChangesAsync();
        }
    }

    // Contact-Property relationship methods

    public async Task<IEnumerable<Property>> GetPropertiesByContactIdAsync(int contactId)
    {
        return await _context.Properties
            .Where(p => p.OwnerContactId == contactId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    // Implementation of base repository methods

    public async Task<int> AddAsync(Property property)
    {
        await _context.Properties.AddAsync(property);
        await _context.SaveChangesAsync();
        return property.Id;
    }

    public async Task UpdateAsync(Property property)
    {
        property.UpdatedAt = DateTime.UtcNow;
        _context.Properties.Update(property);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var property = await _context.Properties.FindAsync(id);
        if (property != null)
        {
            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Properties.AnyAsync(p => p.Id == id);
    }
}