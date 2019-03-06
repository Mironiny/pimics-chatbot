using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.State;

namespace PimBot.Service
{
    public interface ICategoryService
    {
        Task<IEnumerable<PimItemGroup>> GetAllItemGroupAsync();

        Task<IEnumerable<string>> GetItemGroupIdsByDescription(string description);
    }
}
