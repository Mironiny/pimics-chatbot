using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.State;

namespace PimBot.Service
{
    public interface IKeywordService
    {
        Task<Dictionary<string, List<PimKeyword>>> GetAllKeywordsByItemAsync();
    }
}
