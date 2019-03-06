namespace PimBot
{
    public static class Constants
    {
        public const string ODataServiceEndpoint = "http://pimicschatbot.westeurope.cloudapp.azure.com:7048/NAV/ODataV4";
        public const string ItemsServiceEndpointName = "ItemsPIM";
        public const string FeaturesServiceEndpointName = "FeaturesPIM";
        public const string KeywordsServiceEndpointName = "KeywordsPIM";
        public const string ItemGroupServiceEndpointName = "ItemGroupsPIM";

        // Should be somewhere else
        public const string SecureUserName = "Allium";
        public const string SecureUserPassword = "#Allium12345$";

        public const string AzureBlogStorageConnectionString =
            "DefaultEndpointsProtocol=https;AccountName=azurestorepimbotdp;AccountKey=2MTXz0yWH1Ykk7rG0//qYoYQu3amf8ZKfNNU9M64evJ9j36sPTfw/Ka98hmXuBA4vTke+o3cc9dXAyClSOfFZQ==;EndpointSuffix=core.windows.net";

        public const string BlobTranscriptStorageContainerName = "AllConversations";
    }
}
