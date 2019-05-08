using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PimBot.Dto;
using PimBot.Repositories;

namespace PimBot.Services.Impl
{
    public class FeatureService : IFeatureService
    {
        private IFeaturesRepository _featuresRepository;

        public FeatureService(IFeaturesRepository featuresRepository)
        {
            _featuresRepository = featuresRepository;
        }

        /// <summary>
        /// Get all feature by item.
        /// </summary>
        /// <returns>Features.</returns>
        public async Task<Dictionary<string, List<PimFeature>>> GetAllFeaturesByItemAsync()
        {
            var pimFeatures = await _featuresRepository.GetAll();

            return pimFeatures
                .GroupBy(i => i.Code)
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        /// <summary>
        /// Get all features by no.
        /// </summary>
        /// <param name="no">No.</param>
        /// <returns>Features.</returns>
        public async Task<List<PimFeature>> GetFeaturesByNoAsync(string no)
        {
            var pimFeatures = await _featuresRepository.GetAll();
            return pimFeatures.Where(i => i.Code == no).ToList();
        }
    }
}
