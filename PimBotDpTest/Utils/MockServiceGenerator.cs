using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Moq;
using PimBot.Repositories;
using PimBot.State;

namespace PimBotDpTest.Utils
{
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
            var pictures = FakeDataGenerator.CreateDummyItems();
            var x = new List<string>();

            mock.Setup(r => r.GetPictureUrlByPictureDocumentId("id"));
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
