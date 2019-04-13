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
        /// <param name="description"></param>
        /// <returns></returns>
        Task<IEnumerable<PimItem>> GetAllItemsByMatchAsync(string description); //tested

        /// <summary>
        /// Find description by No. If there is no match return null.
        /// </summary>
        Task<PimItem> FindItemByNo(string no); //tested

        /// <summary>
        /// Get features to ask sorted by important of asking.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<List<FeatureToAsk>> GetAllFeaturesToAsk(IEnumerable<PimItem> items); //tested

        /// <summary>
        /// Filter items by feature
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
        Task<string> FindSimilarItemsByDescription(string description); //tested

        /// <summary>
        /// Get all items by category //
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        IEnumerable<PimItemGroup> GetAllItemsCategory(IEnumerable<PimItem> items); //tested

        Task<string> GetImageUrl(PimItem item);

    }
}
