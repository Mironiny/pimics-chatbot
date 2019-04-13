using System.Threading.Tasks;

namespace PimBot.Repositories
{
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
