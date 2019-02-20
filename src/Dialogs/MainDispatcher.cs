using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Logging;
using PimBot;
using PimBot.Messages;
using PimBotDp.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PimBotDp.Dialogs.AddItem;

namespace PimBotDp.Dialogs
{
    public class MainDispatcher : ComponentDialog
    {
        private const string MainDispatcherStateProperty = "mainDispatcherState";

        private readonly BotServices _services;
        private readonly ILogger _logger;
        private readonly IStatePropertyAccessor<OnTurnState> _onTurnAccessor;
        private readonly IStatePropertyAccessor<CartState> _cartStateAccessor;
        private readonly IStatePropertyAccessor<DialogState> _mainDispatcherAccessor;
        private readonly DialogSet _dialogs;

        public MainDispatcher(BotServices services, IStatePropertyAccessor<OnTurnState> onTurnAccessor, UserState userState, ConversationState conversationState,
            ILoggerFactory loggerFactory)
            : base(nameof(MainDispatcher))
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _logger = loggerFactory.CreateLogger<MainDispatcher>();
            _onTurnAccessor = onTurnAccessor;
            _mainDispatcherAccessor = conversationState.CreateProperty<DialogState>(MainDispatcherStateProperty);

            if (conversationState == null)
            {
                throw new ArgumentNullException(nameof(conversationState));
            }

            if (userState == null)
            {
                throw new ArgumentNullException(nameof(userState));
            }

            _cartStateAccessor = userState.CreateProperty<CartState>(nameof(CartState));
            _dialogs = new DialogSet(_mainDispatcherAccessor);
            AddDialog(new AddItemDialog(services, onTurnAccessor, _cartStateAccessor));
        }

        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await MainDispatch(innerDc);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await MainDispatch(innerDc);
        }

        protected async Task<DialogTurnResult> MainDispatch(DialogContext innerDc)
        {
            var context = innerDc.Context;

            var onTurnState = await _onTurnAccessor.GetAsync(context, () => new OnTurnState());

            // Evaluate if the requested operation is possible/ allowed.
            //            var activeDialog = (innerDc.ActiveDialog != null) ? innerDc.ActiveDialog.Id : string.Empty;
            //            var reqOpStatus = IsRequestedOperationPossible(activeDialog, onTurnProperty.Intent);
            //            if (!reqOpStatus.allowed)
            //            {
            //                await context.SendActivityAsync(reqOpStatus.reason);
            //
            //                // Nothing to do here. End main dialog.
            //                return await innerDc.EndDialogAsync();
            //            }

            // Continue outstanding dialogs.
            var dialogTurnResult = await innerDc.ContinueDialogAsync();

            if (!context.Responded && dialogTurnResult != null && dialogTurnResult.Status != DialogTurnStatus.Complete)
            {
                //                No one has responded so start the right child dialog.
                dialogTurnResult = await BeginChildDialogAsync(innerDc, onTurnState);
            }

            if (dialogTurnResult == null)
            {
                return await innerDc.EndDialogAsync();
            }

            // Examine result from dc.continue() or from the call to beginChildDialog().
            switch (dialogTurnResult.Status)
            {
                case DialogTurnStatus.Complete:
                    // The active dialog finished successfully. Ask user if they need help with anything else.
                    //        await context.SendActivityAsync(MessageFactory.SuggestedActions(Helpers.GenSuggestedQueries(), "Is there anything else I can help you with ?"));
                    break;

                case DialogTurnStatus.Waiting:
                    // The active dialog is waiting for a response from the user, so do nothing
                    break;

                case DialogTurnStatus.Cancelled:
                    // The active dialog's stack has been canceled
                    await innerDc.CancelAllDialogsAsync();
                    break;
            }

            return dialogTurnResult;
        }

        protected async Task<DialogTurnResult> BeginChildDialogAsync(DialogContext dc, OnTurnState onTurnState)
        {
            var context = dc.Context;

            // Continue outstanding dialogs.
            var dialogTurnResult = await dc.ContinueDialogAsync();

            switch (onTurnState.Intent)
            {
                case Intents.FindItem:
                    if (onTurnState.Entities["keyPhrase"].Count() > 0)
                    {
                        var firstEntity = (string)onTurnState.Entities["keyPhrase"].First;
                         await context.SendActivityAsync("Entity is: " + firstEntity);
                    }

                    break;

                case Intents.AddItem:
                    return await dc.BeginDialogAsync(AddItemDialog.Name);

                case Intents.ShowCart:
                    var cartState1 =
                        await _cartStateAccessor.GetAsync(context, () => new CartState());
                    if (cartState1.Items == null || cartState1.Items.Count == 0)
                    {
                         await context.SendActivityAsync(Messages.EmptyCart);
                         await context.SendActivityAsync(Messages.SuggestHelp);
                    }
                    else
                    {
                        string itemsInCart = Messages.ShowCartTitle + Environment.NewLine;
                        foreach (var item in cartState1.Items)
                        {
                            if (item.Count > 0)
                            {
                                itemsInCart += $"* {item.Name} ({item.Count}PCS) {Environment.NewLine}";
                            }
                        }

                        itemsInCart += Environment.NewLine + Environment.NewLine +
                                       Messages.ShowCartAfter;

                         await context.SendActivityAsync(itemsInCart);
                    }

                    break;

                case Intents.None:
                    await context.SendActivityAsync(Messages.NotUnderstand);
                    await context.SendActivityAsync(Messages.SuggestHelp);
                    break;

                case Intents.Help:
                    await context.SendActivityAsync(Messages.HelpMessage);
                    break;

//                case "items":
//                    try
//                    {
//                        var items = await _itemService.GetAllItemsAsync();
//                        foreach (var item in items)
//                        {
//                            await turnContext.SendActivityAsync(item,
//                                cancellationToken: cancellationToken);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        await turnContext.SendActivityAsync(Messages.Messages.ServerIssue,
//                            cancellationToken: cancellationToken);
//                    }
//
//                    break;

                default:
                    await context.SendActivityAsync(Messages.NotUnderstand);
                    break;
            }

            return dialogTurnResult;
        }

        //        // Method to evaluate if the requested user operation is possible.
        //        // User could be in the middle of a multi-turn dialog where interruption might not be possible or allowed.
        //        protected (bool allowed, string reason) IsRequestedOperationPossible(string activeDialog, string requestedOperation)
        //        {
        //            (bool allowed, string reason) outcome = (true, string.Empty);
        //
        //            // E.g. What_can_you_do is not possible when you are in the middle of Who_are_you dialog
        //            if (requestedOperation.Equals(WhatCanYouDo.Name))
        //            {
        //                if (activeDialog.Equals(WhatCanYouDo.Name))
        //                {
        //                    outcome.allowed = false;
        //                    outcome.reason = "Sorry! I'm unable to process that. You can say 'cancel' to cancel this conversation.";
        //                }
        //            }
        //            else if (requestedOperation.Equals(CancelIntent))
        //            {
        //                if (string.IsNullOrWhiteSpace(activeDialog))
        //                {
        //                    outcome.allowed = false;
        //                    outcome.reason = "Sure, but there is nothing to cancel...";
        //                }
        //            }
        //
        //            return outcome;
        //        }
    }
}
