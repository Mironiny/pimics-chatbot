namespace PimBot
{
    public static class Constants
    {
        public const string ODataServiceEndpoint = @"http://pimicschatbot.westeurope.cloudapp.azure.com:7048/NAV/ODataV4";
        public const string Company = "Company";
        public const string CompanyName = "CRONUS International Ltd.";

        public const string ItemsServiceEndpointName = "ItemsPIM";
        public const string FeaturesServiceEndpointName = "FeaturesPIM";
        public const string KeywordsServiceEndpointName = "KeywordsPIM";
        public const string ItemGroupServiceEndpointName = "ItemGroupsPIM";
        public const string ProductGroupServiceEndpointName = "ProductGroupsPIM";
        public const string ItemGroupLinks = "ItemGroupLinks";


        // Should be somewhere else
        public const string SecureUserName = "Allium";
        public const string SecureUserPassword = "#Allium12345$";

        public const string AzureBlogStorageConnectionString =
            "DefaultEndpointsProtocol=https;AccountName=azurestorepimbotdp;AccountKey=2MTXz0yWH1Ykk7rG0//qYoYQu3amf8ZKfNNU9M64evJ9j36sPTfw/Ka98hmXuBA4vTke+o3cc9dXAyClSOfFZQ==;EndpointSuffix=core.windows.net";

        public const string BlobTranscriptStorageContainerName = "AllConversations";

        // CosmosDB
        public const string CosmosServiceEndpoint = "https://pimbotdp.documents.azure.com:443/";
        public const string CosmosDBKey = "DCjdl5L3K3TWXV1ByEq995qFKd4auYO0mDJliVC6IgcKxW8XMBXnx74mKzRq70p1zR0y6pOpamqkIl9KOynm0w==";
        public const string CosmosDBDatabaseName = "bot-cosmos-sql-db";
        public const string CosmosDBCollectionName = "bot-storage";

        // QNA Maker
        public const string host = "https://qnapimbotdp.azurewebsites.net/qnamaker";
        public const string endpoint_key = "a363fcfd-b747-4b52-86fc-4bf6d999a6e3";
        public const string route = "/knowledgebases/42bc7b3f-f585-4df2-99ab-e7ea5ce7ebf3/generateAnswer";

        // Find item engine
        public const string UnitPriceType = "UnitPriceType";
        // Descripe after how many question will bot ask if you want to show all question or continue searching
        public const int QuestionLimit = 5;
    }
}
