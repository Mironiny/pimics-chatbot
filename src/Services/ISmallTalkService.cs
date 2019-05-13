// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date:
// ===

using System.Threading.Tasks;

namespace PimBot.Services
{
    /// <summary>
    /// Smalltalk service (api).
    /// </summary>
    public interface ISmallTalkService
    {
        /// <summary>
        /// Get small talkresponse.
        /// </summary>
        /// <param name="inputMessage">Input message.</param>
        /// <returns>Smalltalk Message.</returns>
        Task<string> GetSmalltalkAnswer(string inputMessage);
    }
}
