using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dialogs.FindItem;
using PimBot.State;

namespace PimBot.Service
{
    public interface IItemService
    {
        /// <summary>
        /// Get all items by match.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        Task<IEnumerable<PimItem>> GetAllItemsByMatchAsync(string description); //tested

        /// <summary>
        /// Find item by No.
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        Task<PimItem> FindItemByNo(string no); //tested

        /// <summary>
        /// Get features to ask sorted by important of asking.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<List<FeatureToAsk>> GetAllFeaturesToAsk(IEnumerable<PimItem> items);

        /// <summary>
        /// Filter items by feature
        /// </summary>
        /// <param name="items"></param>
        /// <param name="featureToAsk"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<IEnumerable<PimItem>> FilterItemsByFeature(
            IEnumerable<PimItem> items,
            FeatureToAsk featureToAsk,
            string value,
            int index = -1);

        /// <summary>
        /// Find Similar items by description.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        Task<string> FindSimilarItemsByDescription(string description);

        /// <summary>
        /// Get all items by category //
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        IEnumerable<PimItemGroup> GetAllItemsCategory(IEnumerable<PimItem> items);

        /// <summary>
        /// Get image URL to item in Base64 format. When null is returned, that means that item doesn't contain image.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<string> GetImageUrl(PimItem item);
    }
}
