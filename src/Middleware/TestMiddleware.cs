using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

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
