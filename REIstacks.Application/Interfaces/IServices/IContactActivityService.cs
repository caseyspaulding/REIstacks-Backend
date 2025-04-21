using REIstacks.Application.Contracts.Requests;
using REIstacks.Domain.Entities.CRM;

namespace REIstacks.Application.Interfaces.IServices;
public interface IContactActivityService
{
    Task<IEnumerable<ContactActivity>> GetForContactAsync(
        int contactId,
        string organizationId,
        ActivityType? filterType = null,
        Guid? filterUserId = null,
        DateTime? from = null,
        DateTime? to = null
    );

    Task<ContactActivity> LogAsync(ContactActivity act);

    Task<IEnumerable<UserSummaryDto>> GetActingUsersForContactAsync(
        int contactId,
        string organizationId
    );
}
