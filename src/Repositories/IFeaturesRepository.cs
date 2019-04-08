using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.State;

namespace PimBot.Repositories
{
    public interface IFeaturesRepository
    {
        Task<IEnumerable<PimFeature>> GetAll();
    }
}
