using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using PimBot.Service;
using PimBot.Service.Impl;
using PimBot.Services.Impl;
using PimBot.State;

namespace PimBot.Dialogs.AddItem
{
    /// <summary>
    /// Dialog which takes from user information needs to be know to complete the order.
    /// </summary>
    public class GetUserInfoDialog : ComponentDialog
    {
        private readonly ISalesOrderService _salesOrder = new SalesOrderService();
        private readonly IItemService _itemService = new ItemService();
        private readonly CustomerService _customerService = new CustomerService();


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
                { Messages.Email, 1 },
                { Messages.PhoneNumber, 2 },
                { Messages.Address, 3 },
                { Messages.PostCode, 4 },
                { Messages.City, 5 },
                { Messages.IsAddressMatchShippingAddress, 6 },
                { Messages.ShippingName, 8 },
                { Messages.ShippingAddress, 9 },
                { Messages.ShippingPostCode, 10 },
                { Messages.ShippingCity, 11 },
                { "ConfirmCustomerInfo", 13 },
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
                PromptForEmailAdress,
                PromptForPhone,
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
            AddDialog(new TextPrompt(EmailPrompt, ValidateEmail));
            AddDialog(new TextPrompt(PhonePrompt));
            AddDialog(new TextPrompt(AdressPrompt));
            AddDialog(new TextPrompt(PostCodePrompt));
            AddDialog(new TextPrompt(CityPrompt));
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
//           await stepContext.Context.SendActivityAsync(stepContext.Context.Activity.From.Id);
//           Activity replyToConversation = stepContext.Context.Activity.CreateReply();
//            replyToConversation.Attachments = new List<Attachment>();
//
//            replyToConversation.Attachments.Add(new Attachment
//            {
//                ContentType = OAuthCard.ContentType,
//                Content = new OAuthCard
//                {
//                    Text = "Please sign in",
//                    ConnectionName = " ",
//                    Buttons = new[]
//                    {
//                        new CardAction
//                        {
//                            Title = "Sign In",
//                            Text = "Sign In",
//                            Type = ActionTypes.Signin,
//                        },
//                    },
//                },
//            });
//
//            await stepContext.Context.SendActivityAsync(replyToConversation);
//
//            await stepContext.EndDialogAsync();
//            var response = stepContext.Context.Activity.CreateReply();
//
//            response.Attachments = new List<Attachment>() { CreateAdaptiveCardUsingSdk() };
//            await stepContext.Context.SendActivityAsync(response);
//
//            var tmp = await _salesOrder.GetSalesOrderByCustomer("John Haddock Insurance Co.");
            var context = stepContext.Context;
            var onTurnProperty = await _onTurnAccessor.GetAsync(context, () => new OnTurnState());
            await stepContext.Context.SendActivityAsync(Messages.GetUserInfoNewOrder);
            return await stepContext.NextAsync();
        }

        /// <summary>
        /// Prompt for NAME.
        /// </summary>
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

        /// <summary>
        /// Prompt for EMAIL.
        /// </summary>
        private async Task<DialogTurnResult> PromptForEmailAdress(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var name = stepContext.Result as string;

            if (string.IsNullOrWhiteSpace(customerState.Name) && name != null)
            {
                customerState.Name = name;
                await _customerStateAccessor.SetAsync(stepContext.Context, customerState);
            }

            if (string.IsNullOrWhiteSpace(customerState.Email))
            {
                {
                    var opts = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = Messages.GetUserInfoPromptEmail,
                        },
                    };
                    return await stepContext.PromptAsync(EmailPrompt, opts);
                }
            }

            return await stepContext.NextAsync();
        }

        /// <summary>
        /// Prompt for PHONE NUMBER.
        /// </summary>
        private async Task<DialogTurnResult> PromptForPhone(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var email = stepContext.Result as string;

            if (string.IsNullOrWhiteSpace(customerState.Email) && email != null)
            {
                customerState.Email = email;
                await _customerStateAccessor.SetAsync(stepContext.Context, customerState);
            }

            if (string.IsNullOrWhiteSpace(customerState.PhoneNumber))
            {
                {
                    var opts = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = Messages.GetUserInfoPromptPhoneNumber,
                        },
                    };
                    return await stepContext.PromptAsync(PhonePrompt, opts);
                }
            }

            return await stepContext.NextAsync();
        }

        /// <summary>
        /// Prompt for ADDRESS.
        /// </summary>
        private async Task<DialogTurnResult> PromptForNameAdress(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var phone = stepContext.Result as string;

            if (string.IsNullOrWhiteSpace(customerState.PhoneNumber) && phone != null)
            {
                customerState.PhoneNumber = phone;
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

        /// <summary>
        /// Prompt for POSTCODE.
        /// </summary>
        private async Task<DialogTurnResult> PromptForPostCode(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var adress = stepContext.Result as string;

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

        /// <summary>
        /// Prompt for CITY.
        /// </summary>
        private async Task<DialogTurnResult> PromptForCity(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var postCode = stepContext.Result as string;

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

        /// <summary>
        /// Prompt for SHIPPING.
        /// </summary>
        private async Task<DialogTurnResult> PromptForShipping(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var city = stepContext.Result as string;

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

        /// <summary>
        /// Resolve SHIPPING.
        /// </summary>
        private async Task<DialogTurnResult> ResolveShipping(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
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

        /// <summary>
        /// Prompt for SHIPPING NAME.
        /// </summary>
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

        /// <summary>
        /// Prompt for SHIPPING ADDRESS.
        /// </summary>
        private async Task<DialogTurnResult> PromptForShippingAddress(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var shippingName = stepContext.Result as string;

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

        /// <summary>
        /// Prompt for SHIPPPING POSTCODE.
        /// </summary>
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

        /// <summary>
        /// Prompt for SHIPPING CITY.
        /// </summary>
        private async Task<DialogTurnResult> PromptForShippingCity(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var shippingPostCode = stepContext.Result as string;

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

        /// <summary>
        /// Resolve SHIPPING CITY.
        /// </summary>
        private async Task<DialogTurnResult> ResolveShippingCity(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            CustomerState customerState =
                await _customerStateAccessor.GetAsync(stepContext.Context, () => new CustomerState());
            var shippingCity = stepContext.Result as string;

            if (string.IsNullOrWhiteSpace(customerState.ShippingCity) && shippingCity != null)
            {
                customerState.ShippingCity = shippingCity;
                await _customerStateAccessor.SetAsync(stepContext.Context, customerState);
            }

            return await stepContext.NextAsync();
        }

        /// <summary>
        /// Prompt for CONFIRM CUSTUMER INFO.
        /// </summary>
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

        /// <summary>
        /// Resolve CUSTOMER INFR.
        /// </summary>
        private async Task<DialogTurnResult> ResolveConfirmCustomerInfo(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
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

        /// <summary>
        /// Resolve everything is OK.
        /// </summary>
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
                var isSave = await _salesOrder.CreateOrder(customerState);
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

                    case Messages.Email:
                        customerState.Email = null;
                        break;

                    case Messages.PhoneNumber:
                        customerState.PhoneNumber = null;
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

        /// <summary>
        /// Print Customer state.
        /// </summary>
        private string GetPrintableCustomerInfo(CustomerState customerState)
        {
            string result = Messages.GetUserInfoCustomerInformation + Environment.NewLine;
            result += $"**{Messages.Name}**: {customerState.Name}" + Environment.NewLine;
            result += $"**{Messages.Email}**: {customerState.Email}" + Environment.NewLine;
            result += $"**{Messages.PhoneNumber}**: {customerState.PhoneNumber}" + Environment.NewLine;
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

        /// <summary>
        /// Get list
        /// </summary>
        private List<string> GetListOfEditableProperties(bool? isAddressMatch)
        {
            List<string> editableProperties = new List<string>();
            if (isAddressMatch == null)
            {
                return editableProperties;
            }

            editableProperties.Add(Messages.Name);
            editableProperties.Add(Messages.Email);
            editableProperties.Add(Messages.PhoneNumber);
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

        private async Task<bool> ValidateEmail(
            PromptValidatorContext<string> promptContext,
            CancellationToken cancellationToken)
        {
            var email = promptContext.Recognized.Value;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                await promptContext.Context.SendActivityAsync(Messages.GetUserInfoEmailIsNotValid);
                return false;
            }
        }

        private Attachment CreateAdaptiveCardUsingSdk()
        {
            var card = new AdaptiveCard();
            card.Body.Add(new AdaptiveTextBlock()
            {
                Text = $"User name",
                Size = AdaptiveTextSize.Medium,
                Weight = AdaptiveTextWeight.Bolder,
            });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "userName" });
            card.Body.Add(new AdaptiveTextBlock()
            {
                Text = $"Password",
                Size = AdaptiveTextSize.Medium,
                Weight = AdaptiveTextWeight.Bolder,
                Color = AdaptiveTextColor.Light,
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
