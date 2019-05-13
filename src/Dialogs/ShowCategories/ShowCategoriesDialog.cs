// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date:
// ===============================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.BotBuilderSamples;
using PimBot.Dto;
using PimBot.Services;
using PimBot.State;

namespace PimBot.Dialogs
{
    /// <summary>
    /// Class represents show categories dialogs.
    /// </summary>
    public class ShowCategoriesDialog : ComponentDialog
    {
        public const string Name = "Show_categories";

        private readonly IItemService _itemService;
        private readonly ICategoryService _categoryService;
        private readonly BotServices _services;
        private IStatePropertyAccessor<OnTurnState> _onTurnAccessor;

        public ShowCategoriesDialog(BotServices services, IStatePropertyAccessor<OnTurnState> onTurnAccessor, IPimbotServiceProvider provider)
            : base(Name)
        {
            _services = services;
            _onTurnAccessor = onTurnAccessor;
            _itemService = provider.ItemService;
            _categoryService = provider.CategoryService;

            // Add dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                InitializeStateStepAsync,
            };
            AddDialog(new WaterfallDialog(
                "start",
                waterfallSteps));
        }

        public static string GetPritableGroup(IEnumerable<PimGroup> categories)
        {
            var categoriesToPrint = categories.Where(i => i.Description.Any());
            string result = Messages.ShowCategoriesAvaliableCategories + Environment.NewLine;
            foreach (var category in categoriesToPrint)
            {
                result += category.Description + Environment.NewLine;
            }

            return result;
        }

        private async Task<DialogTurnResult> InitializeStateStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var onTurnProperty = await _onTurnAccessor.GetAsync(context, () => new OnTurnState());

            var categories = await _categoryService.GetAllProductGroupAsync();

            await context.SendActivityAsync(GetPritableGroup(categories));
            return await stepContext.EndDialogAsync();
        }
    }
}
