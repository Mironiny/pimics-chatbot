// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 7.4.2019
// ===

using System.Linq;
using System.Threading.Tasks;

namespace PimBot.Repositories.Impl
{
    /// <summary>
    /// Class responsible for getting pictures (implementation).
    /// </summary>
    public class PictureRepository : IPictureRepository
    {
        public async Task<string> GetPictureUrlByPictureDocumentId(string pictureDocumentId)
        {
            var client = ODataClientSingleton.Get();

            var picture = await client
                .For(Constants.Company).Key(Constants.CompanyName)
                .NavigateTo(Constants.PictureEndpointName)
                .Filter($"Number%20eq%20%27{pictureDocumentId}%27")
                .FindEntriesAsync();

            if (picture == null || !picture.Any())
            {
                return null;
            }

            return (string)picture.First()["Content"];
        }
    }
}
