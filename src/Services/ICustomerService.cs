using System.Threading.Tasks;
using PimBot.State;

namespace PimBot.Services
{
    public interface ICustomerService
    {
        Task<CustomerState> GetCustomerStateById(string id);

        Task UpdateCustomerState(CustomerState customerState);
    }
}
