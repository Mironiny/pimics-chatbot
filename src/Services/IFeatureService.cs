using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PimBot.State;

namespace PimBot.Service
{
    public interface IFeatureService
    {
        Task<Dictionary<string, List<PimFeature>>> GetAllFeaturesByItemAsync();
    }
}
