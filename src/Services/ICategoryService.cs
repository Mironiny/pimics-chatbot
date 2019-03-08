using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.State;

namespace PimBot.Service
{
    public interface ICategoryService
    {
        Task<IEnumerable<PimItemGroup>> GetAllItemGroupAsync();

        Task<IEnumerable<PimProductGroup>> GetAllProductGroupAsync();

        Task<IEnumerable<string>> GetItemGroupIdsByDescription(string description);
    }
}
