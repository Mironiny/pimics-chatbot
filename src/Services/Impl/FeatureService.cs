using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PimBot.Repositories;
using PimBot.Service;
using PimBot.State;

namespace PimBot.Services.Impl
{
    public class FeatureService : IFeatureService
    {
        private IFeaturesRepository _featuresRepository;

        public FeatureService(IFeaturesRepository featuresRepository)
        {
            _featuresRepository = featuresRepository;
        }

        public async Task<Dictionary<string, List<PimFeature>>> GetAllFeaturesByItemAsync()
        {
            var pimFeatures = await _featuresRepository.GetAll();

            return pimFeatures
                .GroupBy(i => i.Code)
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        public async Task<List<PimFeature>> GetFeaturesByNoAsync(string no)
        {
            var pimFeatures = await _featuresRepository.GetAll();
            return pimFeatures.Where(i => i.Code == no).ToList();
        }
    }
}
