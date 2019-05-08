using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dto;

namespace PimBot.Service
{
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
