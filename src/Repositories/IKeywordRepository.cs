using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dto;

namespace PimBot.Repositories
{
    public interface IKeywordRepository
    {
        Task<IEnumerable<PimKeyword>> GetAll();
    }
}
