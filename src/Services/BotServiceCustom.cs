﻿// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 13.2.2019
// ===

using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Configuration;

namespace PimBot.Services
{
    public class BotServiceCustom
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BotServices"/> class.
        /// </summary>
        /// <param name="luisServices">A dictionary of named <see cref="LuisRecognizer"/> instances for usage within the bot.</param>
        public BotServiceCustom(BotConfiguration botConfiguration)
        {
            foreach (var service in botConfiguration.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.Luis:
                        {
                            var luis = (LuisService)service;
                            if (luis == null)
                            {
                                throw new InvalidOperationException("The LUIS service is not configured correctly in your '.bot' file.");
                            }

                            var endpoint = (luis.Region?.StartsWith("https://") ?? false) ? luis.Region : luis.GetEndpoint();
                            var app = new LuisApplication(luis.AppId, luis.AuthoringKey, endpoint);
                            var recognizer = new LuisRecognizer(app, includeApiResults: true);
                            this.LuisServices.Add(luis.Name, recognizer);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Gets the set of LUIS Services used.
        /// Given there can be multiple <see cref="LuisRecognizer"/> services used in a single bot,
        /// LuisServices is represented as a dictionary.  This is also modeled in the
        /// ".bot" file since the elements are named.
        /// </summary>
        /// <remarks>The LUIS services collection should not be modified while the bot is running.</remarks>
        /// <value>
        /// A <see cref="LuisRecognizer"/> client instance created based on configuration in the .bot file.
        /// </value>
        public Dictionary<string, LuisRecognizer> LuisServices { get; } = new Dictionary<string, LuisRecognizer>();
    }
}
