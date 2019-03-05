using System.Collections.Generic;
using System.Threading.Tasks;
using PimBotDp.State;

namespace PimBot.Service
{
    public interface IKeywordService
    {
        Task<Dictionary<string, List<PimKeyword>>> GetAllKeywordsByItemAsync();
    }
}
