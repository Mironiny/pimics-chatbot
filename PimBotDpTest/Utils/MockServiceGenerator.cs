// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date:
// ===

using Moq;
using PimBot.Repositories;

namespace PimBotDpTest.Utils
{
    /// <summary>
    /// Class for generating mocks.
    /// </summary>
    public class MockServiceGenerator
    {
        public static IItemRepository CreateItemRepositoryMock()
        {
            var mock = new Mock<IItemRepository>();
            var items = FakeDataGenerator.CreateDummyItems();

            mock.Setup(r => r.GetAll()).ReturnsAsync(items);
            return mock.Object;
        }

        public static IPictureRepository CreatePictureRepositoryMock()
        {
            var mock = new Mock<IPictureRepository>();
            var pictureUrl = FakeDataGenerator.CreatePictureUrl;

            mock.Setup(r => r.GetPictureUrlByPictureDocumentId("PicID1")).ReturnsAsync(pictureUrl);
            return mock.Object;
        }

        public static IFeaturesRepository CreateFeaturesRepositoryMock()
        {
            var mock = new Mock<IFeaturesRepository>();
            var features = FakeDataGenerator.CreateDummyFeatures();

            mock.Setup(r => r.GetAll()).ReturnsAsync(features);
            return mock.Object;
        }

        public static ICategoryRepository CreateCategoryRepository()
        {
            var mock = new Mock<ICategoryRepository>();
            var itemGroups = FakeDataGenerator.CreateDummyItemGroup();

            mock.Setup(r => r.GetAllItemGroup()).ReturnsAsync(itemGroups);
            return mock.Object;
        }


        public static IKeywordRepository CreateKeywordRepository()
        {
            var mock = new Mock<IKeywordRepository>();
            var keywords = FakeDataGenerator.CreateDummyKeywords();

            mock.Setup(r => r.GetAll()).ReturnsAsync(keywords);
            return mock.Object;
        }


    }
}
