using System;
using System.Threading.Tasks;
using Xunit;

namespace PimBotDpIntegrationTest
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var client = new TestServerProvider().Client;

            var response = await client.GetAsync("/api/messages");

            response.EnsureSuccessStatusCode();
        }
    }
}
