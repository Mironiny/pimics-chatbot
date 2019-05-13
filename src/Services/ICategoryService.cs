// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 19.3.2019
// ===

using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dto;

namespace PimBot.Services
{
    /// <summary>
    /// Service for handling categories (api).
    /// </summary>
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
