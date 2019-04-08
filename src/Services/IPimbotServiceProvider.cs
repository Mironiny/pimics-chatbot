using PimBot.Service;

namespace PimBotDp.Services
{
    public interface IPimbotServiceProvider
    {
        IItemService ItemService { get; set; }

        IFeatureService FeatureService { get; set; }

        ICategoryService CategoryService { get; set; }

        IKeywordService IKeywordService { get; set; }
    }
}
