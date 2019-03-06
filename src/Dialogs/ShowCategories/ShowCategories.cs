using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.BotBuilderSamples;
using PimBot.Service;
using PimBot.Service.Impl;
using PimBot.Services.Impl;
using PimBot.State;

namespace PimBot.Dialogs.AddItem
{
    public class ShowCategoriesDialog : ComponentDialog
    {
        public const string Name = "Show_categories";

        private readonly IItemService _itemService = new ItemService();
        private readonly ICategoryService _categoryService = new CategoryService();
        private readonly BotServices _services;
        private IStatePropertyAccessor<OnTurnState> _onTurnAccessor;

        public ShowCategoriesDialog(BotServices services, IStatePropertyAccessor<OnTurnState> onTurnAccessor)
            : base(Name)
        {
            _services = services;
            _onTurnAccessor = onTurnAccessor;

            // Add dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                InitializeStateStepAsync,
            };
            AddDialog(new WaterfallDialog(
                "start",
                waterfallSteps));
        }

        private async Task<DialogTurnResult> InitializeStateStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var onTurnProperty = await _onTurnAccessor.GetAsync(context, () => new OnTurnState());

            var categories = await _categoryService.GetAllItemGroupAsync();

            await context.SendActivityAsync(GetPritableCategories(categories));
            return await stepContext.EndDialogAsync();
        }

        private string GetPritableCategories(IEnumerable<PimItemGroup> categories)
        {
            var categoriesToPrint = categories.Where(i => i.Description.Any());
            string result = Messages.ShowCategoriesAvaliableCategories + Environment.NewLine;
            foreach (var category in categoriesToPrint)
            {
                result += category.Description + Environment.NewLine;
            }

            return result;
        }
    }
}
