using System;
using System.Collections.Generic;
using System.Text;
using PimBot.State;

namespace PimBotDpTest.Utils
{
    public class FakeDataGenerator
    {
        public static IEnumerable<PimItem> CreateDummyItems()
        {
            var items = new List<PimItem>();
            var item1 = new PimItem();
            item1.PimItemGroups = new List<PimItemGroup>();
            var itemCategory1 = new PimItemGroup();
            itemCategory1.Code = "1000";
            itemCategory1.Description = "Roma benches";
            item1.PimItemGroups.Add(itemCategory1);

            item1.No = "1000";
            item1.Description = "Bicycle";
            items.Add(item1);

            var item2 = new PimItem();
            item2.No = "1001";
            item2.Description = "Benches Roma";
            items.Add(item2);

            var item3 = new PimItem();
            item3.No = "1002";
            item3.Description = "table";
            items.Add(item3);

            return items;
        }

        public static IEnumerable<PimItemGroup> CreateDummyItemGroup()
        {
            var itemGroups = new List<PimItemGroup>();
            var itemCategory1 = new PimItemGroup();
            itemCategory1.Code = "1000";
            itemCategory1.Description = "Roma benches";
            itemGroups.Add(itemCategory1);

            var itemCategory2 = new PimItemGroup();
            itemCategory2.Code = "1001";
            itemCategory2.Description = "Camila benches";
            itemGroups.Add(itemCategory2);

            var itemCategory3 = new PimItemGroup();
            itemCategory3.Code = "1002";
            itemCategory3.Description = "table";
            itemGroups.Add(itemCategory3);

            return itemGroups;
        }

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

            var keyword3 = new PimKeyword();
            keyword2.Code = "1001";
            keyword2.Keyword = "computer";
            keywords.Add(keyword2);

            return keywords;
        }

        public static IEnumerable<PimFeature> CreateDummyFeatures()
        {
            var features = new List<PimFeature>();
            var feature1 = new PimFeature();
            feature1.Code = "1000";
            feature1.Number = "INT100002";
            feature1.Description = "Weight";
            feature1.Value = "14";
            features.Add(feature1);

            var feature2 = new PimFeature();
            feature2.Number = "INT100003";
            feature2.Description = "Color";
            feature2.Code = "1001";
            feature2.Value = "blue";
            features.Add(feature2);

            var feature3 = new PimFeature();
            feature3.Code = "1001";
            feature3.Number = "INT100004";
            feature3.Description = "Height";
            feature3.Value = "11";
            features.Add(feature3);

            return features;
        }
    }
}
