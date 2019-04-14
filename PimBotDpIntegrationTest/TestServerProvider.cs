using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.BotBuilderSamples;

namespace PimBotDpIntegrationTest
{
    public class TestServerProvider
    {
        public HttpClient Client { get; set; }

        public TestServerProvider()
        {
            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());

            Client = server.CreateClient();
        }
    }
}
