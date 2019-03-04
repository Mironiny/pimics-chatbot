using PimBotDp.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
