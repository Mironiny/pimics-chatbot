using System.Linq;
using NUnit.Framework;
using PimBot.Repositories;
using PimBot.Service;
using PimBot.Services.Impl;
using PimBotDpTest.Utils;
using System.Threading.Tasks;

namespace Tests
{
    public class CategoryServiceTest
    {
        private ICategoryService categoryService;

        [SetUp]
        public void Setup()
        {
            ICategoryRepository mock = MockServiceGenerator.CreateCategoryRepository();
            categoryService = new CategoryService(mock);
        }

        [Test]
        public async Task GetAllItemGroupAsync_Count_Test()
        {
            // Given
            var itemGroups = FakeDataGenerator.CreateDummyItemGroup();

            // When
            var returnedGroups = await categoryService.GetAllItemGroupAsync();

            // Then
            Assert.NotNull(returnedGroups);
            Assert.AreEqual(3, returnedGroups.ToList().Count);
        }

        [Test]
        public async Task GetItemGroupIdsByDescription_NoMatch_Test()
        {
            // Given
            var itemGroups = FakeDataGenerator.CreateDummyItemGroup();

            // When
            var returnedGroups = await categoryService.GetItemGroupIdsByDescription("FOO");

            // Then
            Assert.NotNull(returnedGroups);
            Assert.AreEqual(0, returnedGroups.ToList().Count);
        }

        [Test]
        public async Task GetItemGroupIdsByDescription_OneMatchWithCase_Test()
        {
            // Given
            var itemGroups = FakeDataGenerator.CreateDummyItemGroup();

            // When
            var returnedGroups = await categoryService.GetItemGroupIdsByDescription("BENCHES");

            // Then
            Assert.NotNull(returnedGroups);
            Assert.AreEqual(2, returnedGroups.ToList().Count);
        }

        [Test]
        public async Task GetItemGroupIdsByDescription_TwoMatchNoCase_Test()
        {
            // Given
            var itemGroups = FakeDataGenerator.CreateDummyItemGroup();

            // When
            var returnedGroups = await categoryService.GetItemGroupIdsByDescription("table");

            // Then
            Assert.NotNull(returnedGroups);
            Assert.AreEqual(1, returnedGroups.ToList().Count);
        }
    }
}