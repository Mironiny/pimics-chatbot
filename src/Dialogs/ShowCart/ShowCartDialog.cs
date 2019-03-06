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
using PimBot.State;

namespace PimBot.Dialogs.AddItem
{
    public class ShowCartDialog : ComponentDialog
    {
        public const string Name = "Show_cart";

        private readonly IItemService _itemService = new ItemService();
        private readonly BotServices _services;
        private IStatePropertyAccessor<OnTurnState> _onTurnAccessor;
        private IStatePropertyAccessor<CartState> _cartStateAccessor;

        // Prompts names
        private const string CountPrompt = "countPrompt";

        public ShowCartDialog(BotServices services, IStatePropertyAccessor<OnTurnState> onTurnAccessor, IStatePropertyAccessor<CartState> cartStateAccessor)
            : base(Name)
        {
            _services = services;
            _onTurnAccessor = onTurnAccessor;
            _cartStateAccessor = cartStateAccessor;

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
            var cartState =
                await _cartStateAccessor.GetAsync(context, () => new CartState());
            if (cartState.Items == null || cartState.Items.Count == 0)
            {
                await context.SendActivityAsync(Messages.EmptyCart);
                await context.SendActivityAsync(Messages.SuggestHelp);
            }
            else
            {
                string itemsInCart = Messages.ShowCartTitle + Environment.NewLine;
                foreach (var item in cartState.Items)
                {
                    if (item.Count > 0)
                    {
                        itemsInCart += $"* **{Messages.Number}**: {item.No} {Environment.NewLine}";
                        itemsInCart += $"**{Messages.Description}**: {item.Description} {Environment.NewLine}";
                        itemsInCart += $"**{Messages.Count}**: {item.Count} {item.Base_Unit_of_Measure} {Environment.NewLine}";
                        itemsInCart += $"**{Messages.UnitPrice}**: {item.Unit_Price} {Environment.NewLine}";
                    }
                }

                itemsInCart += $"\n\n {Messages.ShowCartFullPrice} {GetFullPrice(cartState.Items)}";

                await context.SendActivityAsync(itemsInCart);
                await context.SendActivityAsync(Messages.ShowCartAfter);
            }

            return await stepContext.EndDialogAsync();
        }

        private decimal GetFullPrice(List<PimItem> items)
        {
            return items.Sum(item => item.Unit_Price * item.Count);
        }
    }
}
