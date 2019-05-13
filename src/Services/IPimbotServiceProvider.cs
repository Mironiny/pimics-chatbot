// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 7.4.2019
// ===

namespace PimBot.Services
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
