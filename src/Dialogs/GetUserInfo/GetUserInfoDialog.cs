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
using PimBot.Messages;
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
                { Messages.Name, 0 },
                { Messages.Address, 1 },
                { Messages.PostCode, 2 },
                { Messages.City, 3 },
                { Messages.IsAddressMatchShippingAddress, 4 },
                { Messages.ShippingName, 6 },
                { Messages.ShippingAddress, 7 },
                { Messages.ShippingPostCode, 8 },
                { Messages.ShippingCity, 9 },
                { "ConfirmCustomerInfo", 11 },
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

                PromptForShippingName,
                PromptForShippingAddress,
                PromptForShippingPostCode,
                PromptForShippingCity,
                ResolveShippingCity,

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
            await stepContext.Context.SendActivityAsync(Messages.GetUserInfoNewOrder);
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
                        Text = Messages.GetUserInfoPromptName,
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
                            Text = Messages.GetUserInfoPromptAddress,
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
                            Text = Messages.GetUserInfoPromptPostCode,
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
                            Text = Messages.GetUserInfoPromptCity,
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

            if (customerState.IsShippingAdressMatch == null)
            {
                var opts = new PromptOptions
                {
                    Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = Messages.GetUserInfoPromptShippingMatch,
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
            if (customerState.IsShippingAdressMatch == null)
            {
                bool isShippingAdressSet = (bool)stepContext.Result;
                customerState.IsShippingAdressMatch = isShippingAdressSet;
                await _customerStateAccessor.SetAsync(stepContext.Context, customerState);
            }
            if (customerState.IsShippingAdressMatch == true)
            {
                stepContext.ActiveDialog.State["stepIndex"] = DialogIndex["ConfirmCustomerInfo"];
            }

            return await stepContext.ContinueDialogAsync();
        }

        private async Task<DialogTurnResult> PromptForShippingName(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());

            if (string.IsNullOrWhiteSpace(customerState.ShippingName))
            {
                {
                    var opts = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = Messages.GetUserInfoPromptShippingName,
                        },
                    };
                    return await stepContext.PromptAsync(ShippingNamePrompt, opts);
                }
            }

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> PromptForShippingAddress(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var shippingName = stepContext.Result as string;

            // Save postcode, if prompted.
            if (string.IsNullOrWhiteSpace(customerState.ShippingName) && shippingName != null)
            {
                customerState.ShippingName = shippingName;
                await _customerStateAccessor.SetAsync(stepContext.Context, customerState);
            }

            if (string.IsNullOrWhiteSpace(customerState.ShippingAddress))
            {
                {
                    var opts = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = Messages.GetUserInfoPromptShippingAddress,
                        },
                    };
                    return await stepContext.PromptAsync(ShippingAdressPrompt, opts);
                }
            }

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> PromptForShippingPostCode(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var shippingAddress = stepContext.Result as string;

            // Save postcode, if prompted.
            if (string.IsNullOrWhiteSpace(customerState.ShippingAddress) && shippingAddress != null)
            {
                customerState.ShippingAddress = shippingAddress;
                await _customerStateAccessor.SetAsync(stepContext.Context, customerState);
            }

            if (string.IsNullOrWhiteSpace(customerState.ShippingPostCode))
            {
                {
                    var opts = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = Messages.GetUserInfoPromptShippingPostCode,
                        },
                    };
                    return await stepContext.PromptAsync(ShippingPostCodePrompt, opts);
                }
            }

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> PromptForShippingCity(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var shippingPostCode = stepContext.Result as string;

            // Save postcode, if prompted.
            if (string.IsNullOrWhiteSpace(customerState.ShippingPostCode) && shippingPostCode != null)
            {
                customerState.ShippingPostCode = shippingPostCode;
                await _customerStateAccessor.SetAsync(stepContext.Context, customerState);
            }

            if (string.IsNullOrWhiteSpace(customerState.ShippingCity))
            {
                {
                    var opts = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = Messages.GetUserInfoPromptShippingCity,
                        },
                    };
                    return await stepContext.PromptAsync(ShippingCityPrompt, opts);
                }
            }

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> ResolveShippingCity(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var shippingCity = stepContext.Result as string;

            // Save postcode, if prompted.
            if (string.IsNullOrWhiteSpace(customerState.ShippingCity) && shippingCity != null)
            {
                customerState.ShippingCity = shippingCity;
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
                    Text = Messages.GetUserInfoPromptIsOrderOk,
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

            if (confirmCustomerInfo)
            {
                await stepContext.Context.SendActivityAsync(Messages.GetUserInfoContinueWithOrder);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(Messages.GetUserInfoPromptChange);
                List<string> dialogIndex = GetListOfEditableProperties(customerState.IsShippingAdressMatch);

                return await stepContext.PromptAsync(
                    CorrectValuePrompt,
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text(Messages.GetUserInfoPromptEdit),
                        Choices = ChoiceFactory.ToChoices(dialogIndex),
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
                    await stepContext.Context.SendActivityAsync(Messages.GetUserInfoPromptIsOrderOk);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(Messages.GetUserInfoServerIssue);
                }

                return await stepContext.EndDialogAsync();
            }
            else
            {
                switch (whatToChange.Value)
                {
                    case Messages.Name:
                        customerState.Name = null;
                        break;

                    case Messages.Address:
                        customerState.Address = null;
                        break;

                    case Messages.PostCode:
                        customerState.PostCode = null;
                        break;

                    case Messages.City:
                        customerState.City = null;
                        break;

                    case Messages.IsAddressMatchShippingAddress:
                        customerState.IsShippingAdressMatch = null;
                        break;

                    case Messages.ShippingName:
                        customerState.ShippingName = null;
                        break;

                    case Messages.ShippingAddress:
                        customerState.ShippingAddress = null;
                        break;

                    case Messages.ShippingPostCode:
                        customerState.ShippingPostCode = null;
                        break;

                    case Messages.ShippingCity:
                        customerState.ShippingCity = null;
                        break;
                }

                await _customerStateAccessor.SetAsync(stepContext.Context, customerState);
                stepContext.ActiveDialog.State["stepIndex"] = DialogIndex[whatToChange.Value];
                return await stepContext.ContinueDialogAsync();
            }
        }

        private string GetPrintableCustomerInfo(CustomerState customerState)
        {
            string result = Messages.GetUserInfoCustomerInformation + Environment.NewLine;
            result += $"**{Messages.Name}**: {customerState.Name}" + Environment.NewLine;
            result += $"**{Messages.Address}**: {customerState.Address}" + Environment.NewLine;
            result += $"**{Messages.PostCode}**: {customerState.PostCode}" + Environment.NewLine;
            result += $"**{Messages.City}**: {customerState.City}" + Environment.NewLine;

            if (customerState.IsShippingAdressMatch != null)
            {
                var isAddressMatch = (bool)customerState.IsShippingAdressMatch ? Messages.Yes : Messages.No;
                result += $"{Messages.GetUserInfoIsThisYourShippingInformation} {isAddressMatch}" + Environment.NewLine;

                if (customerState.IsShippingAdressMatch == false)
                {
                    result += $"**{Messages.ShippingName}**: {customerState.ShippingName}" + Environment.NewLine;
                    result += $"**{Messages.ShippingAddress}**: {customerState.ShippingAddress}" + Environment.NewLine;
                    result += $"**{Messages.ShippingPostCode}**: {customerState.ShippingPostCode}" + Environment.NewLine;
                    result += $"**{Messages.ShippingCity}**: {customerState.ShippingCity}" + Environment.NewLine;
                }
            }

            return result;
        }

        private List<string> GetListOfEditableProperties(bool? isAddressMatch)
        {
            List<string> editableProperties = new List<string>();
            if (isAddressMatch == null)
            {
                return editableProperties;
            }

            editableProperties.Add(Messages.Name);
            editableProperties.Add(Messages.Address);
            editableProperties.Add(Messages.PostCode);
            editableProperties.Add(Messages.City);
            editableProperties.Add(Messages.IsAddressMatchShippingAddress);

            if (!(bool)isAddressMatch)
            {
                editableProperties.Add(Messages.ShippingName);
                editableProperties.Add(Messages.ShippingAddress);
                editableProperties.Add(Messages.ShippingPostCode);
                editableProperties.Add(Messages.ShippingCity);
            }

            return editableProperties;
        }
    }
}
