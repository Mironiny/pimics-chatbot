using System;
using System.Collections.Generic;
using System.Text;
using PimBot.State;

namespace PimBotDpTest.Utils
{
    public class FakeDataGenerator
    {
        public static IEnumerable<PimKeyword> CreateDummyKeywords()
        {
            var keywords = new List<PimKeyword>();
            var keyword1 = new PimKeyword();
            keyword1.Code = "1000";
            keyword1.Keyword = "cellphone";
            keywords.Add(keyword1);

            var keyword2 = new PimKeyword();
            keyword2.Code = "1001";
            keyword2.Keyword = "pc";
            keywords.Add(keyword2);

            return keywords;
        }
    }
}
