using System.Threading.Tasks;
using PimBot.State;

namespace PimBot.Services
{
    public interface ICustomerService
    {
        /// <summary>
        /// Get whole custumer state by id.
        /// </summary>
        /// <param name="id">id.</param>
        /// <returns>Customer state.</returns>
        Task<CustomerState> GetCustomerStateById(string id);

        /// <summary>
        /// Update customer state.
        /// </summary>
        /// <param name="customerState">Customer state.</param>
        /// <returns>Task.</returns>
        Task UpdateCustomerState(CustomerState customerState);
    }
}
