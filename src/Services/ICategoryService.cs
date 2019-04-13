using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.State;

namespace PimBot.Service
{
    public interface ICategoryService
    {
        /// <summary>
        /// Gets all Item groups by item no.
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns></returns>
        Task GetItemGroupsByNo(PimItem item);

        Task<IEnumerable<PimItemGroup>> GetAllItemGroupAsync();

        Task<IEnumerable<PimProductGroup>> GetAllProductGroupAsync();

        Task<IEnumerable<string>> GetItemGroupIdsByDescription(string description);
    }
}
