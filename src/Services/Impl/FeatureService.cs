﻿// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 19.03.2019
// ===

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PimBot.Dto;
using PimBot.Repositories;

namespace PimBot.Services.Impl
{
    /// <summary>
    /// Service for handling features (implementation).
    /// </summary>
    public class FeatureService : IFeatureService
    {
        private IFeaturesRepository _featuresRepository;

        public FeatureService(IFeaturesRepository featuresRepository)
        {
            _featuresRepository = featuresRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PimFeature>> GetAllFeatures() => await _featuresRepository.GetAll();

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
        public async Task<List<PimFeature>> GetFeaturesByNoAsync(string no, IEnumerable<PimFeature> pimFeatures)
        {
            return pimFeatures.Where(i => i.Code == no).ToList();
        }
    }
}
