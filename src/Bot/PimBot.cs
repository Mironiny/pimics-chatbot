// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using PimBot.Service;
using PimBot.Service.Impl;
using PimBot.State;

namespace PimBot
{

    public class PimBot : IBot
    {
        private readonly IItemService _itemService = new ItemService();

        public static readonly string LuisConfiguration = "PimBot";

        // The bot state accessor object. Use this to access specific state properties.
        private readonly PimBotStateAccesors _pimBotStateAccesors;

        private readonly BotServices _services;

        public PimBot(BotServices services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));

            // Verify LUIS configuration.
            if (!_services.LuisServices.ContainsKey(LuisConfiguration))
            {
                throw new InvalidOperationException($"The bot configuration does not contain a service type of `luis` with the id `{LuisConfiguration}`.");
            }

        }

        //        public PimBot(PimBotStateAccesors statePropetyAccesors)
        //        {
        //           _pimBotStateAccesors = statePropetyAccesors ?? throw new System.ArgumentNullException("state accessor can't be null");
        //        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
//            var pimBotState = await _pimBotStateAccesors.PimBotState.GetAsync(turnContext, () => new PimBotState());

            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Perform a call to LUIS to retrieve results for the current activity message.
//                var luisResults = await _services.LuisServices[LuisConfiguration].RecognizeAsync(dc.Context, cancellationToken);

                // If any entities were updated, treat as interruption.
                // For example, "no my name is tony" will manifest as an update of the name to be "tony".
//                var topScoringIntent = luisResults?.GetTopScoringIntent();

//                var topIntent = topScoringIntent.Value.intent;


                switch (turnContext.Activity.Text)
                {
                    case "items":
                        try
                        {
                            var items = await _itemService.GetAllItemsAsync();
                            foreach (var item in items)
                            {
                                await turnContext.SendActivityAsync(item, cancellationToken: cancellationToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            await turnContext.SendActivityAsync(Messages.Messages.ServerIssue, cancellationToken: cancellationToken);
                        }

                        break;

                    default:
                        // Echo back to the user whatever they typed.
                        await turnContext.SendActivityAsync(Messages.Messages.NotUnderstand, cancellationToken: cancellationToken);

                        break;
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
                            await turnContext.SendActivityAsync($"Hi there, {member.Name} ✌️. {Messages.Messages.IntroducingMessage}", cancellationToken: cancellationToken);

                            // There would be nice typing

                            await turnContext.SendActivityAsync(Messages.Messages.HelpMessage, cancellationToken: cancellationToken);
                        }
                    }
                }
            }
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