using Moq;
using NUnit.Framework;
using PimBot.Repositories;
using System.Linq;
using PimBotDpTest.Utils;
using PimBot.Services.Impl;
using System.Threading.Tasks;
using PimBot.Services;

namespace Tests
{
    public class KeywordServiceTest
    {
        private IKeywordService keywordService;

        [SetUp]
        public void Setup()
        {
            IKeywordRepository mock = MockServiceGenerator.CreateKeywordRepository();
            keywordService = new KeywordService(mock);
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
        public async Task GetAllKeywordsGroupByItemCodeAsync_KeywordsKeys_Test()
        {
            // Given
            var keywords = FakeDataGenerator.CreateDummyKeywords().ToList();

            // When
            var returnedKeywords = await keywordService.GetAllKeywordsGroupByItemCodeAsync();

            // Then
            Assert.NotNull(returnedKeywords);
            Assert.AreEqual(3, returnedKeywords.Keys.Count);
        }

        [Test]
        public async Task GetAllKeywordsGroupByItemCodeAsync_CountOfValues_Test()
        {
            // Given
            var keywords = FakeDataGenerator.CreateDummyKeywords().ToList();

            // When
            var returnedKeywords = await keywordService.GetAllKeywordsGroupByItemCodeAsync();

            // Then
            Assert.NotNull(returnedKeywords);
            Assert.AreEqual(1, returnedKeywords["1000"].Count);
            Assert.AreEqual(2, returnedKeywords["1001"].Count);
        }

        [Test]
        public async Task GetAllKeywordsGroupByItemCodeAsync_Values_Test()
        {
            // Given
            var keywords = FakeDataGenerator.CreateDummyKeywords().ToList();

            // When
            var returnedKeywords = await keywordService.GetAllKeywordsGroupByItemCodeAsync();

            // Then
            Assert.NotNull(returnedKeywords);
            Assert.AreEqual("chair", returnedKeywords["1000"].ToList().First().Keyword);
        }
    }
}