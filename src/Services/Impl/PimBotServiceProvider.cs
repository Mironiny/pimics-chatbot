using PimBot.Service;

namespace PimBotDp.Services.Impl
{
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
