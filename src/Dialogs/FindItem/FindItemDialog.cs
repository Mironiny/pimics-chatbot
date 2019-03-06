using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using PimBot.Messages;
using PimBot.Service;
using PimBot.Service.Impl;
using PimBotDp.Constants;
using PimBotDp.State;

namespace PimBotDp.Dialogs.FindItem
{
    public class FindItemDialog : ComponentDialog
    {
        public const string Name = "Find_item";
        private readonly BotServices _services;
        private IStatePropertyAccessor<OnTurnState> _onTurnAccessor;
        private IStatePropertyAccessor<CartState> _cartStateAccessor;
        private readonly IItemService _itemService = new ItemService();

        // Prompts names
        private const string CountPrompt = "countPrompt";

        public FindItemDialog(BotServices services, IStatePropertyAccessor<OnTurnState> onTurnAccessor, IStatePropertyAccessor<CartState> cartStateAccessor)
            : base(Name)
        {
            _services = services;
            _onTurnAccessor = onTurnAccessor;
            _cartStateAccessor = cartStateAccessor;

            // Add dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                InitializeStateStepAsync,
//                PromptForCountStepAsync,
//                ResolveCountAsync,
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
            if (onTurnProperty.Entities[EntityNames.FindItem].Count() > 0)
            {
                var firstEntity = (string)onTurnProperty.Entities[EntityNames.FindItem].First;
          //      await context.SendActivityAsync("Entity is: " + firstEntity);

                var items = await _itemService.GetAllItemsAsync(firstEntity);
                var count = items.Count();
                if (items.Count() == 0)
                {
                    await context.SendActivityAsync($"{Messages.NotFound} **{firstEntity}**");
                }
                else if (items.Count() < 10)
                {
                    foreach (var item in items)
                    {
                        var response = stepContext.Context.Activity.CreateReply();
                        response.Attachments = new List<Attachment>() { CreateAdaptiveCardUsingSdk(item) };
                        await context.SendActivityAsync(response);
                    }

                    await context.SendActivityAsync(Messages.FindItemAddToCart);
                }
                else
                {
                    await context.SendActivityAsync(GetPrintableListItems(items));
                    await context.SendActivityAsync(Messages.FindItemAddToCart);
                }
            }

            return await stepContext.EndDialogAsync();
        }

        private string GetPrintableListItems(IEnumerable<PimItem> items)
        {
            string result = string.Empty;
            foreach (var item in items)
            {
                result += $"**No**: {item.No}, desciption: {item.Description}, unit price: {item.Unit_Price}/{item.Base_Unit_of_Measure} {Environment.NewLine}";
            }

            return result;
        }

        private Attachment CreateAdaptiveCardUsingSdk(PimItem item)
        {
            var card = new AdaptiveCard();
            card.Body.Add(new AdaptiveTextBlock() { Text = $"**No**: {item.No}", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextBlock() { Text = $"**Description** :{item.Description}", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextBlock()
            {
                Text = $"**Price**: {item.Unit_Price}/{item.Base_Unit_of_Measure}", Size = AdaptiveTextSize.Medium,
                Weight = AdaptiveTextWeight.Bolder
            });
//            card.Actions.Add(new AdaptiveSubmitAction() { Title = "Submit" });
            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }

    }
}
