using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dialogs.FindItem;
using PimBot.State;

namespace PimBot.Service
{
    public interface IItemService
    {
        /// <summary>
        /// Method returns every items which match item description or category description or has keywords with same value.
        /// </summary>
        /// <param name="description">Value to look for in item description, category and keywords</param>
        /// <returns>All items by match.</returns>
        Task<IEnumerable<PimItem>> GetAllItemsByMatchAsync(string description);

        /// <summary>
        /// Find item by NO number.
        /// </summary>
        /// <param name="no">No.</param>
        /// <returns>Item.</returns>
        Task<PimItem> FindItemByNo(string no);

        /// <summary>
        /// Method choose from inputs items features to ask. It's sorts features by order and computed information gain.
        /// </summary>
        /// <param name="items">List of items.</param>
        /// <returns>Ordered features to ask.</returns>
        Task<List<FeatureToAsk>> GetAllFeaturesToAsk(IEnumerable<PimItem> items);

        /// <summary>
        /// Filter items by feature.
        /// </summary>
        /// <param name="items">Input items to filter.</param>
        /// <param name="featureToAsk">Selected feature which is was asked.</param>
        /// <param name="value">Value which user put. If type of feature is numeric, then this value will be median.</param>
        /// <param name="index">Valid only in numerics type. If user select under median, the index is 0 otherwise 1.</param>
        /// <returns>Filtered items.</returns>
        Task<IEnumerable<PimItem>> FilterItemsByFeature(
            IEnumerable<PimItem> items,
            FeatureToAsk featureToAsk,
            string value,
            int index = -1);

        /// <summary>
        /// Find similar items in by description using Levenshtein distance.
        /// </summary>
        /// <param name="description">Description</param>
        /// <returns>Description of similar item.</returns>
        Task<string> FindSimilarItemsByDescription(string description);

        /// <summary>
        /// Get all categories which match input items.
        /// </summary>
        /// <param name="items">List of items.</param>
        /// <returns>Items group.</returns>
        IEnumerable<PimItemGroup> GetAllItemsCategory(IEnumerable<PimItem> items);

        /// <summary>
        /// Get image Url, try create base64 URI. If the size is too big, resize until its gonna be OK.
        /// </summary>
        /// <param name="item">Items.</param>
        /// <returns>Image url</returns>
        Task<string> GetImageUrl(PimItem item);
    }
}
