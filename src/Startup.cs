// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PimBot;
using PimBot.Repositories.Impl;
using PimBot.Service;
using PimBot.Service.Impl;
using PimBot.Services.Impl;
using PimBotDp.Services;
using PimBotDp.Services.Impl;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// The Startup class configures services and the app's request pipeline.
    /// </summary>
    public class Startup
    {
        private ILoggerFactory _loggerFactory;
        private bool _isProduction = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </summary>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-2.1"/>
        public Startup(IHostingEnvironment env)
        {
            _isProduction = env.IsProduction();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        /// <summary>
        /// Gets the configuration that represents a set of key/value application configuration properties.
        /// </summary>
        /// <value>
        /// The <see cref="IConfiguration"/> that represents a set of key/value application configuration properties.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Specifies the contract for a <see cref="IServiceCollection"/> of service descriptors.</param>
        public void ConfigureServices(IServiceCollection services)
        {

            var secretKey = Configuration.GetSection("botFileSecret")?.Value;
            var botFilePath = Configuration.GetSection("botFilePath")?.Value;
            if (!File.Exists(botFilePath))
            {
                throw new FileNotFoundException($"The .bot configuration file was not found. botFilePath: {botFilePath}");
            }

            // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
            BotConfiguration botConfig = null;
            try
            {
                botConfig = BotConfiguration.Load(botFilePath, secretKey);
            }
            catch
            {
                var msg = @"Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.
    - You can find the botFilePath and botFileSecret in the Azure App Service application settings.
    - If you are running this bot locally, consider adding a appsettings.json file with botFilePath and botFileSecret.
    - See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration.
    ";
                throw new InvalidOperationException(msg);
            }

            services.AddSingleton(sp => botConfig ?? throw new InvalidOperationException($"The .bot configuration file could not be loaded. botFilePath: {botFilePath}"));

            // Add BotServices singleton.
            // Create the connected services from .bot file.
            services.AddSingleton(sp => new BotServices(botConfig));

            // Create PimBotServiceProvider
            var featureService = new FeatureService(new FeatureRepository());
            var keywordService = new KeywordService(new KeywordRepository());
            var categoryService = new CategoryService(new CategoryRepository());
            var itemService = new ItemService(new ItemRepository(), featureService, keywordService, categoryService, new PictureRepository());
            var serviceProvider = new PimBotServiceProvider(itemService, keywordService, featureService, categoryService);

            services.AddSingleton<IPimbotServiceProvider>(serviceProvider);

            services.AddScoped<IKeywordService, KeywordService>();

            // Retrieve current endpoint.
            var environment = _isProduction ? "production" : "development";
            var service = botConfig.Services.FirstOrDefault(s => s.Type == "endpoint" && s.Name == environment);
            if (service == null && _isProduction)
            {
                // Attempt to load development environment
                service = botConfig.Services.Where(s => s.Type == "endpoint" && s.Name == "development").FirstOrDefault();
            }

            if (!(service is EndpointService endpointService))
            {
                throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{environment}'.");
            }

            // For testing and develop
            IStorage dataStore = new MemoryStorage();
            
            // For publishing
//            IStorage dataStore = new CosmosDbStorage(new CosmosDbStorageOptions()
//            {
//                AuthKey = Constants.CosmosDBKey,
//                CollectionId = Constants.CosmosDBCollectionName,
//                CosmosDBEndpoint = new Uri(Constants.CosmosServiceEndpoint),
//                DatabaseId = Constants.CosmosDBDatabaseName,
//            });

            // Create and add conversation state.
            var conversationState = new ConversationState(dataStore);
            services.AddSingleton(conversationState);

            services.AddScoped<IKeywordService, KeywordService>();

            var userState = new UserState(dataStore);
            services.AddSingleton(userState);

            var blobStorage = new AzureBlobTranscriptStore(
                Constants.AzureBlogStorageConnectionString,
                Constants.BlobTranscriptStorageContainerName);

            services.AddBot<PimBot.PimBot>(options =>
            {
                options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);
                options.ChannelProvider = new ConfigurationChannelProvider(Configuration);

                ILogger logger = _loggerFactory.CreateLogger<PimBot.PimBot>();

                // For logging every single conversations
//                options.Middleware.Add(new TranscriptLoggerMiddleware(blobStorage));
                options.Middleware.Add(new ShowTypingMiddleware());
                var middleware = options.Middleware;

                // Catches any errors that occur during a conversation turn and logs them to currently
                // configured ILogger.
                options.OnTurnError = async (context, exception) =>
                {
                    logger.LogError($"Exception caught : {exception}");
                    if (exception is System.Net.Http.HttpRequestException || exception is System.Net.Sockets.SocketException)
                    {
                        await context.SendActivityAsync(Messages.ServerIssue);
                    }
                    else
                    {
                        await context.SendActivityAsync(Messages.SomethingWrong);
                    }
                };
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application Builder.</param>
        /// <param name="env">Hosting Environment.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to create logger object for tracing.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }
    }
}
