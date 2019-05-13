// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date:
// ===

using System;
using System.Net;
using System.Web;
using Simple.OData.Client;

namespace PimBot.Repositories
{
    /// <summary>
    /// Class which instanticiate OData client as singleton.
    /// </summary>
    public class ODataClientSingleton
    {
        private static ODataClient client = null;

        private ODataClientSingleton()
        {
        }

        public static ODataClient Get()
        {
            if (client == null)
            {
                ODataClientSettings settings = new ODataClientSettings();
                settings.BaseUri = new Uri(Constants.ODataServiceEndpoint);
                settings.IgnoreUnmappedProperties = true;
                settings.Credentials = new NetworkCredential(Constants.SecureUserName, Constants.SecureUserPassword);
                client = new ODataClient(settings);
            }

            return client;
        }

        private static void BeforeRequest(System.Net.Http.HttpRequestMessage requestMessage)
        {
            string url = HttpUtility.UrlDecode(requestMessage.RequestUri.AbsoluteUri);
            requestMessage.RequestUri = new Uri(url);
        }
    }
}
