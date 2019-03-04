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
                var items = await _itemService.GetAllItemsAsync(firstEntity);

                var response = stepContext.Context.Activity.CreateReply();
                response.Attachments = new List<Attachment>() { CreateAdaptiveCardUsingSdk() };

                await context.SendActivityAsync("Entity is: " + firstEntity);
                await context.SendActivityAsync(GetPrintableItems(items));
                await context.SendActivityAsync(response);


            }

            return await stepContext.EndDialogAsync();
        }

        private string GetPrintableItems(IEnumerable<PimItem> items)
        {
            string result = string.Empty;
            foreach (var item in items)
            {
                result += $"Name: {item.Description}, unit price: {item.Unit_Price} {Environment.NewLine}";
            }

            return result;
        }

        private Attachment CreateAdaptiveCardUsingSdk()
        {
            var card = new AdaptiveCard();
            card.Body.Add(new AdaptiveTextBlock() { Text = "Colour", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveChoiceSetInput()
            {
                Id = "Colour",
                Style = AdaptiveChoiceInputStyle.Compact,
                Choices = new List<AdaptiveChoice>(new[] {
                    new AdaptiveChoice() { Title = "Red", Value = "RED" },
                    new AdaptiveChoice() { Title = "Green", Value = "GREEN" },
                    new AdaptiveChoice() { Title = "Blue", Value = "BLUE" } })
            });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Registration number:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "RegistrationNumber" });
            card.Actions.Add(new AdaptiveSubmitAction() { Title = "Submit" });
            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }

    }
}
