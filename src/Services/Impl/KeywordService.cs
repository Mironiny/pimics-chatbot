using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PimBot.Dto;
using PimBot.Repositories;

namespace PimBot.Services.Impl
{
    public class KeywordService : IKeywordService
    {
        private IKeywordRepository _keywordRepository;

        public KeywordService(IKeywordRepository keywordRepository)
        {
            _keywordRepository = keywordRepository;
        }

        /// <summary>
        /// Get all keywords.
        /// </summary>
        /// <returns>All keywords.</returns>
        public async Task<IEnumerable<PimKeyword>> GetAllKeywordsAsync() => await _keywordRepository.GetAll();

        /// <summary>
        /// Get all keywords by group by item code.
        /// </summary>
        /// <returns>Keywords.</returns>
        public async Task<Dictionary<string, List<PimKeyword>>> GetAllKeywordsGroupByItemCodeAsync()
        {
            var pimKeywords = await GetAllKeywordsAsync();
            return pimKeywords
                .GroupBy(i => i.Code)
                .ToDictionary(group => group.Key, group => group.ToList());
        }
    }
}
