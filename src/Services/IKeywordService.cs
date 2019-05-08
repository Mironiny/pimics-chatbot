using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dto;

namespace PimBot.Services
{
    public interface IKeywordService
    {
        /// <summary>
        /// Get all keywords.
        /// </summary>
        /// <returns>All keywords.</returns>
        Task<IEnumerable<PimKeyword>> GetAllKeywordsAsync();

        /// <summary>
        /// Get all keywords by group by item code.
        /// </summary>
        /// <returns>Keywords.</returns>
        Task<Dictionary<string, List<PimKeyword>>> GetAllKeywordsGroupByItemCodeAsync();
    }
}
