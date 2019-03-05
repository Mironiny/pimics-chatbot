using System.Collections.Generic;
using System.Threading.Tasks;
using PimBotDp.State;

namespace PimBot.Service
{
    public interface IItemService
    {
        Task<IEnumerable<PimItem>> GetAllItemsAsync(string item);

        /// <summary>
        /// Find item by No. If there is no match return null.
        /// </summary>
        Task<PimItem> FindItemByNo(string no);
    }
}
