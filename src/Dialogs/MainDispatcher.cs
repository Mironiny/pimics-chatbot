﻿// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 18.02.2019
// ===============================

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Logging;
using PimBot.Services;
using PimBot.Services.Impl;
using PimBot.State;

namespace PimBot.Dialogs
{
    /// <summary>
    /// Main class for handling dialogs.
    /// </summary>
    public class MainDispatcher : ComponentDialog
    {
        private const string MainDispatcherStateProperty = "mainDispatcherState";

        private readonly BotServices _services;
        private readonly ILogger _logger;
        private readonly IStatePropertyAccessor<OnTurnState> _onTurnAccessor;
        private readonly IStatePropertyAccessor<CartState> _cartStateAccessor;
        private readonly IStatePropertyAccessor<CustomerState> _customerStateAccessor;
        private readonly IStatePropertyAccessor<DialogState> _mainDispatcherAccessor;
        private readonly UserState _userState;
        private readonly ConversationState _conversationState;
        private readonly ISmallTalkService _smallTalkService = new SmallTalkService();

        private readonly DialogSet _dialogs;

        public MainDispatcher(
            BotServices services,
            IStatePropertyAccessor<OnTurnState> onTurnAccessor,
            UserState userState,
            ConversationState conversationState,
            ILoggerFactory loggerFactory,
            IPimbotServiceProvider provider)
            : base(nameof(MainDispatcher))
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _logger = loggerFactory.CreateLogger<MainDispatcher>();
            _onTurnAccessor = onTurnAccessor;
            _mainDispatcherAccessor = conversationState.CreateProperty<DialogState>(MainDispatcherStateProperty);
            _userState = userState;
            _conversationState = conversationState;
            _customerStateAccessor = conversationState.CreateProperty<CustomerState>("customerProperty");

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
            AddDialog(new AddItemDialog(services, onTurnAccessor, _cartStateAccessor, provider));
            AddDialog(new RemoveItemDialog(services, onTurnAccessor, _cartStateAccessor));
            AddDialog(new GetUserInfoDialog(services, onTurnAccessor, _cartStateAccessor, _customerStateAccessor, provider));
            AddDialog(new FindItemDialog(services, onTurnAccessor, _cartStateAccessor, provider));
            AddDialog(new ShowCartDialog(services, onTurnAccessor, _cartStateAccessor, provider));
            AddDialog(new ShowOrdersDialog(services, onTurnAccessor, provider));
            AddDialog(new ShowCategoriesDialog(services, onTurnAccessor, provider));
            AddDialog(new DetailItemDialog(services, onTurnAccessor, provider));
        }

        protected override async Task<DialogTurnResult> OnBeginDialogAsync(
            DialogContext innerDc,
            object options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await MainDispatch(innerDc);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(
            DialogContext innerDc,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await MainDispatch(innerDc);
        }

        protected async Task<DialogTurnResult> MainDispatch(DialogContext innerDc)
        {
            var context = innerDc.Context;

            var onTurnState = await _onTurnAccessor.GetAsync(context, () => new OnTurnState());

            // See if there are any conversation interrupts we need to handle.
            if (onTurnState.Intent.Equals(Intents.Cancel))
            {
                if (innerDc.ActiveDialog != null)
                {
                    FindItemDialog.didYouMean = null;
                    await _conversationState.SaveChangesAsync(innerDc.Context);
                    await _userState.SaveChangesAsync(innerDc.Context);
                    await innerDc.Context.SendActivityAsync(Messages.InteruptionCancelConfirm);
                    await innerDc.Context.SendActivityAsync(Messages.IsThereAnythingICanDo);

                    return await innerDc.CancelAllDialogsAsync();
                }
                else
                {
                    await innerDc.Context.SendActivityAsync(Messages.InteruptionCancelNotConfirm);
                }
            }

            // Continue outstanding dialogs.
            var dialogTurnResult = await innerDc.ContinueDialogAsync();

            if (!context.Responded && dialogTurnResult != null && dialogTurnResult.Status != DialogTurnStatus.Complete)
            {
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
                    await context.SendActivityAsync(Messages.IsThereAnythingICanDo);
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
                    return await dc.BeginDialogAsync(FindItemDialog.Name);

                case Intents.ShowCategories:
                    return await dc.BeginDialogAsync(ShowCategoriesDialog.Name);

                case Intents.AddItem:
                    return await dc.BeginDialogAsync(AddItemDialog.Name);

                case Intents.RemoveItem:
                    return await dc.BeginDialogAsync(RemoveItemDialog.Name);

                case Intents.Confirm:
                    var customerState =
                        await _customerStateAccessor.GetAsync(context, () => new CustomerState());
                    return await dc.BeginDialogAsync(GetUserInfoDialog.Name);

                case Intents.ShowCart:
                    return await dc.BeginDialogAsync(ShowCartDialog.Name);

                case Intents.ShowOrders:
                    return await dc.BeginDialogAsync(ShowOrdersDialog.Name);

                case Intents.DetailItem:
                    return await dc.BeginDialogAsync(DetailItemDialog.Name);

                case Intents.SmallTalk:
                    var inputMessage = context.Activity.Text;
                    var result = await _smallTalkService.GetSmalltalkAnswer(inputMessage);
                    var customerState1 =
                        await _customerStateAccessor.GetAsync(context, () => new CustomerState());

                    await context.SendActivityAsync(result);

                    // If user have smalltalk 3 times, show help
                    if (customerState1.SmallTalkCount == 3)
                    {
                        customerState1.SmallTalkCount = 0;
                        await context.SendActivityAsync(Messages.SuggestHelp);
                    }
                    else
                    {
                        customerState1.SmallTalkCount += 1;
                    }

                    await _customerStateAccessor.SetAsync(context, customerState1);
                    break;

                case Intents.None:
                    await context.SendActivityAsync(Messages.NotUnderstand);
                    await context.SendActivityAsync(Messages.SuggestHelp);
                    break;

                case Intents.Help:
                    await context.SendActivityAsync(Messages.HelpMessage);
                    break;

                case Intents.Cancel:
                    await context.SendActivityAsync(Messages.InteruptionCancelNotConfirm);
                    break;

                default:
                    await context.SendActivityAsync(Messages.NotUnderstand);
                    await context.SendActivityAsync(Messages.SuggestHelp);
                    break;
            }

            return dialogTurnResult;
        }
    }
}