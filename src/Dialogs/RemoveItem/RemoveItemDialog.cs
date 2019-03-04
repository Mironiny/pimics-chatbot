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
using PimBotDp.Constants;
using PimBotDp.State;

namespace PimBotDp.Dialogs.AddItem
{
    public class RemoveItemDialog : ComponentDialog
    {
        public const string Name = "Remove_item";
        private readonly BotServices _services;
        private IStatePropertyAccessor<OnTurnState> _onTurnAccessor;
        private IStatePropertyAccessor<CartState> _cartStateAccessor;

        // Prompts names
        private const string ConfirmPrompt = "confirmPrompt";

        public RemoveItemDialog(BotServices services, IStatePropertyAccessor<OnTurnState> onTurnAccessor, IStatePropertyAccessor<CartState> cartStateAccessor)
            : base(Name)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            _services = services;
            _onTurnAccessor = onTurnAccessor;
            _cartStateAccessor = cartStateAccessor;

            // Add dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                InitializeStateStepAsync,
                PromptForConfirmStepAsync,
                ResolveConfirmAsync,
            };
            AddDialog(new WaterfallDialog(
                "start",
                waterfallSteps));
            AddDialog(new ConfirmPrompt(ConfirmPrompt));
        }

        private async Task<DialogTurnResult> InitializeStateStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var onTurnProperty = await _onTurnAccessor.GetAsync(context, () => new OnTurnState());
            if (onTurnProperty.Entities != null || onTurnProperty.Entities[EntityNames.Item].Count() > 0)
            {
                var item = (string)onTurnProperty.Entities[EntityNames.Item].First;

                // Check if item exists in cart
                CartState cartState =
                    await _cartStateAccessor.GetAsync(context, () => new CartState());
                if (cartState.Items == null)
                {
                    cartState.Items = new List<PimItem>();
                }

                if (!cartState.Items.Exists(x => x.No == item))
                {
                    await context.SendActivityAsync($"Sorry, I cannot find {item} in your cart. You can show your cart simple by write *show cart*.");
                    return await stepContext.EndDialogAsync();
                }
                else
                {
                    return await stepContext.NextAsync();
                }
            }

            await context.SendActivityAsync($"Sorry, I cannot find in your cart. You can show your cart simple by write *show cart*.");
            return await stepContext.EndDialogAsync();
        }

        private async Task<DialogTurnResult> PromptForConfirmStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = $"Are you sure?",
                },
            };
            return await stepContext.PromptAsync(ConfirmPrompt, opts);
        }

        private async Task<DialogTurnResult> ResolveConfirmAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            bool isConfirmed = (bool)stepContext.Result;
            if (isConfirmed)
            {
                CartState cartState =
                    await _cartStateAccessor.GetAsync(stepContext.Context, () => new CartState());

                var nameToRemove = cartState.Items[cartState.Items.Count - 1].Description;
                var item = cartState.Items.SingleOrDefault(x => x.Description == nameToRemove);
                if (item != null)
                {
                    cartState.Items.Remove(item);
                }

                await stepContext.Context.SendActivityAsync("Ok, removed.");
                return await stepContext.EndDialogAsync();
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Ok, never mind.");
                return await stepContext.EndDialogAsync();
            }
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
