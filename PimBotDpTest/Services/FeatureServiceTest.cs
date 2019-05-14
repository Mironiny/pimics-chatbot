// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date:
// ===

using System.Linq;
using NUnit.Framework;
using PimBot.Repositories;
using PimBot.Services;
using PimBot.Services.Impl;
using PimBotDpTest.Utils;
using System.Threading.Tasks;

namespace Tests
{
    /// <summary>
    /// Test suite for Feature service.
    /// </summary>
    public class FeatureServiceTest
    {
        private IFeatureService featureService;

        [SetUp]
        public void Setup()
        {
            IFeaturesRepository mock = MockServiceGenerator.CreateFeaturesRepositoryMock();
            featureService = new FeatureService(mock);
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
            Assert.AreEqual(3, returnedFeatures.Count);
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
            Assert.AreEqual(4, returnedFeatures["1000"].Count);
            Assert.AreEqual(4, returnedFeatures["1001"].Count);
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
            Assert.AreEqual("10", returnedFeatures["1000"].First().Value);

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
            var allFeatures = await featureService.GetAllFeatures();
            var returnedFeatures = await featureService.GetFeaturesByNoAsync("0", allFeatures);

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
            var allFeatures = await featureService.GetAllFeatures();
            var returnedFeatures = await featureService.GetFeaturesByNoAsync("1000", allFeatures);

            // Then
            Assert.NotNull(returnedFeatures);
            Assert.AreEqual(4, returnedFeatures.Count);
            Assert.AreEqual("Weight", returnedFeatures.First().Description);
            Assert.AreEqual("10", returnedFeatures.First().Value);
        }

        [Test]
        public async Task GetFeaturesByNoAsync_TwoNo_Test()
        {
            // Given
            var features = FakeDataGenerator.CreateDummyFeatures();

            // When
            var allFeatures = await featureService.GetAllFeatures();
            var returnedFeatures = await featureService.GetFeaturesByNoAsync("1001", allFeatures);

            // Then
            Assert.NotNull(returnedFeatures);
            Assert.AreEqual(4, returnedFeatures.Count);
        }

    }
}