using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dialogs.FindItem;
using PimBot.State;

namespace PimBot.Service
{
    public interface IItemService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<IEnumerable<PimItem>> GetAllItemsAsync(string item);

        /// <summary>
        /// Find item by No. If there is no match return null.
        /// </summary>
        Task<PimItem> FindItemByNo(string no);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<List<FeatureToAsk>> GetAllAttributes(IEnumerable<PimItem> items);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="featureToAsk"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<IEnumerable<PimItem>> FilterItemsByFeature(IEnumerable<PimItem> items, FeatureToAsk featureToAsk,
            string value, int index = -1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        IEnumerable<PimItemGroup> GetAllItemsCategory(IEnumerable<PimItem> items);
    }
}
