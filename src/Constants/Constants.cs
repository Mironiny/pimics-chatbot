using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PimBot
{
    public static class Constants
    {
        public const string ODataServiceEndpoint = "http://pimicschatbot.westeurope.cloudapp.azure.com:7048/NAV/ODataV4";
        public const string ItemsServiceEndpointName = "ItemsPIM";
        public const string FeaturesServiceEndpointName = "FeaturesPIM";
        public const string KeywordsServiceEndpointName = "KeywordsPIM";

        // Should be somewhere else
        public const string SecureUserName = "Allium";
        public const string SecureUserPassword = "#Allium12345$";

    }
}
