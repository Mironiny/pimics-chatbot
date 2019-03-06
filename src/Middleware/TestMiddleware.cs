using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Simple.OData.Client;


namespace PimBotDp.Middleware
{
    public class TestMiddleware : IMiddleware
    {
        public async Task OnTurnAsync(ITurnContext context, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
//            await context.SendActivityAsync($"[TestMiddleware] {context.Activity.Type}/OnTurn/Before");
            await next(cancellationToken);
        }
    }
}
