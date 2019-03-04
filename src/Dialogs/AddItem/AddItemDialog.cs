using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using PimBot.Service;
using PimBot.Service.Impl;
using PimBotDp.Constants;
using PimBotDp.State;

namespace PimBotDp.Dialogs.AddItem
{
    public class AddItemDialog : ComponentDialog
    {
        public const string Name = "Add_item";
        private readonly BotServices _services;
        private IStatePropertyAccessor<OnTurnState> _onTurnAccessor;
        private IStatePropertyAccessor<CartState> _cartStateAccessor;

        // Prompts names
        private const string CountPrompt = "countPrompt";

        public AddItemDialog(BotServices services, IStatePropertyAccessor<OnTurnState> onTurnAccessor, IStatePropertyAccessor<CartState> cartStateAccessor)
            : base(Name)
        {
            _services = services;
            _onTurnAccessor = onTurnAccessor;
            _cartStateAccessor = cartStateAccessor;

            // Add dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                InitializeStateStepAsync,
                PromptForCountStepAsync,
                ResolveCountAsync,
            };
            AddDialog(new WaterfallDialog(
                "start",
                waterfallSteps));
            AddDialog(new TextPrompt(CountPrompt, ValidateCount));
        }

        private async Task<DialogTurnResult> InitializeStateStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var onTurnProperty = await _onTurnAccessor.GetAsync(context, () => new OnTurnState());
            if (onTurnProperty.Entities[EntityNames.Item].Count() > 0)
            {
                var firstEntity = (string)onTurnProperty.Entities[EntityNames.Item].First;

                //TODO check if item exists in PIM
                CartState cartState =
                    await _cartStateAccessor.GetAsync(context, () => new CartState());
                if (cartState.Items == null)
                {
                    cartState.Items = new List<CartItem>();
                }

                cartState.Items.Add(new CartItem(firstEntity));

                // Set the new values into state.
                await _cartStateAccessor.SetAsync(context, cartState);
                await context.SendActivityAsync("Entity is: " + firstEntity);
            }
            return await stepContext.NextAsync();

        }

        private async Task<DialogTurnResult> PromptForCountStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CartState cartState =
                await _cartStateAccessor.GetAsync(stepContext.Context, () => new CartState());

            var item = cartState.Items[cartState.Items.Count - 1].Description;

            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = $"How many **{item}** do you want order?",
                },
                RetryPrompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = $"How many **{item}** do you want order?",
                },
            };
            return await stepContext.PromptAsync(CountPrompt, opts);
        }

        private async Task<DialogTurnResult> ResolveCountAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var context = stepContext.Context;

            var inputCount = stepContext.Result as string;
            int outputCount;
            int.TryParse(inputCount, out outputCount);

            CartState cartState =
                await _cartStateAccessor.GetAsync(context, () => new CartState());

            cartState.Items[cartState.Items.Count - 1].Count = outputCount;
            await _cartStateAccessor.SetAsync(context, cartState);

            await stepContext.Context.SendActivityAsync("Added to cart 👍. You can show your current cart by writting *show cart*.");
            return await stepContext.EndDialogAsync();
        }

        private async Task<bool> ValidateCount(PromptValidatorContext<string> promptContext,
            CancellationToken cancellationToken)
        {
            var count = promptContext.Recognized.Value;
            int n;
            bool isNumeric = int.TryParse(count, out n);
            if (!isNumeric)
            {
                await promptContext.Context.SendActivityAsync($"Sorry, please add just number.");
                return false;
            }

            if (n < 1)
            {
                await promptContext.Context.SendActivityAsync($"Sorry, count has to be greater than 0.");
                return false;
            }

            return true;
        }
    }
}
