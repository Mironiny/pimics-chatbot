using Moq;
using NUnit.Framework;
using PimBot.Repositories;
using PimBot.Repositories.Impl;
using PimBot.Services;
using PimBot.State;
using System.Collections.Generic;
using System.Linq;
using PimBotDpTest.Utils;
using PimBot.Services.Impl;
using System.Threading.Tasks;

namespace Tests
{
    public class KeywordServiceTest
    {
        private KeywordService keywordService;

        [SetUp]
        public void Setup()
        {
            var mock = new Mock<IKeywordRepository>();
            var keywords = FakeDataGenerator.CreateDummyKeywords();

            mock.Setup(foo => foo.GetAll()).ReturnsAsync(keywords);
            keywordService = new KeywordService(mock.Object);

        }

        [Test]
        public async Task GetAllKeywords_CountOfKeywords_Test()
        {
            // Given
            var keywords = FakeDataGenerator.CreateDummyKeywords().ToList();

            // When
            var returnedKeywords = await keywordService.GetAllKeywordsAsync();

            // Then
            Assert.NotNull(returnedKeywords);
            Assert.AreEqual(keywords.Count, returnedKeywords.ToList().Count);
            Assert.AreNotEqual(1, returnedKeywords.ToList().Count);
        }

        [Test]
        public async Task GetAllKeywords_KeywordProperty_Test()
        {
            // Given
            var keywords = FakeDataGenerator.CreateDummyKeywords().ToList();

            // When
            var returnedKeywords = await keywordService.GetAllKeywordsAsync();
            var returnedKeyword = returnedKeywords.ToList()[0];

            // Then
            Assert.NotNull(returnedKeywords);
            Assert.AreEqual(keywords[0].Code, returnedKeyword.Code);
            Assert.AreNotEqual("FUU", returnedKeyword.Code);
        }

        [Test]
        public async Task GetAllKeywordsByItemAsync_KeywordProperty_Test()
        {
            // Given
            var keywords = FakeDataGenerator.CreateDummyKeywords().ToList();

            // When
            var returnedKeywords = await keywordService.GetAllKeywordsByItemAsync();
            Assert.NotNull(returnedKeywords);

        }

    }
}