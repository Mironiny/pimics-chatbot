using System.Linq;
using Moq;
using NUnit.Framework;
using PimBot.Repositories;
using PimBot.Service;
using PimBotDpTest.Utils;
using System.Threading.Tasks;
using PimBot.Service.Impl;
using PimBot.State;
using System.Collections.Generic;
using PimBot.Dialogs.FindItem;
using PimBot.Services.Impl;

namespace Tests
{
    public class ItemServiceTest
    {
        private IItemService itemService;

        [SetUp]
        public void Setup()
        {
            var itemRepositoryMock = MockServiceGenerator.CreateItemRepositoryMock();
            var featureRepositoryMock = MockServiceGenerator.CreateFeaturesRepositoryMock();
            var keywordsRepositoryMock = MockServiceGenerator.CreateKeywordRepository();
            var categoryRepositoryMock = MockServiceGenerator.CreateCategoryRepository();

            var featureService = new FeatureService(featureRepositoryMock);
            var keywordService = new KeywordService(keywordsRepositoryMock);
            var categoryService = new CategoryService(categoryRepositoryMock);

            itemService = new ItemService(itemRepositoryMock, featureService, keywordService, categoryService);

//            var keywords = FakeDataGenerator.CreateDummyKeywords();
//            var features = FakeDataGenerator.CreateDummyFeatures();
//
//            var newKeywords = new List<PimKeyword>();
//            newKeywords.Add(keywords.ToList().First());
//
//            var disctionary = new Dictionary<string, List<PimKeyword>>()
//            {
//                { "1000", newKeywords}
//            };
//
//            keywordServiceMock.Setup(s => s.GetAllKeywordsGroupByItemCodeAsync())
//                .ReturnsAsync(disctionary);
//
//            itemService = new ItemService(itemRepositoryMock, featureServiceMock.Object, keywordServiceMock.Object, categoryServiceMock.Object);
        }

        //
        // GetAllFeaturesByItemAsync method tests. Find no item.
        //
        [Test]
        public async Task FindItemByNo_NoItem_Test()
        {
            // When
            var returnedItem = await itemService.FindItemByNo("0");

            // Then
            Assert.Null(returnedItem);
        }

        //
        // GetAllFeaturesByItemAsync method tests. Find one item.
        //
        [Test]
        public async Task FindItemByNo_CountOfItems_Test()
        {
            // When
            var returnedItem = await itemService.FindItemByNo("1000");

            // Then
            Assert.NotNull(returnedItem);
            Assert.AreEqual("Chair Camilla", returnedItem.Description);
        }

        //
        // GetAllItemsCategory method tests
        //
        [Test]
        public void GetAllItemsCategory_CountOfCategories_Test()
        {
            // When
            var returnedGroup = itemService.GetAllItemsCategory(FakeDataGenerator.CreateDummyItems());

            // Then
            Assert.NotNull(returnedGroup);
            Assert.AreEqual(1, returnedGroup.Count());
        }

        //
        // GetAllItemsByMatch method tests
        //
        [Test]
        public async Task GetAllItemsByMatchAsync_NoMatch_Test()
        {
            // When
            var returnedItems = await itemService.GetAllItemsByMatchAsync("foo");

            // Then
            Assert.NotNull(returnedItems);
            Assert.AreEqual(0, returnedItems.Count());
        }

        //
        // GetAllItemsByMatch method tests
        //
        [Test]
        public async Task GetAllItemsByMatchAsync_OneMatch_Test()
        {
            // When
            var returnedItems = await itemService.GetAllItemsByMatchAsync("Table");

            // Then
            Assert.NotNull(returnedItems);
            Assert.AreEqual(1, returnedItems.Count());
        }

        //
        // GetAllItemsByMatch method tests
        //
        [Test]
        public async Task GetAllItemsByMatchAsync_CategoryMatch_Test()
        {
            // When
            var returnedItems = await itemService.GetAllItemsByMatchAsync("chair");

            // Then
            Assert.NotNull(returnedItems);
            Assert.AreEqual(3, returnedItems.Count());
        }

        //
        // GetAllFeaturesToAsk method tests
        //
        [Test]
        public async Task GetAllFeaturesToAsk_NoItem_Test()
        {
            // Given
            var pimItems = await itemService.GetAllItemsByMatchAsync("fuu");

            // When
            var featuresToAsk = await itemService.GetAllFeaturesToAsk(pimItems);

            // Then
            Assert.NotNull(featuresToAsk);
            Assert.AreEqual(0, featuresToAsk.Count());
        }

        //
        // GetAllFeaturesToAsk method tests
        //
        [Test]
        public async Task GetAllFeaturesToAsk_Count_Test()
        {
            // Given
            var pimItems = await itemService.GetAllItemsByMatchAsync("Chair");

            // When
            var featuresToAsk = await itemService.GetAllFeaturesToAsk(pimItems);

            // Then
            Assert.NotNull(featuresToAsk);
            Assert.AreEqual(4, featuresToAsk.Count());
        }

        //
        // GetAllFeaturesToAsk method tests. Order is defined by order and information gain.
        //
        [Test]
        public async Task GetAllFeaturesToAsk_Order_Test()
        {
            // Given
            var pimItems = await itemService.GetAllItemsByMatchAsync("Chair");

            // When
            var featuresToAsk = await itemService.GetAllFeaturesToAsk(pimItems);

            // Then
            Assert.NotNull(featuresToAsk);
            Assert.AreEqual("Unit price", featuresToAsk[0].Description);
            Assert.AreEqual("Weight", featuresToAsk[1].Description);
            Assert.AreEqual("Color", featuresToAsk[2].Description);
            Assert.AreEqual("Material", featuresToAsk[3].Description);
        }

        //
        // GetAllFeaturesToAsk method tests. Type should be automatic infered based on values.
        //
        [Test]
        public async Task GetAllFeaturesToAsk_Type_Test()
        {
            // Given
            var pimItems = await itemService.GetAllItemsByMatchAsync("Chair");

            // When
            var featuresToAsk = await itemService.GetAllFeaturesToAsk(pimItems);

            // Then
            Assert.NotNull(featuresToAsk);
            Assert.AreEqual(FeatureType.Numeric, featuresToAsk[0].Type);
            Assert.AreEqual(FeatureType.Numeric, featuresToAsk[1].Type);
            Assert.AreEqual(FeatureType.Alphanumeric, featuresToAsk[2].Type);
            Assert.AreEqual(FeatureType.Alphanumeric, featuresToAsk[3].Type);
        }

        //
        // GetAllFeaturesToAsk method tests. Check if there is right amount of values to ask. 
        //
        [Test]
        public async Task GetAllFeaturesToAsk_ValuesCount_Test()
        {
            // Given
            var pimItems = await itemService.GetAllItemsByMatchAsync("Chair");

            // When
            var featuresToAsk = await itemService.GetAllFeaturesToAsk(pimItems);

            // Then
            Assert.NotNull(featuresToAsk);
            Assert.AreEqual(3, featuresToAsk[0].ValuesList.Count);
            Assert.AreEqual(3, featuresToAsk[1].ValuesList.Count);
            Assert.AreEqual(3, featuresToAsk[2].ValuesList.Count);
            Assert.AreEqual(2, featuresToAsk[3].ValuesList.Count);
        }

        //
        // GetAllFeaturesToAsk method tests. Check if there is right amount of values to ask. 
        //
        [Test]
        public async Task GetAllFeaturesToAsk_Values_Test()
        {
            // Given
            var pimItems = await itemService.GetAllItemsByMatchAsync("Chair");

            // When
            var featuresToAsk = await itemService.GetAllFeaturesToAsk(pimItems);

            // Then
            Assert.NotNull(featuresToAsk);
            Assert.Contains("1000.00", featuresToAsk[0].ValuesList.ToList());
            Assert.Contains("500.00", featuresToAsk[0].ValuesList.ToList());
            Assert.Contains("700.00", featuresToAsk[0].ValuesList.ToList());

            Assert.Contains("10", featuresToAsk[1].ValuesList.ToList());
            Assert.Contains("20", featuresToAsk[1].ValuesList.ToList());
            Assert.Contains("40", featuresToAsk[1].ValuesList.ToList());

            Assert.Contains("Brown", featuresToAsk[2].ValuesList.ToList());
            Assert.Contains("Red", featuresToAsk[2].ValuesList.ToList());
            Assert.Contains("Blue", featuresToAsk[2].ValuesList.ToList());

            Assert.Contains("Steel", featuresToAsk[3].ValuesList.ToList());
            Assert.Contains("Wood", featuresToAsk[3].ValuesList.ToList());
        }

        //
        // GetAllFeaturesToAsk method test. Calculate median of the number values.
        //
        [Test]
        public async Task GetAllFeaturesToAsk_Median_Test()
        {
            // Given
            var pimItems = await itemService.GetAllItemsByMatchAsync("Chair");

            // When
            var featuresToAsk = await itemService.GetAllFeaturesToAsk(pimItems);

            // Then
            Assert.NotNull(featuresToAsk);
            Assert.AreEqual(700, featuresToAsk[0].GetMedianValue());
            Assert.AreEqual(20, featuresToAsk[1].GetMedianValue());
        }

        //
        // GetAllFeaturesToAsk method tests. One item without feautures.
        //
        [Test]
        public async Task GetAllFeaturesToAsk_OneItem_Test()
        {
            // Given
            var pimItems = await itemService.GetAllItemsByMatchAsync("table");

            // When
            var featuresToAsk = await itemService.GetAllFeaturesToAsk(pimItems);

            // Then
            Assert.NotNull(featuresToAsk);
            Assert.AreEqual(0, featuresToAsk.Count);
        }

        //
        // GetAllFeaturesToAsk method tests. One item without feautures.
        //
        [Test]
        public async Task FindSimilarItemsByDescription_OneItem_Test()
        {
            // When
            var similaItem = await itemService.FindSimilarItemsByDescription("tble");

            // Then
            Assert.NotNull(similaItem);
            Assert.AreEqual("table", similaItem);
        }

        //
        // GetAllFeaturesToAsk method tests. One item without feautures.
        //
        [Test]
        public async Task FindSimilarItemsByDescription_EmptyString_Test()
        {
            // When
            var similaItem = await itemService.FindSimilarItemsByDescription("");

            // Then
            Assert.NotNull(similaItem);
            Assert.AreEqual("stul", similaItem);
        }

        //
        // GetAllFeaturesToAsk method tests. One item without feautures.
        //
        [Test]
        public async Task FindSimilarItemsByDescription_LongString_Test()
        {
            // When
            var similaItem = await itemService.FindSimilarItemsByDescription("Roma bench");

            // Then
            Assert.NotNull(similaItem);
            Assert.AreEqual("Roma benches", similaItem);
        }

        //
        // GetAllFeaturesToAsk method tests. One item without feautures.
        //
        [Test]
        public async Task FilterItemsByFeature_Test_Test()
        {
            Assert.Pass();
        }

    }
}