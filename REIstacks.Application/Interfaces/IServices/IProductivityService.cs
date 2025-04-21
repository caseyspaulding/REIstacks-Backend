using REIstacks.Application.Contracts.Requests;

namespace REIstacks.Application.Interfaces.IServices;
public interface IProductivityService
{
    /// <summary>
    /// Returns the four productivity numbers (and their targets) for the given tenant/user.
    /// </summary>
    Task<ProductivityDto> GetProductivityAsync(string organizationId, Guid profileId);
}

