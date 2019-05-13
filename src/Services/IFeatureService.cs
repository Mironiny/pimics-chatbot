// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 19.3.2019
// ===

using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dto;

namespace PimBot.Services
{
    /// <summary>
    /// Service for handling features (api).
    /// </summary>
    public interface IFeatureService
    {
        /// <summary>
        /// Get all feature by item.
        /// </summary>
        /// <returns>Features.</returns>
        Task<Dictionary<string, List<PimFeature>>> GetAllFeaturesByItemAsync();

        /// <summary>
        /// Get all features by no.
        /// </summary>
        /// <param name="no">No.</param>
        /// <returns>Features.</returns>
        Task<List<PimFeature>> GetFeaturesByNoAsync(string no);
    }
}
