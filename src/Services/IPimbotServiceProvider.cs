using PimBot.Service;

namespace PimBotDp.Services
{
    /// <summary>
    /// Service provider serves as DI container.
    /// </summary>
    public interface IPimbotServiceProvider
    {
        IItemService ItemService { get; set; }

        IFeatureService FeatureService { get; set; }

        ICategoryService CategoryService { get; set; }

        IKeywordService IKeywordService { get; set; }
    }
}
