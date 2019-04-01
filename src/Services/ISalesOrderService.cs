using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.State;

namespace PimBot.Service
{
    public interface ISalesOrderService
    {
        Task<bool> CreateOrder(CustomerState customerState);

        Task<IEnumerable<SalesOrder>> GetSalesOrderByCustomer(string customerNo);
    }
}
