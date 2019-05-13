// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date:
// ===

namespace PimBot.Services.Impl
{
    /// <summary>
    /// Container class for service instances (implementation).
    /// </summary>
    public class PimBotServiceProvider : IPimbotServiceProvider
    {
        public PimBotServiceProvider(
            IItemService itemservice,
            IKeywordService keywordService,
            IFeatureService featureService,
            ICategoryService categoryService)
        {
            ItemService = itemservice;
            FeatureService = featureService;
            IKeywordService = keywordService;
            CategoryService = categoryService;
        }

        public IItemService ItemService { get; set; }

        public IFeatureService FeatureService { get; set; }

        public ICategoryService CategoryService { get; set; }

        public IKeywordService IKeywordService { get; set; }
    }
}
