using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.State;

namespace PimBot.Service
{
    public interface IKeywordService
    {
        Task<IEnumerable<PimKeyword>> GetAllKeywordsAsync();

        Task<Dictionary<string, List<PimKeyword>>> GetAllKeywordsByItemAsync();
    }
}
