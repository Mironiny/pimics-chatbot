using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dto;

namespace PimBot.Services
{
    public interface ICategoryService
    {
        /// <summary>
        /// Get item groups by No.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <returns>Task.</returns>
        Task GetItemGroupsByNo(PimItem item);

        /// <summary>
        /// Get all items by group.
        /// </summary>
        /// <returns>PimItemGroup.</returns>
        Task<IEnumerable<PimItemGroup>> GetAllItemGroupAsync();

        /// <summary>
        /// Get all product group.
        /// </summary>
        /// <returns>Product group.</returns>
        Task<IEnumerable<PimProductGroup>> GetAllProductGroupAsync();

        /// <summary>
        /// Get item group ids by description.
        /// </summary>
        /// <param name="description">Description.</param>
        /// <returns>Items groups.</returns>
        Task<IEnumerable<string>> GetItemGroupIdsByDescription(string description);
    }
}
