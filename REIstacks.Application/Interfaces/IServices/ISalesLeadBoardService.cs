using REIstacks.Application.Contracts.Requests;

namespace REIstacks.Application.Interfaces.IServices;
public interface ISalesLeaderBoardService
{
    Task<IEnumerable<SalesLeaderBoardRequest>> GetLeaderBoardAsync(string organizationId);
}
