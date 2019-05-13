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
    /// Class responsible for getting categories (api).
    /// </summary>
    public interface ICategoryRepository
    {
        Task<IEnumerable<PimItemGroup>> GetAllItemGroup();

        Task<IEnumerable<PimProductGroup>> GetAllProductGroup();

    }
}
