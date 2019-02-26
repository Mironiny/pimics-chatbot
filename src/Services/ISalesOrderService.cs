using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PimBotDp.State;

namespace PimBot.Service
{
    public interface ISalesOrderService
    {
        Task<bool> CreateOrder(CustomerState customerState);
    }
}
