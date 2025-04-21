using REIstacks.Domain.Entities.Deals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REIstacks.Application.Interfaces.IServices;
public interface IDealService
{
    Task<IEnumerable<Deal>> GetDealsAsync(string organizationId);
    Task<Deal?> GetDealByIdAsync(int id, string organizationId);
    Task<Deal> CreateDealAsync(Deal deal);
    Task<bool> UpdateDealAsync(Deal deal, string organizationId);
    Task<bool> DeleteDealAsync(int id, string organizationId);
}
