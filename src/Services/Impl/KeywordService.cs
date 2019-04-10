using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PimBot.Repositories;
using PimBot.Service;
using PimBot.State;

namespace PimBot.Services.Impl
{
    public class KeywordService : IKeywordService
    {
        private IKeywordRepository _keywordRepository;

        public KeywordService(IKeywordRepository keywordRepository)
        {
            _keywordRepository = keywordRepository;
        }

        public async Task<IEnumerable<PimKeyword>> GetAllKeywordsAsync() => await _keywordRepository.GetAll();

        public async Task<Dictionary<string, List<PimKeyword>>> GetAllKeywordsGroupByItemCodeAsync()
        {
            var pimKeywords = await GetAllKeywordsAsync();
            return pimKeywords
                .GroupBy(i => i.Code)
                .ToDictionary(group => group.Key, group => group.ToList());
        }
    }
}
