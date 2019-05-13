// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date:
// ===

using NUnit.Framework;
using PimBot.Services;

namespace Tests
{
    /// <summary>
    /// Test suite for Common util.
    /// </summary>
    public class CommonUtilTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ContainsIgnoreCase_WithoutCase_Test()
        {
            // Given
            var sentance = "wood chair";
            var key = "chair";

            // When

            // Then
            Assert.IsTrue(CommonUtil.ContainsIgnoreCase(sentance, key));
        }

        [Test]
        public void ContainsIgnoreCase_WithCase_Test()
        {
            // Given
            var sentance = "WOOD CHAIR";
            var key = "chair";

            // When

            // Then
            Assert.IsTrue(CommonUtil.ContainsIgnoreCase(sentance, key));
        }

        [Test]
        public void ContainsIgnoreCase_NoMatch_Test()
        {
            // Given
            var sentance = "WOOD CHAIR";
            var key = "table";

            // When

            // Then
            Assert.IsFalse(CommonUtil.ContainsIgnoreCase(sentance, key));
        }

        [Test]
        public void ComputeLevenshteinDistance_Same_Test()
        {
            // Given
            var sentance = "chair";
            var key = "chair";

            // When
            var distance = CommonUtil.ComputeLevenshteinDistance(sentance, key);

            // Then
            Assert.AreEqual(0, distance);
        }

        [Test]
        public void ComputeLevenshteinDistance_Diff1_Test()
        {
            // Given
            var sentance = "chais";
            var key = "chair";

            // When
            var distance = CommonUtil.ComputeLevenshteinDistance(sentance, key);

            // Then
            Assert.AreEqual(1, distance);
        }

        [Test]
        public void ComputeLevenshteinDistance_WholeDiff_Test()
        {
            // Given
            var sentance = "abcde";
            var key = "chair";

            // When
            var distance = CommonUtil.ComputeLevenshteinDistance(sentance, key);

            // Then
            Assert.AreEqual(5, distance);
        }
    }
}