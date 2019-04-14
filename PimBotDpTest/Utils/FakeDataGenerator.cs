using System;
using System.Collections.Generic;
using System.Text;
using PimBot.State;

namespace PimBotDpTest.Utils
{
    public class FakeDataGenerator
    {
        public static string CreatePictureUrl = "vut.fit.cz";

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
            item1.Description = "Chair Camilla";
            item1.Unit_Price = 500;
            item1.Picture_Document_ID = "PicID1";
            items.Add(item1);

            var item2 = new PimItem();
            item2.No = "1001";
            item2.Description = "Chair Roma";
            item2.Unit_Price = 700;
            item2.Picture_Document_ID = "PicID1";
            items.Add(item2);

            var item3 = new PimItem();
            item3.No = "1002";
            item3.Description = "Chair Milada";
            item3.Unit_Price = 1000;
            item3.Picture_Document_ID = "PicID1";
            items.Add(item3);

            var item4 = new PimItem();
            item4.No = "1003";
            item4.Description = "Table";
            item4.Unit_Price = 2000;
            items.Add(item4);

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
            keyword1.Code = "1003";
            keyword1.Keyword = "stul";
            keywords.Add(keyword1);

            var keyword2 = new PimKeyword();
            keyword2.Code = "1000";
            keyword2.Keyword = "chair";
            keywords.Add(keyword2);

            var keyword3 = new PimKeyword();
            keyword3.Code = "1001";
            keyword3.Keyword = "chair";
            keywords.Add(keyword3);

            var keyword4 = new PimKeyword();
            keyword4.Code = "1001";
            keyword4.Keyword = "wood";
            keywords.Add(keyword4);

            return keywords;
        }

        public static IEnumerable<PimFeature> CreateDummyFeatures()
        {
            var features = new List<PimFeature>();
            var feature1 = new PimFeature();
            feature1.Code = "1000";
            feature1.Number = "INT100001";
            feature1.Description = "Weight";
            feature1.Value = "10";
            feature1.Order = 1;
            features.Add(feature1);

            var feature10 = new PimFeature();
            feature10.Code = "1000";
            feature10.Number = "INT100009";
            feature10.Description = "Legs";
            feature10.Value = "4";
            feature10.Order = 1;
            features.Add(feature10);

            var feature4 = new PimFeature();
            feature4.Code = "1000";
            feature4.Number = "INT100002";
            feature4.Description = "Material";
            feature4.Value = "Wood";
            feature4.Order = 0;
            features.Add(feature4);

            var feature7 = new PimFeature();
            feature7.Code = "1000";
            feature7.Number = "INT100003";
            feature7.Description = "Color";
            feature7.Value = "Brown";
            feature7.Order = 0;
            features.Add(feature7);

            var feature2 = new PimFeature();
            feature2.Code = "1001";
            feature2.Number = "INT100001";
            feature2.Description = "Weight";
            feature2.Value = "20";
            feature2.Order = 1;
            features.Add(feature2);

            var feature11 = new PimFeature();
            feature11.Code = "1001";
            feature11.Number = "INT100009";
            feature11.Description = "Legs";
            feature11.Value = "4";
            feature11.Order = 1;
            features.Add(feature11);

            var feature3 = new PimFeature();
            feature3.Code = "1001";
            feature3.Number = "INT100002";
            feature3.Description = "Material";
            feature3.Value = "Wood";
            feature3.Order = 0;
            features.Add(feature3);

            var feature8 = new PimFeature();
            feature8.Code = "1001";
            feature8.Number = "INT100003";
            feature8.Description = "Color";
            feature8.Value = "Red";
            feature8.Order = 0;
            features.Add(feature8);

            var feature5 = new PimFeature();
            feature5.Code = "1002";
            feature5.Number = "INT100001";
            feature5.Description = "Weight";
            feature5.Value = "40";
            feature5.Order = 1;
            features.Add(feature5);

            var feature6 = new PimFeature();
            feature6.Code = "1002";
            feature6.Number = "INT100002";
            feature6.Description = "Material";
            feature6.Value = "Steel";
            feature6.Order = 0;
            features.Add(feature6);

            var feature12 = new PimFeature();
            feature12.Code = "1002";
            feature12.Number = "INT100009";
            feature12.Description = "Legs";
            feature12.Value = "4";
            feature12.Order = 1;
            features.Add(feature12);

            var feature9 = new PimFeature();
            feature9.Code = "1002";
            feature9.Number = "INT100003";
            feature9.Description = "Color";
            feature9.Value = "Blue";
            feature9.Order = 0;
            features.Add(feature9);

            return features;
        }
    }
}
