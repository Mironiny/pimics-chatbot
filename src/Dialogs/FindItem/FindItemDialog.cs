using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using PimBot.Dialogs.AddItem;
using PimBot.Service;
using PimBot.Service.Impl;
using PimBot.Services.Impl;
using PimBot.State;

namespace PimBot.Dialogs.FindItem
{
    public class FindItemDialog : ComponentDialog
    {
        private static readonly int ShowItemsDialogIndex = 5;
        private static bool AskedForFeature;
        private static int questionCounter;
        private static List<string> questionAkedList = new List<string>();

        public const string Name = "Find_item";
        private readonly BotServices _services;

        private const string ShowAllItemsPrompt = "ShowAllItemsPrompt";
        private const string AskForPropertyPrompt = "AskForPropertyPrompt";

        private IStatePropertyAccessor<OnTurnState> _onTurnAccessor;
        private IStatePropertyAccessor<CartState> _cartStateAccessor;
        private readonly IItemService _itemService = new ItemService();
        private readonly ICategoryService _categoryService = new CategoryService();

        private static IEnumerable<PimItem> pimItems = new List<PimItem>();
        private static List<FeatureToAsk> featuresToAsk = new List<FeatureToAsk>();

        // Prompts names
        private const string CountPrompt = "countPrompt";

        public FindItemDialog(BotServices services, IStatePropertyAccessor<OnTurnState> onTurnAccessor, IStatePropertyAccessor<CartState> cartStateAccessor)
            : base(Name)
        {
            _services = services;
            _onTurnAccessor = onTurnAccessor;
            _cartStateAccessor = cartStateAccessor;

            // Add dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                InitializeStateStepAsync,
                ResolveShowItemDialog,
                AskByAttributesDialog,
                ResolveAttributeFiltering,
                AskWhatNext,
                ResolveShowItems,
                PrintItems,
            };
            AddDialog(new WaterfallDialog(
                "start",
                waterfallSteps));
            AddDialog(new ChoicePrompt(ShowAllItemsPrompt));
            AddDialog(new ChoicePrompt(AskForPropertyPrompt));
            AddDialog(new ShowCategoriesDialog(services, onTurnAccessor));
        }

        private async Task<DialogTurnResult> InitializeStateStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            // Initalization of static variables
            AskedForFeature = false;
            questionCounter = 0;
            questionAkedList = new List<string>();

            var context = stepContext.Context;
            var onTurnProperty = await _onTurnAccessor.GetAsync(context, () => new OnTurnState());
            if (onTurnProperty.Entities[EntityNames.FindItem] != null && onTurnProperty.Entities[EntityNames.FindItem].Count() > 0)
            {
                var firstEntity = (string)onTurnProperty.Entities[EntityNames.FindItem].First;

                pimItems = await _itemService.GetAllItemsAsync(firstEntity);
                var groupCount = _itemService.GetAllItemsCategory(pimItems).Count();

                if (pimItems.Count() == 0)
                {
                    await context.SendActivityAsync($"{Messages.NotFound} **{firstEntity}**");
                    return await stepContext.EndDialogAsync();
                }
                else
                {
                    await context.SendActivityAsync($"I've found {pimItems.Count()} potentional {firstEntity} in {groupCount} groups.");

                    return await stepContext.PromptAsync(
                        ShowAllItemsPrompt,
                        new PromptOptions
                        {
                            Choices = ChoiceFactory.ToChoices(new List<string> { Messages.FindItemShowAllItem, Messages.FindItemSpecialize }),
                        },
                        cancellationToken);
                }
            }

            await context.SendActivityAsync(Messages.FindItemForgotItem);
            var categories = await _categoryService.GetAllProductGroupAsync();
            await context.SendActivityAsync(ShowCategoriesDialog.GetPritableGroup(categories));

            return await stepContext.EndDialogAsync();
        }


        /// <summary>
        /// Resolve everything is OK.
        /// </summary>
        private async Task<DialogTurnResult> ResolveShowItemDialog(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var choice = stepContext.Result as FoundChoice;

            if (choice.Value.Equals(Messages.FindItemShowAllItem))
            {
                if (pimItems.Count() < 10)
                {
                    foreach (var item in pimItems)
                    {
                        var response = stepContext.Context.Activity.CreateReply();
                        response.Attachments = new List<Attachment>() { CreateAdaptiveCardUsingSdk(item) };
                        await context.SendActivityAsync(response);
                    }

                    await context.SendActivityAsync(Messages.FindItemAddToCart);
                }
                else
                {
                    await context.SendActivityAsync(GetPrintableListItems(pimItems));
                    await context.SendActivityAsync(Messages.FindItemAddToCart);
                }

                return await stepContext.EndDialogAsync();
            }
            else
            {
                await context.SendActivityAsync(Messages.FindItemStartSpecialize);
                return await stepContext.ContinueDialogAsync();
            }
        }

        /// <summary>
        /// Resolve everything is OK.
        /// </summary>
        private async Task<DialogTurnResult> AskByAttributesDialog(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            if (!AskedForFeature)
            {
                featuresToAsk = await _itemService.GetAllAttributes(pimItems);
                AskedForFeature = true;
            }

            if (!featuresToAsk.Any())
            {
                await context.SendActivityAsync(Messages.FindItemNothingToAsk);
                stepContext.ActiveDialog.State["stepIndex"] = ShowItemsDialogIndex;
                return await stepContext.ContinueDialogAsync();
            }

            var feature = featuresToAsk[0];

            var choices = feature.GetPrintableValues();
            choices.Add(Messages.Skip);
            return await stepContext.PromptAsync(
                AskForPropertyPrompt,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text($"{GetPrepositon()} **{feature.Description}**?"),
                    Choices = ChoiceFactory.ToChoices(choices),
                },
                cancellationToken);
        }

        /// <summary>
        /// Resolve everything is OK.
        /// </summary>
        private async Task<DialogTurnResult> ResolveAttributeFiltering(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var choice = stepContext.Result as FoundChoice;

            // Raise counter
            questionCounter++;

            if (!(choice.Value == Messages.Skip))
            {
                // Do some filtering
                if (featuresToAsk[0].Type == FeatureType.Alphanumeric)
                {
                    pimItems = await _itemService.FilterItemsByFeature(pimItems, featuresToAsk[0], choice.Value);
                }
                else
                {
                    pimItems = await _itemService.FilterItemsByFeature(pimItems, featuresToAsk[0],
                        featuresToAsk[0].GetMedianValue().ToString(), choice.Index);
                }

                questionAkedList.Add(featuresToAsk[0].Number);

                featuresToAsk = await _itemService.GetAllAttributes(pimItems);
                featuresToAsk = RemoveFromList(featuresToAsk, questionAkedList);

                if (!featuresToAsk.Any())
                {
                    await context.SendActivityAsync(Messages.FindItemNothingToAsk);
                    stepContext.ActiveDialog.State["stepIndex"] = ShowItemsDialogIndex;
                    return await stepContext.ContinueDialogAsync();
                }
                else if (pimItems.Count() == 0)
                {
                    // Nothing find
                    await context.SendActivityAsync(Messages.FindItemNotFound);
                    return await stepContext.EndDialogAsync();
                }
                else if (pimItems.Count() == 1)
                {
                    // Jump to end
                    return await stepContext.ContinueDialogAsync();
                }
            }
            else
            {
                questionAkedList.Add(featuresToAsk[0].Number);
                featuresToAsk.RemoveAt(0);
            }

  //          featuresToAsk.RemoveAt(0);
            if (!(questionCounter % Constants.QuestionLimit == 0))
            {
                // Once in a time ask if user want continue with question 
                stepContext.ActiveDialog.State["stepIndex"] = 1;
            }

            return await stepContext.ContinueDialogAsync();
        }

        /// <summary>
        /// Resolve everything is OK.
        /// </summary>
        private async Task<DialogTurnResult> AskWhatNext(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var groups = _itemService.GetAllItemsCategory(pimItems);

            if (pimItems.Count() == 1)
            {
                await context.SendActivityAsync(Messages.FindItemFindItem);
                stepContext.ActiveDialog.State["stepIndex"] = ShowItemsDialogIndex;
                return await stepContext.ContinueDialogAsync();
            }

            await context.SendActivityAsync($"I've found {pimItems.Count()} items in {groups.Count()} groups.");
            return await stepContext.PromptAsync(
                ShowAllItemsPrompt,
                new PromptOptions
                {
                    Choices = ChoiceFactory.ToChoices(new List<string> { Messages.FindItemShowAllItem, Messages.FindItemSpecializeContinue }),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> ResolveShowItems(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var choice = stepContext.Result as FoundChoice;

            if (choice.Value.Equals(Messages.FindItemShowAllItem))
            {
                stepContext.ActiveDialog.State["stepIndex"] = ShowItemsDialogIndex;
                return await stepContext.ContinueDialogAsync();
            }
            else
            {
                stepContext.ActiveDialog.State["stepIndex"] = 1;
                return await stepContext.ContinueDialogAsync();
            }
        }

        private async Task<DialogTurnResult> PrintItems(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var context = stepContext.Context;

            if (pimItems.Count() < 10)
            {
                foreach (var item in pimItems)
                {
                    var response = stepContext.Context.Activity.CreateReply();
                    response.Attachments = new List<Attachment>() { CreateAdaptiveCardUsingSdk(item) };
                    await context.SendActivityAsync(response);
                }

                await context.SendActivityAsync(Messages.FindItemAddToCart);
            }
            else
            {
                await context.SendActivityAsync(GetPrintableListItems(pimItems));
                await context.SendActivityAsync(Messages.FindItemAddToCart);
            }

            return await stepContext.EndDialogAsync();
        }

        private string GetPrintableListItems(IEnumerable<PimItem> items)
        {
            string result = string.Empty;
            foreach (var item in items)
            {
                result += $"**No**: {item.No}, desciption: {item.Description}, unit price: {item.Unit_Price}/{item.Base_Unit_of_Measure} {Environment.NewLine}";
            }

            return result;
        }

        private Attachment CreateAdaptiveCardUsingSdk(PimItem item)
        {
            var card = new AdaptiveCard();
            card.Body.Add(new AdaptiveTextBlock()
            {
                Text = $"**{Messages.No}**: {item.No}",
                Size = AdaptiveTextSize.Medium,
                Weight = AdaptiveTextWeight.Bolder,
            });
            card.Body.Add(new AdaptiveTextBlock()
            {
                Text = $"**{Messages.Description}**: {item.Description}",
                Size = AdaptiveTextSize.Medium,
                Weight = AdaptiveTextWeight.Bolder,
            });
            card.Body.Add(new AdaptiveTextBlock()
            {
                Text = $"**{Messages.Price}**: {item.Unit_Price}/{item.Base_Unit_of_Measure}",
                Size = AdaptiveTextSize.Medium,
                Weight = AdaptiveTextWeight.Bolder,
            });
            //            card.Actions.Add(new AdaptiveSubmitAction() { Title = "Submit" });
            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }

        private static int position = 0;
        private string GetPrepositon()
        {
            if (position >= Messages.FindItemQuestionStart.Length)
            {
                position = 0;
            }

            return Messages.FindItemQuestionStart[position++];
        }

        private List<FeatureToAsk> RemoveFromList(List<FeatureToAsk> features, List<string> numbers)
        {
            foreach (var number in numbers)
            {
                var item = featuresToAsk.SingleOrDefault(x => x.Number == number);
                if (item != null)
                {
                    featuresToAsk.Remove(item);
                }
            }

            return features;

        }

    }
}
