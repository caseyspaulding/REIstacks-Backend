using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Application.Interfaces.IServices;
public interface IPhoneStatusService
{
    Task<ContactPhone> UpdatePhoneStatusAsync(int phoneId, string statusId);
    Task<IEnumerable<PhoneStatus>> GetPhoneStatusesAsync();
}