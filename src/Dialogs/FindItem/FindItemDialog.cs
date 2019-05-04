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
using PimBot.State;
using PimBotDp.Services;

namespace PimBot.Dialogs.FindItem
{
    public class FindItemDialog : ComponentDialog
    {
        public const string Name = "Find_item";

        private const string ShowAllItemsPrompt = "ShowAllItemsPrompt";
        private const string AskForPropertyPrompt = "AskForPropertyPrompt";
        private const string DidYouMeanPrompt = "DidYouMean";
        private const string CountPrompt = "countPrompt";

        private static readonly int ShowItemsDialogIndex = 5;
        private static bool askedForFeature;
        private static int questionCounter;
        private static List<string> questionAkedList = new List<string>();
        private static IEnumerable<PimItem> pimItems = new List<PimItem>();
        private static List<FeatureToAsk> featuresToAsk = new List<FeatureToAsk>();

        // Variable serve for savings didYouMean recomendation. If is empty that means that we should not look against this variable
        public static string didYouMean = null;

        private readonly IItemService _itemService;
        private readonly ICategoryService _categoryService;
        private readonly BotServices _services;

        private IStatePropertyAccessor<OnTurnState> _onTurnAccessor;
        private IStatePropertyAccessor<CartState> _cartStateAccessor;

        public FindItemDialog(BotServices services, IStatePropertyAccessor<OnTurnState> onTurnAccessor, IStatePropertyAccessor<CartState> cartStateAccessor, IPimbotServiceProvider provider)
            : base(Name)
        {
            _services = services;
            _onTurnAccessor = onTurnAccessor;
            _cartStateAccessor = cartStateAccessor;
            _itemService = provider.ItemService;
            _categoryService = provider.CategoryService;

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
            AddDialog(new ChoicePrompt(DidYouMeanPrompt));
            AddDialog(new ChoicePrompt(AskForPropertyPrompt));
            AddDialog(new ShowCategoriesDialog(services, onTurnAccessor, provider));
        }

        private async Task<DialogTurnResult> InitializeStateStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            // Initalization of static variables
            askedForFeature = false;
            questionCounter = 0;
            questionAkedList = new List<string>();

            var context = stepContext.Context;
            var onTurnProperty = await _onTurnAccessor.GetAsync(context, () => new OnTurnState());

            if (didYouMean != null || (onTurnProperty.Entities[EntityNames.FindItem] != null &&
                                       onTurnProperty.Entities[EntityNames.FindItem].Count() > 0))
            {
                string firstEntity;
                if (didYouMean == null)
                {
                    firstEntity = (string)onTurnProperty.Entities[EntityNames.FindItem].First;
                }
                else
                {
                    firstEntity = didYouMean;
                }

                // Get all items
                pimItems = await _itemService.GetAllItemsByMatchAsync(firstEntity);
                //                var groupCount = _itemService.GetAllItemsCategory(pimItems).Count();

                if (didYouMean != null)
                {
                    didYouMean = null;
                }

                if (pimItems.Count() == 0)
                {
                    await context.SendActivityAsync($"{Messages.NotFound} **{firstEntity}**.");

                    // Try to find if user doesnt do type error
                    didYouMean = await _itemService.FindSimilarItemsByDescription(firstEntity);

                    return await stepContext.PromptAsync(
                        DidYouMeanPrompt,
                        new PromptOptions
                        {
                            Prompt = MessageFactory.Text(string.Format(Messages.FindItemDidYouMean, didYouMean)),
                            RetryPrompt = MessageFactory.Text(string.Format(Messages.FindItemDidYouMean, didYouMean) + Messages.CancelPrompt),
                            Choices = ChoiceFactory.ToChoices(new List<string> { Messages.Yes, Messages.No }),
                        },
                        cancellationToken);
                }
                else if (pimItems.Count() == 1)
                {
                    stepContext.ActiveDialog.State["stepIndex"] = ShowItemsDialogIndex;
                    return await stepContext.ContinueDialogAsync();
                }
                else
                {
                    var prompt = string.Format(Messages.FindItemFound, pimItems.Count(), firstEntity) + Messages.WhatToDoPrompt;

                    return await stepContext.PromptAsync(
                        ShowAllItemsPrompt,
                        new PromptOptions
                        {
                            Prompt = MessageFactory.Text(prompt),
                            RetryPrompt = MessageFactory.Text(prompt + Messages.CancelPrompt),
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

            // There were no file find and user get didyoumean message
            if (choice.Value.Contains(Messages.Yes) || choice.Value.Contains(Messages.No))
            {
                if (choice.Value.Contains(Messages.Yes))
                {
                    stepContext.ActiveDialog.State["stepIndex"] = -1;
                    return await stepContext.ContinueDialogAsync();
                }
                else
                {
                    didYouMean = null;
                    return await stepContext.EndDialogAsync();
                }
            }

            if (choice.Value.Equals(Messages.FindItemShowAllItem))
            {
                if (pimItems.Count() < 10)
                {
                    foreach (var item in pimItems)
                    {
                        var response = stepContext.Context.Activity.CreateReply();
                        var pictureUrl = await _itemService.GetImageUrl(item);
                        //                        response.Attachments.Add(new Attachment()
                        //                        {
                        //                            ContentUrl = pictureUrl,
                        //                            ContentType = "image/jpeg",
                        //                        });
                        //
                        //                        await context.SendActivityAsync(response);


                        response.Attachments = new List<Attachment>() { CreateAdaptiveCardUsingSdk(item, pictureUrl) };
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
            if (!askedForFeature)
            {
                featuresToAsk = await _itemService.GetAllFeaturesToAsk(pimItems);
                askedForFeature = true;
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
            var prompt = $"{GetPrepositon()} **{feature.Description}**?";
            return await stepContext.PromptAsync(
                AskForPropertyPrompt,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(prompt),
                    RetryPrompt = MessageFactory.Text(prompt + Messages.CancelPrompt),
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
                    pimItems = await _itemService.FilterItemsByFeature(
                        pimItems,
                        featuresToAsk[0],
                        featuresToAsk[0].GetMedianValue().ToString(),
                        choice.Index);
                }

                questionAkedList.Add(featuresToAsk[0].Number);

                featuresToAsk = await _itemService.GetAllFeaturesToAsk(pimItems);
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

            if (pimItems.Count() == 1)
            {
                await context.SendActivityAsync(Messages.FindItemFindItem);
                stepContext.ActiveDialog.State["stepIndex"] = ShowItemsDialogIndex;
                return await stepContext.ContinueDialogAsync();
            }

            await context.SendActivityAsync($"I've found {pimItems.Count()} items.");
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

            var reply1 = stepContext.Context.Activity.CreateReply();

            if (pimItems.Count() < 10)
            {
                foreach (var item in pimItems)
                {
                    var pictureUrl = await _itemService.GetImageUrl(item);

                    var response = stepContext.Context.Activity.CreateReply();

                    response.Attachments = new List<Attachment>() { CreateAdaptiveCardUsingSdk(item, pictureUrl) };
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

        private Attachment CreateAdaptiveCardUsingSdk(PimItem item, string pictureUrl)
        {
            var card = new AdaptiveCard();
            if (pictureUrl != null)
            {
                card.Body.Add(new AdaptiveImage()
                {
                    Type = "Image",
                    UrlString = pictureUrl,
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
                    Size = AdaptiveImageSize.Large
                });
            }

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
            card.Actions.Add(new AdaptiveSubmitAction() { Title = Messages.FindItemAddToCartButton, Data = $"add {item.No}" });
            card.Actions.Add(new AdaptiveSubmitAction() { Title = Messages.FindItemShowDetailButton, Data = $"detail {item.No}" });
            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
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
