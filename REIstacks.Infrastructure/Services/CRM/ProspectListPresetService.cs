using Microsoft.EntityFrameworkCore;
using REIstacks.Application.Interfaces.IServices;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Domain.Entities.Properties;
using REIstacks.Infrastructure.Data;
using System.Text.Json;

namespace REIstacks.Infrastructure.Services.CRM;
public class ProspectListPresetService : IProspectListPresetService
{
    private readonly AppDbContext _context;

    public ProspectListPresetService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProspectListPreset>> GetPresetsAsync(string organizationId)
    {
        return await _context.ProspectListPresets
            .Where(p => p.OrganizationId == organizationId || p.IsSystemPreset)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<ProspectListPreset> GetPresetByIdAsync(int id)
    {
        return await _context.ProspectListPresets.FindAsync(id);
    }

    public async Task<int> CreatePresetAsync(ProspectListPreset preset)
    {
        _context.ProspectListPresets.Add(preset);
        await _context.SaveChangesAsync();
        return preset.Id;
    }

    public async Task UpdatePresetAsync(ProspectListPreset preset)
    {
        _context.Entry(preset).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeletePresetAsync(int id)
    {
        var preset = await _context.ProspectListPresets.FindAsync(id);
        if (preset == null)
            throw new KeyNotFoundException($"Preset with ID {id} not found");

        _context.ProspectListPresets.Remove(preset);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Property>> GetPropertiesByPresetAsync(int presetId, string organizationId)
    {
        var preset = await _context.ProspectListPresets.FindAsync(presetId);
        if (preset == null)
            throw new KeyNotFoundException($"Preset with ID {presetId} not found");

        // Deserialize filter criteria
        var criteria = JsonSerializer.Deserialize<PropertyFilterCriteria>(preset.FilterCriteria);

        // Start with base query for org properties
        var query = _context.Properties.Where(p => p.OrganizationId == organizationId);

        // Apply filters based on criteria
        if (criteria.IsAbsenteeOwner == true)
        {
            // Logic to identify absentee owners (property address != owner address)
            query = query.Where(p => p.OwnerContact != null &&
                p.StreetAddress != p.OwnerContact.StreetAddress);
        }

        if (criteria.IsVacant == true)
        {
            // Logic for vacant properties
            query = query.Where(p => p.PropertyStatus == "Vacant");
        }

        // Apply all other filters as needed...

        return await query.ToListAsync();
    }
}