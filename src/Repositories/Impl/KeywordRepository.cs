using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Service;
using PimBot.State;

namespace PimBot.Repositories.Impl
{
    public class KeywordRepository : IKeywordRepository
    {
        public async Task<IEnumerable<PimKeyword>> GetAll()
        {
            var client = ODataClientSingleton.Get();
            var keywords = await client
                .For(Constants.Company).Key(Constants.CompanyName)
                .NavigateTo(Constants.KeywordsServiceEndpointName)
                .FindEntriesAsync();

            return MapKeywords(keywords);
        }

        private IEnumerable<PimKeyword> MapKeywords(IEnumerable<IDictionary<string, object>> keywords)
        {
            List<PimKeyword> pimKeywords = new List<PimKeyword>();
            foreach (var keyword in keywords)
            {
                var pimItem = MapPimKeyword(keyword);
                pimKeywords.Add(pimItem);
            }

            return pimKeywords;
        }

        private PimKeyword MapPimKeyword(IDictionary<string, object> keyword)
        {
            var pimKeyword = new PimKeyword
            {
                Source = (string)keyword["Source"],
                Code = (string)keyword["Code"],
                Group_System_Number = (string)keyword["Group_System_Number"],
                Source_Type = (string)keyword["Source_Type"],
                Source_Code = (string)keyword["Source_Code"],
                Line_No = (int)keyword["Line_No"],
                Keyword_ID = (string)keyword["Keyword_ID"],
                Keyword = (string)keyword["Keyword"],
                Print = (string)keyword["Print"],
                Classification_System = (string)keyword["Classification_System"],
                Classification_System_Version = (string)keyword["Classification_System_Version"],
                Usage_Type_Code = (string)keyword["Usage_Type_Code"],
                Inherited = (bool)keyword["Inherited"],
            };
            return pimKeyword;
        }
    }
}
