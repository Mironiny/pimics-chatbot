using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using PimBot.Service;
using PimBot.Service.Impl;
using PimBotDp.Constants;
using PimBotDp.State;

namespace PimBotDp.Dialogs.AddItem
{
    public class GetUserInfoDialog : ComponentDialog
    {
        private ISalesOrderService _salesOrder = new SalesOrderService();
        private IItemService _itemService = new ItemService();

        // Prompts names
        private const string NamePrompt = "NamePrompt";
        private const string AdressPrompt = "AdressPrompt";
        private const string PostCodePrompt = "PostCodePrompt";
        private const string CityPrompt = "CityPrompt";
        private const string EmailPrompt = "EmailPrompt";
        private const string PhonePrompt = "PhonePrompt";
        private const string IsShippingAdressMatchPrompt = "IsShippingAdressMatchPrompt";
        private const string ShippingNamePrompt = "ShippingNamePrompt";
        private const string ShippingAdressPrompt = "ShippingAdressPrompt";
        private const string ShippingPostCodePrompt = "ShippingPostCodePrompt";
        private const string ShippingCityPrompt = "ShippingCityPrompt";
        private const string ConfirmUserInfoPrompt = "ConfirmUserInfoPrompt";
        private const string CorrectValuePrompt = "CorrectValuePrompt";

        public const string Name = "GetUserInfo";
        private readonly BotServices _services;
        private IStatePropertyAccessor<OnTurnState> _onTurnAccessor;
        private IStatePropertyAccessor<CartState> _cartStateAccessor;
        private IStatePropertyAccessor<CustomerState> _customerStateAccessor;

        private static readonly Dictionary<string, int> DialogIndex
            = new Dictionary<string, int>
            {
                { "Name", 0 },
                { "Address", 1 },
                { "Post code", 2 },
                { "City", 3 },
                { "Shipping", 4 },
            };

        public GetUserInfoDialog(BotServices services, IStatePropertyAccessor<OnTurnState> onTurnAccessor,
            IStatePropertyAccessor<CartState> cartStateAccessor,
            IStatePropertyAccessor<CustomerState> customerStateAccessor)
            : base(Name)
        {
            _services = services;
            _onTurnAccessor = onTurnAccessor;
            _cartStateAccessor = cartStateAccessor;
            _customerStateAccessor = customerStateAccessor;

            // Add dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                InitializeStateStepAsync,
                PromptForNameAsync,
                PromptForNameAdress,
                PromptForPostCode,
                PromptForCity,
                PromptForShipping,
                ResolveShipping,
                ConfirmCustomerInfo,
                ResolveConfirmCustomerInfo,
                ResolveIsEverythingOk,
            };
            AddDialog(new WaterfallDialog(
                "start",
                waterfallSteps));
            AddDialog(new TextPrompt(NamePrompt));
            AddDialog(new TextPrompt(AdressPrompt));
            AddDialog(new TextPrompt(PostCodePrompt));
            AddDialog(new TextPrompt(CityPrompt));
            AddDialog(new TextPrompt(EmailPrompt));
            AddDialog(new TextPrompt(PhonePrompt));
            AddDialog(new ConfirmPrompt(IsShippingAdressMatchPrompt));
            AddDialog(new TextPrompt(ShippingNamePrompt));
            AddDialog(new TextPrompt(ShippingAdressPrompt));
            AddDialog(new TextPrompt(ShippingPostCodePrompt));
            AddDialog(new ChoicePrompt(CorrectValuePrompt));
            AddDialog(new TextPrompt(ShippingCityPrompt));
            AddDialog(new ConfirmPrompt(ConfirmUserInfoPrompt));
        }

        private async Task<DialogTurnResult> InitializeStateStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var onTurnProperty = await _onTurnAccessor.GetAsync(context, () => new OnTurnState());
            await stepContext.Context.SendActivityAsync("To make new order I need contact information about you.");
            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> PromptForNameAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());

            if (string.IsNullOrWhiteSpace(customerState.Name))
            {
                var opts = new PromptOptions
                {
                    Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "What is your **full name**? Or company name?",
                    },
                };
                return await stepContext.PromptAsync(NamePrompt, opts);
            }

            return await stepContext.ContinueDialogAsync();
        }

        private async Task<DialogTurnResult> PromptForNameAdress(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var name = stepContext.Result as string;

            // Save name, if prompted.
            if (string.IsNullOrWhiteSpace(customerState.Name) && name != null)
            {
                customerState.Name = name;
                await _customerStateAccessor.SetAsync(stepContext.Context, customerState);
            }

            if (string.IsNullOrWhiteSpace(customerState.Address))
            {
                {
                    var opts = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "What is your **adress**?",
                        },
                    };
                    return await stepContext.PromptAsync(AdressPrompt, opts);
                }
            }

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> PromptForPostCode(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var adress = stepContext.Result as string;

            // Save name, if prompted.
            if (string.IsNullOrWhiteSpace(customerState.Address) && adress != null)
            {
                customerState.Address = adress;
                await _customerStateAccessor.SetAsync(stepContext.Context, customerState);
            }

            if (string.IsNullOrWhiteSpace(customerState.PostCode))
            {
                {
                    var opts = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "What is your **post code**?",
                        },
                    };
                    return await stepContext.PromptAsync(PostCodePrompt, opts);
                }
            }

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> PromptForCity(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var postCode = stepContext.Result as string;

            // Save postcode, if prompted.
            if (string.IsNullOrWhiteSpace(customerState.PostCode) && postCode != null)
            {
                customerState.PostCode = postCode;
                await _customerStateAccessor.SetAsync(stepContext.Context, customerState);
            }

            if (string.IsNullOrWhiteSpace(customerState.City))
            {
                {
                    var opts = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "What is your **city**?",
                        },
                    };
                    return await stepContext.PromptAsync(CityPrompt, opts);
                }
            }

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> PromptForShipping(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {

            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var city = stepContext.Result as string;

            // Save postcode, if prompted.
            if (string.IsNullOrWhiteSpace(customerState.City) && city != null)
            {
                customerState.City = city;
                await _customerStateAccessor.SetAsync(stepContext.Context, customerState);
            }

            if (customerState.IsShippingAdressSet == null)
            {
                var opts = new PromptOptions
                {
                    Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Is this address also shipping address?",
                    },
                };
                return await stepContext.PromptAsync(IsShippingAdressMatchPrompt, opts);
            }
            return await stepContext.NextAsync();

        }

        private async Task<DialogTurnResult> ResolveShipping(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            //TODO use case when is shipping adress not same
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            if (customerState.IsShippingAdressSet == null)
            {
                bool isShippingAdressSet = (bool)stepContext.Result;
                customerState.IsShippingAdressSet = isShippingAdressSet;
                await _customerStateAccessor.SetAsync(stepContext.Context, customerState);
            }

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> ConfirmCustomerInfo(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            //TODO use case when is shipping adress not same
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());

            await stepContext.Context.SendActivityAsync(GetPrintableCustomerInfo(customerState));

            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = "Is all information correct?",
                },
            };
            return await stepContext.PromptAsync(ConfirmUserInfoPrompt, opts);
        }

        private async Task<DialogTurnResult> ResolveConfirmCustomerInfo(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            //TODO use case when is shipping adress not same
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());

            bool confirmCustomerInfo = (bool)stepContext.Result;
            //  stepContext.ActiveDialog.State["stepIndex"] = 0;
            //   return await stepContext.NextAsync();

            if (confirmCustomerInfo)
            {
                await stepContext.Context.SendActivityAsync("Okey, let's continue with the order");
            }
            else
            {
                await stepContext.Context.SendActivityAsync("What customer information do you want to change?");

                // Prompt for the location.
                return await stepContext.PromptAsync(
                    CorrectValuePrompt,
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Please choose a location."),
                        RetryPrompt = MessageFactory.Text("Sorry, please choose a location from the list."),
                        Choices = ChoiceFactory.ToChoices(new List<string>(DialogIndex.Keys)),
                    },
                    cancellationToken);
            }

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> ResolveIsEverythingOk(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var whatToChange = stepContext.Result as FoundChoice;

            // Save name, if prompted.
            if (whatToChange == null)
            {
                var isSave = await _salesOrder.CreateOrder(new CustomerState());
                if (isSave == true)
                {
                    await stepContext.Context.SendActivityAsync("Okey, let's continue with the order.");
                }
                else
                {
                    await stepContext.Context.SendActivityAsync("Sorry, there is some problem with server. Please try it later.");
                }

                return await stepContext.EndDialogAsync();
            }
            else
            {
                switch (whatToChange.Value)
                {
                    case "Name":
                        customerState.Name = null;
                        break;

                    case "Address":
                        customerState.Address = null;
                        break;

                    case "Post code":
                        customerState.PostCode = null;
                        break;

                    case "City":
                        customerState.City = null;
                        break;

                    case "Shipping":
                        customerState.IsShippingAdressSet = null;
                        break;
                }

                await _customerStateAccessor.SetAsync(stepContext.Context, customerState);
                stepContext.ActiveDialog.State["stepIndex"] = DialogIndex[whatToChange.Value];
                return await stepContext.ContinueDialogAsync();
            }
        }

        private string GetPrintableCustomerInfo(CustomerState customerState)
        {
            string result = "This is your customer information:" + Environment.NewLine;
            result += $"**Name**: {customerState.Name}" + Environment.NewLine;
            result += $"**Address**: {customerState.Address}" + Environment.NewLine;
            result += $"**Post code**: {customerState.PostCode}" + Environment.NewLine;
            result += $"**City**: {customerState.City}" + Environment.NewLine;

            if (customerState.IsShippingAdressSet != null)
            {
                var isAddressMatch = (bool)customerState.IsShippingAdressSet ? "Yes" : "No";
                result += $"Is address match shipping address: {isAddressMatch}" + Environment.NewLine;
            }

            return result;
        }
    }
}
