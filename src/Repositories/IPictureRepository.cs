// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 7.4.2019
// ===

using System.Threading.Tasks;

namespace PimBot.Repositories
{
    /// <summary>
    /// Class responsible for getting pictures (api).
    /// </summary>
    public interface IPictureRepository
    {
        /// <summary>
        /// Get Picture URL in Base64 format.
        /// </summary>
        /// <param name="pictureDocumentId"></param>
        /// <returns></returns>
        Task<string> GetPictureUrlByPictureDocumentId(string pictureDocumentId);
    }
}
