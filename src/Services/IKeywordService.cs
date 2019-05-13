// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date:
// ===

using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dto;

namespace PimBot.Services
{
    /// <summary>
    /// Service for handling keywords (api).
    /// </summary>
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
