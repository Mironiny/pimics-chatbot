using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using PimBot.Services;
using PimBot.Services.Impl;
using PimBot.State;

namespace PimBot.Dialogs
{
    public class ShowOrdersDialog : ComponentDialog
    {
        public const string Name = "Show_orders";
        private readonly ICustomerService _customerService = new CustomerService();

        private readonly IItemService _itemService;
        private readonly BotServices _services;
        private IStatePropertyAccessor<OnTurnState> _onTurnAccessor;

        // Prompts names
        private const string CountPrompt = "countPrompt";

        public ShowOrdersDialog(BotServices services, IStatePropertyAccessor<OnTurnState> onTurnAccessor, IPimbotServiceProvider provider)
            : base(Name)
        {
            _services = services;
            _onTurnAccessor = onTurnAccessor;
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
            var customer = await _customerService.GetCustomerStateById(stepContext.Context.Activity.From.Id);
            if (customer.Orders != null && customer.Orders.Count > 0)
            {
                foreach (var order in customer.Orders)
                {
                    var response = stepContext.Context.Activity.CreateReply();
                    response.Attachments = new List<Attachment>() { CreateAdaptiveCardUsingSdk(order) };
                    await context.SendActivityAsync(response);
                }
            }
            else
            {
                await context.SendActivityAsync(Messages.ShowOrdersNoOrder);
            }

            return await stepContext.EndDialogAsync();
        }

        private Attachment CreateAdaptiveCardUsingSdk(OrderState order)
        {
            var card = new AdaptiveCard();
            card.Body.Add(new AdaptiveTextBlock()
            {
                Text = $"**Status**: processing",
                Size = AdaptiveTextSize.Small,
                Weight = AdaptiveTextWeight.Bolder,
            });
            card.Body.Add(new AdaptiveTextBlock()
            {
                Text = $"**Created at**: {order.CreateDateTime}",
                Size = AdaptiveTextSize.Small,
                Weight = AdaptiveTextWeight.Bolder,
            });
            card.Body.Add(new AdaptiveTextBlock()
            {
                Text = $"{ShowCartDialog.GetPrintableCart(order, "order")}",
                Size = AdaptiveTextSize.Small,
                Weight = AdaptiveTextWeight.Bolder,
            });

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }

    }
}
