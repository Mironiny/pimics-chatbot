using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PimBot.Service
{
    public class ODataClientSingleton
    {
        private ODataClientSingleton() { }

        private static ODataClient client = null;

        public static ODataClient Get()
        {
            if (client == null)
            {
                ODataClientSettings settings = new ODataClientSettings();
                settings.BaseUri = new Uri(Constants.ODataServiceEndpoint);
                settings.Credentials = new NetworkCredential(Constants.SecureUserName, Constants.SecureUserPassword);
                client = new ODataClient(settings);
            }      
            return client;
        }
    }
}
