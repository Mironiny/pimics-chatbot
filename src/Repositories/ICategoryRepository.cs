using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.State;

namespace PimBot.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<PimItemGroup>> GetAllItemGroup();

        Task<IEnumerable<PimProductGroup>> GetAllProductGroup();

    }
}
