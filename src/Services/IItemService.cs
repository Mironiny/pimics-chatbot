using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dialogs.FindItem;
using PimBot.State;

namespace PimBot.Service
{
    public interface IItemService
    {
        Task<IEnumerable<PimItem>> GetAllItemsAsync(string item);

        /// <summary>
        /// Find item by No. If there is no match return null.
        /// </summary>
        Task<PimItem> FindItemByNo(string no);

        Task<List<FeatureToAsk>> GetAllAttributes(IEnumerable<PimItem> items);

        Task<IEnumerable<PimItem>> FilterItemsByFeature(IEnumerable<PimItem> items, FeatureToAsk featureToAsk,
            string value, int index = -1);
    }
}
