// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 7.4.2019
// ===

using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dto;

namespace PimBot.Repositories
{
    /// <summary>
    /// Class responsible for getting items (api).
    /// </summary>
    public interface IItemRepository
    {
        Task<IEnumerable<PimItem>> GetAll();
    }
}
