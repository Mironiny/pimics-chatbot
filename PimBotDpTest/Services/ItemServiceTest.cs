using System.Linq;
using Moq;
using NUnit.Framework;
using PimBot.Repositories;
using PimBot.Service;
using PimBot.Services.Impl;
using PimBotDpTest.Utils;
using System.Threading.Tasks;
using PimBot.Service.Impl;
using PimBot.State;
using System.Collections.Generic;

namespace Tests
{
    public class ItemServiceTest
    {
        private IItemService itemService;

        [SetUp]
        public void Setup()
        {
            var itemRepositoryMock = new Mock<IItemRepository>();
            var featureServiceMock = new Mock<IFeatureService>();
            var keywordServiceMock = new Mock<IKeywordService>();
            var categoryServiceMock = new Mock<ICategoryService>();

            var items = FakeDataGenerator.CreateDummyItems();
            var keywords = FakeDataGenerator.CreateDummyKeywords();
            var newKeywords = new List<PimKeyword>();
            newKeywords.Add(keywords.ToList().First());

            itemRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(items);
            var disctionary = new Dictionary<string, List<PimKeyword>>()
            {
                { "1000", newKeywords}
            };

            keywordServiceMock.Setup(s => s.GetAllKeywordsGroupByItemCodeAsync())
                .ReturnsAsync(disctionary);

            itemService = new ItemService(itemRepositoryMock.Object, featureServiceMock.Object, keywordServiceMock.Object, categoryServiceMock.Object);
        }

        //
        // GetAllFeaturesByItemAsync method tests
        //
        [Test]
        public async Task FindItemByNo_CountOfItems_Test()
        {
            // When
            var returnedItem = await itemService.FindItemByNo("1000");

            // Then
            Assert.NotNull(returnedItem);
            Assert.AreEqual("Bicycle", returnedItem.Description);
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
            var returnedItems = await itemService.GetAllItemsByMatchAsync("Bicycle");

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
            var returnedItems = await itemService.GetAllItemsByMatchAsync("benches");

            // Then
            Assert.NotNull(returnedItems);
            Assert.AreEqual(1, returnedItems.Count());
        }

        //
        // GetAllItemsByMatch method tests
        //
        [Test]
        public async Task GetAllItemsByMatchAsync_KeywordsMatch_Test()
        {
            // When
            var returnedItems = await itemService.GetAllItemsByMatchAsync("cellphone");

            // Then
            Assert.NotNull(returnedItems);
            Assert.AreEqual(1, returnedItems.Count());
        }

    }
}