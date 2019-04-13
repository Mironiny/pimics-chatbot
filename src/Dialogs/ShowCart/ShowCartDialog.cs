using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.BotBuilderSamples;
using PimBot.Service;
using PimBot.Services;
using PimBot.Services.Impl;
using PimBot.State;
using PimBotDp.Services;

namespace PimBot.Dialogs.AddItem
{
    public class ShowCartDialog : ComponentDialog
    {
        public const string Name = "Show_cart";

        private readonly IItemService _itemService;
        private readonly BotServices _services;
        private IStatePropertyAccessor<OnTurnState> _onTurnAccessor;
        private IStatePropertyAccessor<CartState> _cartStateAccessor;
        private readonly ICustomerService _customerService = new CustomerService();

        // Prompts names
        private const string CountPrompt = "countPrompt";

        public ShowCartDialog(BotServices services, IStatePropertyAccessor<OnTurnState> onTurnAccessor, IStatePropertyAccessor<CartState> cartStateAccessor, IPimbotServiceProvider provider)
            : base(Name)
        {
            _services = services;
            _onTurnAccessor = onTurnAccessor;
            _cartStateAccessor = cartStateAccessor;
            _itemService = provider.ItemService;

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
            CustomerState customerState =
                await _customerService.GetCustomerStateById(stepContext.Context.Activity.From.Id);

            if (customerState.Cart.Items == null || customerState.Cart.Items.Count == 0)
            {
                await context.SendActivityAsync(Messages.EmptyCart);
                await context.SendActivityAsync(Messages.SuggestHelp);
            }
            else
            {
                var itemsInCart = GetPrintableCart(customerState.Cart);

                await context.SendActivityAsync(itemsInCart);
                await context.SendActivityAsync(Messages.ShowCartAfter);
            }

            return await stepContext.EndDialogAsync();
        }

        public static string GetPrintableCart(CartState cartState, string label = "cart")
        {
            string itemsInCart = Messages.ShowCartTitle + label + ":" + Environment.NewLine;
            foreach (var item in cartState.Items)
            {
                if (item.Count > 0)
                {
                    itemsInCart += $"* **{Messages.Number}**: {item.No} {Environment.NewLine}";
                    itemsInCart += $"**{Messages.Description}**: {item.Description} {Environment.NewLine}";
                    itemsInCart += $"**{Messages.Count}**: {item.Count} {item.Base_Unit_of_Measure} {Environment.NewLine}";
                    itemsInCart += $"**{Messages.UnitPrice}**: {item.Unit_Price} {Environment.NewLine} {Environment.NewLine}";
                }
            }

            itemsInCart += $"{Environment.NewLine} _________________________________ ";
            itemsInCart += $"\n\n {Environment.NewLine} {Messages.ShowCartFullPrice} **{GetFullPrice(cartState.Items)}**";
            return itemsInCart;
        }

        private static decimal GetFullPrice(List<PimItem> items)
        {
            return items.Sum(item => item.Unit_Price * item.Count);
        }
    }
}
