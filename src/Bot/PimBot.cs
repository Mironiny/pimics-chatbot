// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PimBot.Service;
using PimBot.Service.Impl;
using PimBot.State;
using PimBotDp.Dialogs;
using PimBotDp.State;

namespace PimBot
{

    public class PimBot : IBot
    {
        private readonly IItemService _itemService = new ItemService();

        public static readonly string LuisConfiguration = "pimbotdp";

        private readonly PimBotStateAccesors _pimBotStateAccesors;
        private readonly BotServices _services;
        private readonly UserState _userState;
        private readonly ConversationState _conversationState;
        private readonly IStatePropertyAccessor<OnTurnState> _onTurnAccessor;
        private readonly IStatePropertyAccessor<DialogState> _dialogStateAccessor;
        private readonly ILogger _logger;

        public PimBot(BotServices services, UserState userState, ConversationState conversationState, ILoggerFactory loggerFactory)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _userState = userState ?? throw new ArgumentNullException(nameof(userState));
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));

            _onTurnAccessor = conversationState.CreateProperty<OnTurnState>("onTurnStateProperty");
            _dialogStateAccessor = _conversationState.CreateProperty<DialogState>(nameof(DialogState));

            //             Verify logger configuration.
            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }
            _logger = loggerFactory.CreateLogger<PimBot>();
            _logger.LogTrace("CafeBot turn start.");

            // Verify LUIS configuration.
            if (!_services.LuisServices.ContainsKey(LuisConfiguration))
            {
                throw new InvalidOperationException($"The bot configuration does not contain a service type of `luis` with the id `{LuisConfiguration}`.");
            }

            Dialogs = new DialogSet(_dialogStateAccessor);
            Dialogs.Add(new MainDispatcher(services, _onTurnAccessor, userState, conversationState, loggerFactory));

        }

        private DialogSet Dialogs { get; set; }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            //       var dc = await Dialogs.CreateContextAsync(turnContext);

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {

                var luisResults = await _services.LuisServices[LuisConfiguration]
                    .RecognizeAsync(turnContext, cancellationToken);
                var entities = luisResults.Entities.ToString();
                var topScoringIntent = luisResults?.GetTopScoringIntent();
                var topIntent = topScoringIntent.Value.intent;

                var onTurnState = new OnTurnState(topIntent, luisResults.Entities);

                await _onTurnAccessor.SetAsync(turnContext, onTurnState);
//                await ConversationState.SaveChangesAsync(turnContext);
//                await UserState.SaveChangesAsync(turnContext);

                //                await turnContext.SendActivityAsync("Your intent is: " + topIntent,
                //                    cancellationToken: cancellationToken);

                // Create dialog context.
                var dc = await Dialogs.CreateContextAsync(turnContext);

                // Continue outstanding dialogs.
                await dc.ContinueDialogAsync();

             //   await dc.BeginDialogAsync(nameof(MainDispatcher));

                // Begin main dialog if no outstanding dialogs/ no one responded.
                if (!dc.Context.Responded)
                {
                    await dc.BeginDialogAsync(nameof(MainDispatcher));
                }

                //                // Continue the current dialog
                //                var dialogResult = await dc.ContinueDialogAsync();
                //
                //                // if no one has responded,
                //                if (!dc.Context.Responded)
                //                {
                //                    // examine results from active dialog
                //                    switch (dialogResult.Status)
                //                    {
                //                        case DialogTurnStatus.Empty:
                //
                //                            switch (topIntent)
                //                            {
                //                                case Intents.FindItem:
                //                                    if (luisResults.Entities["keyPhrase"].Count() > 0)
                //                                    {
                //                                        var firstEntity = (string) luisResults.Entities["keyPhrase"].First;
                //                                        await turnContext.SendActivityAsync("Entity is: " + firstEntity,
                //                                            cancellationToken: cancellationToken);
                //                                    }
                //
                //                                    break;
                //
                //                                case Intents.AddItem:
                //                                    if (luisResults.Entities["itemAdd"].Count() > 0)
                //                                    {
                //                                        var firstEntity = (string) luisResults.Entities["itemAdd"].First;
                //
                //                                        //TODO check if item exists in PIM
                //                                        CartState cartState =
                //                                            await _cartStateAccessor.GetAsync(turnContext, () => new CartState());
                //                                        if (cartState.Items == null)
                //                                        {
                //                                            cartState.Items = new List<string>();
                //                                        }
                //
                //                                        cartState.Items.Add(firstEntity);
                //
                //                                        // Set the new values into state.
                //                                        await _cartStateAccessor.SetAsync(turnContext, cartState);
                //                                        await turnContext.SendActivityAsync("Entity is: " + firstEntity,
                //                                            cancellationToken: cancellationToken);
                //                                    }
                //
                //                                    break;
                //
                //                                case Intents.ShowCart:
                //                                    var cartState1 =
                //                                        await _cartStateAccessor.GetAsync(turnContext, () => new CartState());
                //                                    if (cartState1.Items == null || cartState1.Items.Count == 0)
                //                                    {
                //                                        await turnContext.SendActivityAsync(Messages.Messages.EmptyCart,
                //                                            cancellationToken: cancellationToken);
                //                                        await turnContext.SendActivityAsync(Messages.Messages.SuggestHelp,
                //                                            cancellationToken: cancellationToken);
                //                                    }
                //                                    else
                //                                    {
                //                                        string itemsInCart = Messages.Messages.ShowCartTitle + Environment.NewLine;
                //                                        foreach (var item in cartState1.Items)
                //                                        {
                //                                            itemsInCart += "* " + item + Environment.NewLine;
                //                                        }
                //
                //                                        itemsInCart += Environment.NewLine + Environment.NewLine +
                //                                                       Messages.Messages.ShowCartAfter;
                //
                //                                        await turnContext.SendActivityAsync(itemsInCart,
                //                                            cancellationToken: cancellationToken);
                //                                    }
                //
                //                                    break;
                //
                //                                case Intents.None:
                //                                    await turnContext.SendActivityAsync(Messages.Messages.NotUnderstand,
                //                                        cancellationToken: cancellationToken);
                //                                    await turnContext.SendActivityAsync(Messages.Messages.SuggestHelp,
                //                                        cancellationToken: cancellationToken);
                //                                    break;
                //
                //                                case Intents.Help:
                //                                    await turnContext.SendActivityAsync(Messages.Messages.HelpMessage,
                //                                        cancellationToken: cancellationToken);
                //                                    break;
                //
                //                                case "items":
                //                                    try
                //                                    {
                //                                        var items = await _itemService.GetAllItemsAsync();
                //                                        foreach (var item in items)
                //                                        {
                //                                            await turnContext.SendActivityAsync(item,
                //                                                cancellationToken: cancellationToken);
                //                                        }
                //                                    }
                //                                    catch (Exception ex)
                //                                    {
                //                                        await turnContext.SendActivityAsync(Messages.Messages.ServerIssue,
                //                                            cancellationToken: cancellationToken);
                //                                    }
                //
                //                                    break;
                //
                //                                default:
                //                                    await turnContext.SendActivityAsync(Messages.Messages.NotUnderstand,
                //                                        cancellationToken: cancellationToken);
                //                                    break;
                //                            }
                //
                //                            break;
                //
                //                        case DialogTurnStatus.Waiting:
                //                            // The active dialog is waiting for a response from the user, so do nothing.
                //                            break;
                //
                //                        case DialogTurnStatus.Complete:
                //                            await dc.EndDialogAsync();
                //                            break;
                //
                //                        default:
                //                            await dc.CancelAllDialogsAsync();
                //                            break;
                //                    }
                //                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    // Iterate over all new members added to the conversation
                    foreach (var member in turnContext.Activity.MembersAdded)
                    {
                        // Greet anyone that was not the target (recipient) of this message
                        // the 'bot' is the recipient for events from the channel,
                        // turnContext.Activity.MembersAdded == turnContext.Activity.Recipient.Id indicates the
                        // bot was added to the conversation.
                        if (member.Id != turnContext.Activity.Recipient.Id)
                        {
                            await turnContext.SendActivityAsync($"Hi there, {member.Name} ✌️. {Messages.Messages.IntroducingMessage}", cancellationToken: cancellationToken);

                            // There would be nice typing

                            await turnContext.SendActivityAsync(Messages.Messages.HelpMessage, cancellationToken: cancellationToken);
                        }
                    }
                }
            }
            await _conversationState.SaveChangesAsync(turnContext);
            await _userState.SaveChangesAsync(turnContext);
        }
    }
}

//if (pimBotState.DidBotWelcomeUser == false)
//{
//pimBotState.DidBotWelcomeUser = true;
//
//await _pimBotStateAccesors.PimBotState.SetAsync(turnContext, pimBotState);
//await _pimBotStateAccesors.UserState.SaveChangesAsync(turnContext);
//
//// the channel should sends the user name in the 'From' object
//var userName = turnContext.Activity.From.Name;
//
//await turnContext.SendActivityAsync($"You are seeing this message because this was your first message ever to this bot.", cancellationToken: cancellationToken);
//await turnContext.SendActivityAsync($"It is a good practice to welcome the user and provide personal greeting. For example, welcome {userName}.", cancellationToken: cancellationToken);
//
//}
//else
//{