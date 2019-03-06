using System.Threading.Tasks;

namespace PimBot.Service
{
    public interface ISmallTalkService
    {
        Task<string> GetSmalltalkAnswer(string inputMessage);
    }
}
