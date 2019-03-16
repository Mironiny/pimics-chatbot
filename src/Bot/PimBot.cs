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
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PimBot.Service;
using PimBot.Service.Impl;
using PimBot.State;
using PimBotDp.Dialogs;

namespace PimBot
{
    public class PimBot : IBot
    {
        public static readonly string LuisConfiguration = "pimbotdp";

        private readonly IItemService _itemService = new ItemService();

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

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger<PimBot>();
            _logger.LogTrace("Pimbot turn start.");

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
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Found intent from the input
                var luisResults = await _services.LuisServices[LuisConfiguration]
                    .RecognizeAsync(turnContext, cancellationToken);
                var entities = luisResults.Entities.ToString();
                var topScoringIntent = luisResults?.GetTopScoringIntent();
                var topIntent = topScoringIntent.Value.intent;

                var onTurnState = new OnTurnState(topIntent, luisResults.Entities);

                await _onTurnAccessor.SetAsync(turnContext, onTurnState);

                // Create dialog context.
                var dc = await Dialogs.CreateContextAsync(turnContext);

                // Continue outstanding dialogs.
                await dc.ContinueDialogAsync();

                // Begin main dialog if no outstanding dialogs/ no one responded.
                if (!dc.Context.Responded)
                {
                    await dc.BeginDialogAsync(nameof(MainDispatcher));
                }
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
                            await turnContext.SendActivityAsync(
                                $"{Messages.Greetings}, {member.Name}. {Environment.NewLine} {Messages.IntroducingMessage}",
                                cancellationToken: cancellationToken);

                            await turnContext.SendActivityAsync(Messages.HelpMessage, cancellationToken: cancellationToken);
                            await turnContext.SendActivityAsync(Messages.WhatCanIDo, cancellationToken: cancellationToken);
                        }
                    }
                }
            }

            await _conversationState.SaveChangesAsync(turnContext);
            await _userState.SaveChangesAsync(turnContext);
        }
    }
}