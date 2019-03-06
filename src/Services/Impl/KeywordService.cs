using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PimBot.Service;
using PimBot.State;

namespace PimBot.Services.Impl
{
    public class KeywordService : IKeywordService
    {
        public async Task<Dictionary<string, List<PimKeyword>>> GetAllKeywordsByItemAsync()
        {
            var client = ODataClientSingleton.Get();
            var keywords = await client
                .For(Constants.KeywordsServiceEndpointName)
                .FindEntriesAsync();

            var pimKeywords = MapKeywords(keywords);

            return pimKeywords
                .GroupBy(i => i.Code)
                .ToDictionary(group => group.Key, group => group.ToList());
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
