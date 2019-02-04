using FeaturesPimServiceReference;
using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Simple.OData.Client;


namespace PimBot.Middleware
{
    public class TestMiddleware : IMiddleware
    {
        public async Task OnTurnAsync(ITurnContext context, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            await context.SendActivityAsync($"[TestMiddleware] {context.Activity.Type}/OnTurn/Before");

            await next(cancellationToken);

            ODataClientSettings settings = new ODataClientSettings();
            settings.BaseUri = new Uri("http://pimicschatbot.westeurope.cloudapp.azure.com:7048/NAV/OData/");

            settings.Credentials = new NetworkCredential("Allium", "#Allium12345$");

            var client = new ODataClient(settings);

            var featuresPIM = await client
                .For("FeaturesPIM")
                .FindEntriesAsync();


            var e = featuresPIM.First();

            var s = e["Code"];


            //            FeaturesPIM featuresPim = new FeaturesPIM();

            //            FeaturesPIM_PortClient featuresPimPortClient = new FeaturesPIM_PortClient();
            //
            //            featuresPimPortClient.featuresPimPortClient.CreateAsync();


            //            var featuresPimPort = new FeaturesPIM_PortClient(new Uri("http://pimicschatbot.westeurope.cloudapp.azure.com:7047/NAV/WS/CRONUS%20International%20Ltd"));


            //           var result1 = await featuresPimPort.ReadByRecIdAsync("0");
            //       var result = await featuresPimPort.ReadAsync("INTERNAL", "Item", "");
            //
            //            var fuu = featuresPimPort.State;

            //            var valules = result.FeaturesPIM.Values;


            await context.SendActivityAsync($"[TestMiddleware] first code: {s} and {context.Activity.Type}/OnTurn/After");

        }
    }
}
