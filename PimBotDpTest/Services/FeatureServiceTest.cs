using System.Linq;
using Moq;
using NUnit.Framework;
using PimBot.Repositories;
using PimBot.Service;
using PimBot.Services.Impl;
using PimBotDpTest.Utils;
using System.Threading.Tasks;

namespace Tests
{
    public class FeatureServiceTest
    {
        private IFeatureService featureService;

        [SetUp]
        public void Setup()
        {
            var mock = new Mock<IFeaturesRepository>();
            var features = FakeDataGenerator.CreateDummyFeatures();

            mock.Setup(r => r.GetAll()).ReturnsAsync(features);
            featureService = new FeatureService(mock.Object);
        }

        //
        // GetAllFeaturesByItemAsync method tests
        //
        [Test]
        public async Task GetAllFeaturesByItemAsync_CountOfFeatures_Test()
        {
            // Given
            var features = FakeDataGenerator.CreateDummyFeatures();

            // When
            var returnedFeatures = await featureService.GetAllFeaturesByItemAsync();

            // Then
            Assert.NotNull(returnedFeatures);
            Assert.AreEqual(2, returnedFeatures.Count);
        }

        [Test]
        public async Task GetAllFeaturesByItemAsync_FeaturesCode_Test()
        {
            // Given
            var features = FakeDataGenerator.CreateDummyFeatures();

            // When
            var returnedFeatures = await featureService.GetAllFeaturesByItemAsync();

            // Then
            Assert.NotNull(returnedFeatures);
            Assert.AreEqual(1, returnedFeatures["1000"].Count);
            Assert.AreEqual(2, returnedFeatures["1001"].Count);
        }

        [Test]
        public async Task GetAllFeaturesByItemAsync_Property_Test()
        {
            // Given
            var features = FakeDataGenerator.CreateDummyFeatures();

            // When
            var returnedFeatures = await featureService.GetAllFeaturesByItemAsync();

            // Then
            Assert.NotNull(returnedFeatures);
            Assert.AreEqual("Weight", returnedFeatures["1000"].First().Description);
            Assert.AreEqual("14", returnedFeatures["1000"].First().Value);

        }

        //
        // GetFeaturesByNoAsync method tests
        //
        [Test]
        public async Task GetFeaturesByNoAsync_NotExistionNo_Test()
        {
            // Given
            var features = FakeDataGenerator.CreateDummyFeatures();

            // When
            var returnedFeatures = await featureService.GetFeaturesByNoAsync("0");

            // Then
            Assert.NotNull(returnedFeatures);
            Assert.AreEqual(0, returnedFeatures.Count);
        }

        [Test]
        public async Task GetFeaturesByNoAsync_OneNo_Test()
        {
            // Given
            var features = FakeDataGenerator.CreateDummyFeatures();

            // When
            var returnedFeatures = await featureService.GetFeaturesByNoAsync("1000");

            // Then
            Assert.NotNull(returnedFeatures);
            Assert.AreEqual(1, returnedFeatures.Count);
            Assert.AreEqual("Weight", returnedFeatures.First().Description);
            Assert.AreEqual("14", returnedFeatures.First().Value);
        }

        [Test]
        public async Task GetFeaturesByNoAsync_TwoNo_Test()
        {
            // Given
            var features = FakeDataGenerator.CreateDummyFeatures();

            // When
            var returnedFeatures = await featureService.GetFeaturesByNoAsync("1001");

            // Then
            Assert.NotNull(returnedFeatures);
            Assert.AreEqual(2, returnedFeatures.Count);
        }

    }
}