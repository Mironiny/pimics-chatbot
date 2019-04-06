using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using PimBot.Service;
using PimBot.Service.Impl;
using PimBot.Services;
using PimBot.Services.Impl;
using PimBot.State;

namespace PimBot.Dialogs.AddItem
{
    public class AddItemDialog : ComponentDialog
    {
        public const string Name = "Add_item";

        // Prompts names
        private const string CountPrompt = "countPrompt";

        private readonly IItemService _itemService = new ItemService();
        private readonly BotServices _services;
        private IStatePropertyAccessor<OnTurnState> _onTurnAccessor;
        private IStatePropertyAccessor<CartState> _cartStateAccessor;
        private readonly ICustomerService _customerService = new CustomerService();

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
            if (onTurnProperty.Entities[EntityNames.Item] != null && onTurnProperty.Entities[EntityNames.Item].Count() > 0)
            {
                var firstEntity = (string)onTurnProperty.Entities[EntityNames.Item].First;

                var pimItem = await _itemService.FindItemByNo(firstEntity);

                if (pimItem == null)
                {
                    await context.SendActivityAsync($"{Messages.NotFound} **{firstEntity}**.");
                    return await stepContext.EndDialogAsync();
                }

                CustomerState customerState =
                    await _customerService.GetCustomerStateById(stepContext.Context.Activity.From.Id);

                customerState.Cart.Items.Add(pimItem);
                await _customerService.UpdateCustomerState(customerState);
                return await stepContext.NextAsync();
            }
            else
            {
                await context.SendActivityAsync(Messages.AddItemNoItem);
                return await stepContext.EndDialogAsync();
            }
        }

        private async Task<DialogTurnResult> PromptForCountStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerService.GetCustomerStateById(stepContext.Context.Activity.From.Id);

            var item = customerState.Cart.Items[customerState.Cart.Items.Count - 1].Description;
            var prompt = string.Format(Messages.AddItemHowManyPrompt, item);
            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = prompt,
                },
                RetryPrompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = prompt + Messages.CancelPrompt,
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

            CustomerState customerState =
                await _customerService.GetCustomerStateById(stepContext.Context.Activity.From.Id);

            customerState.Cart.Items[customerState.Cart.Items.Count - 1].Count = outputCount;
            await _customerService.UpdateCustomerState(customerState);

            await stepContext.Context.SendActivityAsync(Messages.AddToCartAdded);
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
                await promptContext.Context.SendActivityAsync(Messages.AddToCartValidationOnlyNumber);
                return false;
            }

            if (n < 1)
            {
                await promptContext.Context.SendActivityAsync(Messages.AddToCartValidationPositiveNumber);
                return false;
            }

            return true;
        }
    }
}
