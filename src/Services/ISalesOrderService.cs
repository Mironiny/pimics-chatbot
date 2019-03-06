using PimBot.State;
using System.Threading.Tasks;

namespace PimBot.Service
{
    public interface ISalesOrderService
    {
        Task<bool> CreateOrder(CustomerState customerState);
    }
}
