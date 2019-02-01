// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using PimBot.State;

namespace PimBot
{
    /// <summary>
    /// Represents a bot that processes incoming activities.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service. Transient lifetime services are created
    /// each time they're requested. Objects that are expensive to construct, or have a lifetime
    /// beyond a single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class PimBot : IBot
    {
        // Messages sent to the user.

        private const string WelcomeMessage = 
            @"Let me introduce myself. My name is PimBot and I'm Bot 🤖 and your virtual assistent!
            I can help you with making and managing the orders. Simple type find and add a product
              you are looking for. ";

        //        private const string WelcomeMessage = @"This is a simple Welcome Bot sample.This bot will introduce you
        //                                                to welcoming and greeting users. You can say 'intro' to see the
        //                                                introduction card. If you are running this bot in the Bot Framework
        //                                                Emulator, press the 'Start Over' button to simulate user joining
        //                                                a bot or a channel";

        // The bot state accessor object. Use this to access specific state properties.
        private readonly PimBotStateAccesors _pimBotStateAccesors;

        /// <summary>
        /// Initializes a new instance of the <see cref="PimBot"/> class.
        /// </summary>                        
        public PimBot(PimBotStateAccesors statePropetyAccesors)
        {
            _pimBotStateAccesors = statePropetyAccesors ?? throw new System.ArgumentNullException("state accessor can't be null");
        }

        /// <summary>
        /// Every conversation turn calls this method.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {

            var pimBotState = await _pimBotStateAccesors.PimBotState.GetAsync(turnContext, () => new PimBotState());

            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                if (pimBotState.DidBotWelcomeUser == false)
                {
                    pimBotState.DidBotWelcomeUser = true;

                    await _pimBotStateAccesors.PimBotState.SetAsync(turnContext, pimBotState);
                    await _pimBotStateAccesors.UserState.SaveChangesAsync(turnContext);

                    // the channel should sends the user name in the 'From' object
                    var userName = turnContext.Activity.From.Name;

                    await turnContext.SendActivityAsync($"You are seeing this message because this was your first message ever to this bot.", cancellationToken: cancellationToken);
                    await turnContext.SendActivityAsync($"It is a good practice to welcome the user and provide personal greeting. For example, welcome {userName}.", cancellationToken: cancellationToken);

                }
                else
                {
                    // Echo back to the user whatever they typed.             
                    await turnContext.SendActivityAsync(turnContext.Activity.Text, cancellationToken: cancellationToken);

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
                            await turnContext.SendActivityAsync($"Hi there, {member.Name} ✌️. {WelcomeMessage}", cancellationToken: cancellationToken);
//                            await turnContext.SendActivityAsync(InfoMessage, cancellationToken: cancellationToken);
//                            await turnContext.SendActivityAsync(PatternMessage, cancellationToken: cancellationToken);
                        }
                    }
                }

            }
        }
    }
}
