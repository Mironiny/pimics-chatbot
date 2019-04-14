using System.Threading.Tasks;

namespace PimBot.Service
{
    public interface ISmallTalkService
    {
        /// <summary>
        /// Get small response.
        /// </summary>
        /// <param name="inputMessage">Input message.</param>
        /// <returns>Smalltalk Message.</returns>
        Task<string> GetSmalltalkAnswer(string inputMessage);
    }
}
