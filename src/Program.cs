// ===============================
// Author: Miroslav Nov�k (xnovak1k@stud.fit.vutbr.cz)
// Create date:
// ===

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.BotBuilderSamples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    // Add Azure Logging
             //       logging.AddAzureWebAppDiagnostics();

                    // Other Loggers.
                    // There are other logging options available:
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1
                    // logging.AddDebug();
                    // logging.AddConsole();
                })

                // Application Insights.
                // An alternative logging and metrics service for your application.
                // https://azure.microsoft.com/en-us/services/application-insights/
                // .UseApplicationInsights()
                .UseStartup<Startup>()
                .Build();
    }
}
