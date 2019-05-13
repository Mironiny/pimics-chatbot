// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 19.03.2019
// ===

using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dto;
using PimBot.State;

namespace PimBot.Services
{
    /// <summary>
    /// Service for handling sales order (api).
    /// </summary>
    public interface ISalesOrderService
    {
        Task<bool> CreateOrder(CustomerState customerState);

        Task<IEnumerable<SalesOrder>> GetSalesOrderByCustomer(string customerNo);
    }
}
