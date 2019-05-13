// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date:
// ===============================

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using PimBot.Services;
using PimBot.Services.Impl;
using PimBot.State;

namespace PimBot.Dialogs
{
    /// <summary>
    /// Class represents remove item dialog.
    /// </summary>
    public class RemoveItemDialog : ComponentDialog
    {
        public const string Name = "Remove_item";
        private readonly ICustomerService _customerService = new CustomerService();

        private readonly BotServices _services;
        private IStatePropertyAccessor<OnTurnState> _onTurnAccessor;
        private IStatePropertyAccessor<CartState> _cartStateAccessor;

        // Prompts names
        private const string ConfirmPrompt = "confirmPrompt";

        public RemoveItemDialog(BotServices services, IStatePropertyAccessor<OnTurnState> onTurnAccessor, IStatePropertyAccessor<CartState> cartStateAccessor)
            : base(Name)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
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
            if (onTurnProperty.Entities[EntityNames.Item] != null && onTurnProperty.Entities[EntityNames.Item].Count() > 0)
            {
                var item = (string)onTurnProperty.Entities[EntityNames.Item].First;

                // Check if item exists in cart
                CustomerState customerState =
                    await _customerService.GetCustomerStateById(stepContext.Context.Activity.From.Id);

                if (!customerState.Cart.Items.Exists(x => x.No == item))
                {
                    await context.SendActivityAsync(Messages.RemoveItemForgotItem);
                    return await stepContext.EndDialogAsync();
                }
                else
                {
                    return await stepContext.NextAsync();
                }
            }

            await context.SendActivityAsync(Messages.RemoveItemForgotItem);
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
                CustomerState customerState =
                    await _customerService.GetCustomerStateById(stepContext.Context.Activity.From.Id);

                var nameToRemove = customerState.Cart.Items[customerState.Cart.Items.Count - 1].Description;
                var item = customerState.Cart.Items.SingleOrDefault(x => x.Description == nameToRemove);
                if (item != null)
                {
                    customerState.Cart.Items.Remove(item);
                }

                await _customerService.UpdateCustomerState(customerState);
                await stepContext.Context.SendActivityAsync(Messages.RemoveItemRemoved);
                return await stepContext.EndDialogAsync();
            }
            else
            {
                await stepContext.Context.SendActivityAsync(Messages.RemoveItemChangeMind);
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
                await promptContext.Context.SendActivityAsync(Messages.RemoveItemNotNumber);
                return false;
            }

            if (n < 1)
            {
                await promptContext.Context.SendActivityAsync(Messages.RemoveItemGreater);
                return false;
            }

            return true;
        }
    }
}
