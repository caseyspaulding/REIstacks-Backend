using REIstacks.Domain.Entities.CRM;
using REIstacks.Domain.Entities.Properties;

namespace REIstacks.Application.Interfaces.IServices;
public interface IProspectListPresetService
{
    Task<IEnumerable<ProspectListPreset>> GetPresetsAsync(string organizationId);
    Task<ProspectListPreset> GetPresetByIdAsync(int id);
    Task<int> CreatePresetAsync(ProspectListPreset preset);
    Task UpdatePresetAsync(ProspectListPreset preset);
    Task DeletePresetAsync(int id);
    Task<IEnumerable<Property>> GetPropertiesByPresetAsync(int presetId, string organizationId);
}
