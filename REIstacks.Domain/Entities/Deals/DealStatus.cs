using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REIstacks.Domain.Entities.Deals;
public enum DealStatus
{
    Lead,
    Negotiating,
    UnderContract,
    Closed,
    Dead
}
