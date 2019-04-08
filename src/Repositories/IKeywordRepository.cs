using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.State;

namespace PimBot.Repositories
{
    public interface IKeywordRepository
    {
        Task<IEnumerable<PimKeyword>> GetAll();
    }
}
