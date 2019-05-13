// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 19.3.2019
// ===============================

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using PimBot.Dto;
using PimBot.Services;
using PimBot.State;

namespace PimBot.Dialogs
{
    /// <summary>
    /// Class represents detail of item dialog.
    /// </summary>
    public class DetailItemDialog : ComponentDialog
    {
        public const string Name = "DetailItem";

        private readonly IItemService _itemService;
        private readonly IFeatureService _featureService;

        private readonly BotServices _services;
        private IStatePropertyAccessor<OnTurnState> _onTurnAccessor;

        public DetailItemDialog(BotServices services, IStatePropertyAccessor<OnTurnState> onTurnAccessor, IPimbotServiceProvider provider)
            : base(Name)
        {
            _services = services;
            _onTurnAccessor = onTurnAccessor;
            _itemService = provider.ItemService;
            _featureService = provider.FeatureService;

            // Add dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                InitializeStateStepAsync,
            };
            AddDialog(new WaterfallDialog(
                "start",
                waterfallSteps));
        }

        private async Task<DialogTurnResult> InitializeStateStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var onTurnProperty = await _onTurnAccessor.GetAsync(context, () => new OnTurnState());
            if (onTurnProperty.Entities[EntityNames.Item] != null && onTurnProperty.Entities[EntityNames.Item].Count() > 0)
            {
                var firstEntity = (string) onTurnProperty.Entities[EntityNames.Item].First;

                var pimItem = await _itemService.FindItemByNo(firstEntity);

                if (pimItem == null)
                {
                    await context.SendActivityAsync($"{Messages.NotFound} **{firstEntity}**.");
                    return await stepContext.EndDialogAsync();
                }

                var features = await _featureService.GetFeaturesByNoAsync(pimItem.No);
                var response = stepContext.Context.Activity.CreateReply();
                var pictureUrl = await _itemService.GetImageUrl(pimItem);

                response.Attachments = new List<Attachment>() { CreateAdaptiveCardDetailUsingSdk(pimItem, features, pictureUrl) };
                await context.SendActivityAsync(response);
            }

            return await stepContext.EndDialogAsync();
        }

        private Attachment CreateAdaptiveCardDetailUsingSdk(PimItem item, List<PimFeature> features, string pictureUrl)
        {
            var card = new AdaptiveCard();
            if (pictureUrl != null)
            {
                card.Body.Add(new AdaptiveImage()
                {
                    Type = "Image",
                    UrlString = pictureUrl,
                    HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
                    Size = AdaptiveImageSize.Large,
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

            foreach (var feature in features)
            {
                card.Body.Add(new AdaptiveTextBlock()
                {
                    Text = $"{feature.Description}: {feature.Value}",
                    Size = AdaptiveTextSize.Medium,
                    Weight = AdaptiveTextWeight.Bolder,
                });
            }

            card.Actions.Add(new AdaptiveSubmitAction() { Title = Messages.FindItemAddToCartButton, Data = $"add {item.No}" });

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };
        }
    }
}
