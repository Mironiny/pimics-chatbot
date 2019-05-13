// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date:
// ===============================

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Logging;
using PimBot.Dialogs;
using PimBot.Services;
using PimBot.State;

namespace PimBot
{
    public class PimBot : IBot
    {
        public static readonly string LuisConfiguration = "pimbotdp";

        private readonly BotServices _services;
        private readonly UserState _userState;
        private readonly ConversationState _conversationState;
        private readonly IStatePropertyAccessor<OnTurnState> _onTurnAccessor;
        private readonly IStatePropertyAccessor<DialogState> _dialogStateAccessor;
        private readonly ILogger _logger;

        public PimBot(BotServices services, UserState userState, ConversationState conversationState, ILoggerFactory loggerFactory, IPimbotServiceProvider provider)
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
            Dialogs.Add(new MainDispatcher(services, _onTurnAccessor, userState, conversationState, loggerFactory, provider));
        }

        private DialogSet Dialogs { get; set; }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // If user come from webchat bot should get user info
            if (turnContext.Activity.Name == "webchat/join")
            {
                await turnContext.SendActivityAsync(
                    $"{Messages.Greetings}, {turnContext.Activity.From.Id}. {Environment.NewLine} {Messages.IntroducingMessage}",
                    cancellationToken: cancellationToken);

                await turnContext.SendActivityAsync(Messages.HelpMessage, cancellationToken: cancellationToken);
                await turnContext.SendActivityAsync(Messages.WhatCanIDo, cancellationToken: cancellationToken);
            }

            // Classic reaction to message
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
            else if (turnContext.Activity.ChannelId == "emulator" && turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
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
                                $"{Messages.Greetings}, {turnContext.Activity.From.Id}. {Environment.NewLine} {Messages.IntroducingMessage}",
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