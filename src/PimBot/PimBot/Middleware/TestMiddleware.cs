using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PimBot.Middleware
{
    public class TestMiddleware : IMiddleware
    {
        public async Task OnTurnAsync(ITurnContext context, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            await context.SendActivityAsync($"[TestMiddleware] {context.Activity.Type}/OnTurn/Before");

            await next(cancellationToken);

            await context.SendActivityAsync($"[TestMiddleware] {context.Activity.Type}/OnTurn/After");

        }
    }
}
