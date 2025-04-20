using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Domain.Entities.Properties;
using REIstacks.Infrastructure.Data;

namespace REIstacks.Infrastructure.Services.CRM;
public class TagService : ITagService
{
    private readonly AppDbContext _context;

    public TagService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tag>> GetTagsByTypeAsync(string type, string organizationId)
    {
        return await _context.Tags
            .Where(t => t.TagType == type && t.OrganizationId == organizationId)
            .ToListAsync();
    }

    public async Task<int> CreateTagAsync(Tag tag)
    {
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return tag.Id;
    }

    public async Task DeleteTagAsync(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
            throw new KeyNotFoundException($"Tag with ID {id} not found");

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
    }

    // implementing in TagService
    public async Task AddTagToPropertyAsync(int propertyId, int tagId, string organizationId)
    {
        var propertyTag = new PropertyTag
        {
            PropertyId = propertyId,
            TagId = tagId,
            OrganizationId = organizationId,   // comes from the claims
            TaggedAt = DateTime.UtcNow
        };

        _context.PropertyTags.Add(propertyTag);
        await _context.SaveChangesAsync();
    }

    public async Task AddTagToContactAsync(int contactId, int tagId)
    {
        var contactTag = new ContactTag
        {
            ContactId = contactId,
            TagId = tagId
        };

        _context.ContactTags.Add(contactTag);
        await _context.SaveChangesAsync();
    }

    public async Task AddTagToPhoneAsync(int phoneId, int tagId)
    {
        var phoneTag = new PhoneTag
        {
            PhoneId = phoneId,
            TagId = tagId
        };

        _context.PhoneTags.Add(phoneTag);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveTagFromPropertyAsync(int propertyId, int tagId)
    {
        var propertyTag = await _context.PropertyTags
            .FirstOrDefaultAsync(pt => pt.PropertyId == propertyId && pt.TagId == tagId);

        if (propertyTag != null)
        {
            _context.PropertyTags.Remove(propertyTag);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveTagFromContactAsync(int contactId, int tagId)
    {
        var contactTag = await _context.ContactTags
            .FirstOrDefaultAsync(ct => ct.ContactId == contactId && ct.TagId == tagId);

        if (contactTag != null)
        {
            _context.ContactTags.Remove(contactTag);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveTagFromPhoneAsync(int phoneId, int tagId)
    {
        var phoneTag = await _context.PhoneTags
            .FirstOrDefaultAsync(pt => pt.PhoneId == phoneId && pt.TagId == tagId);

        if (phoneTag != null)
        {
            _context.PhoneTags.Remove(phoneTag);
            await _context.SaveChangesAsync();
        }
    }
}