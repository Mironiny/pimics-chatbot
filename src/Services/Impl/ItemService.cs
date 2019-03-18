using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PimBot.Dialogs.FindItem;
using PimBot.Services;
using PimBot.Services.Impl;
using PimBot.State;

namespace PimBot.Service.Impl
{
    public class ItemService : IItemService
    {
        private readonly IKeywordService _keywordService = new KeywordService();
        private readonly IFeatureService _featuresService = new FeatureService();
        private readonly ICategoryService _categoryService = new CategoryService();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public async Task<PimItem> FindItemByNo(string no)
        {
            // Refactor to look only for one items
            var itemList = new List<string>();

            var client = ODataClientSingleton.Get();

            var items = await client
                .For(Constants.Company).Key(Constants.CompanyName)
                .NavigateTo(Constants.ItemsServiceEndpointName)
                .FindEntriesAsync();

            var pimItems = MapItems(items);

            var pimItem = pimItems.Where(o => o.No.Equals(no, StringComparison.OrdinalIgnoreCase)).ToList();
            if (pimItem == null || !pimItem.Any())
            {
                return null;
            }
            else
            {
                return pimItem.First();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PimItem>> GetAllItemsAsync(string entity)
        {
            var itemList = new List<string>();

            var client = ODataClientSingleton.Get();

            var items = await client
                .For(Constants.Company).Key(Constants.CompanyName)
                .NavigateTo(Constants.ItemsServiceEndpointName)
                .FindEntriesAsync();

            var keywordsByItemSet = await _keywordService.GetAllKeywordsByItemAsync();
            var featuresByItem = await _featuresService.GetAllFeaturesByItemAsync();

            var pimItems = MapItems(items);

            var filteredByCategory = await FilterByCategory(pimItems, entity);
            var filteredByKeywords = FilterByKeywordsMatch(pimItems, entity, keywordsByItemSet);
            var filteredByDescription = FilterByDescription(pimItems, entity);

            var unitedItems = filteredByDescription
                .Union(filteredByKeywords)
                .Union(filteredByDescription);

            // Initialization of list
            unitedItems.ToList().ForEach(i => _categoryService.GetItemGroupsByNo(i));
            foreach (var item in unitedItems)
            {
                item.PimFeatures = await _featuresService.GetFeaturesByNoAsync(item.No);
            }

            return unitedItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public async Task<List<FeatureToAsk>> GetAllAttributes(IEnumerable<PimItem> items)
        {
            var features = await _featuresService.GetAllFeaturesByItemAsync();
            var itemsNos = items.ToList().Select(i => i.No).ToList();
            var pimFeature = features.Where(f => itemsNos.Contains(f.Key)).ToList();

            //            var pimFeature = features
            //                .Where(i => items.ToList()
            //                                .FindIndex(f => f.No == i.Key) >= 0).ToList();

            var fff = new List<FeatureToAsk>();
            foreach (var feature in pimFeature)
            {
                List<string> added = new List<string>();
                foreach (var ftr in feature.Value)
                {
                    // Check if already in array
                    var index = fff.FindIndex(f => f.Number == ftr.Number);
                    if (!(index >= 0))
                    {
                        var featureToAsk = new FeatureToAsk(ftr.Number, ftr.Description, ftr.Order, ftr.Unit_Shorthand_Description);
                        featureToAsk.ValuesList = new HashSet<string>();
                        if (ftr.Value != string.Empty)
                        {
                            added.Add(ftr.Description);
                            featureToAsk.ValuesList.Add(ftr.Value);
                        }

                        fff.Add(featureToAsk);
                    }
                    else
                    {
                        // Check if same item doesnt content same attribut
                        if (!added.Contains(ftr.Description))
                        {
                            if (ftr.Value != string.Empty)
                            {
                                added.Add(ftr.Description);
                                fff[index].ValuesList.Add(ftr.Value);
                            }
                        }
                    }
                }
            }

            // Added to question UNIT PRICE
            var featureToA = new FeatureToAsk("UNITPRICE", "Unit price", 1, string.Empty);
            featureToA.ValuesList = new HashSet<string>();
            foreach (var item in items)
            {
                featureToA.ValuesList.Add(item.Unit_Price.ToString("F", CultureInfo.CreateSpecificCulture("en-US")));
            }

            fff.Add(featureToA);

            // Atributes without value or with one value are meaningless
            var ff = fff.Where(i => i.ValuesList.Count > 1).ToList();

            ff.ForEach(i => i.SetAndCheckType());
            ff.ForEach(i => i.ComputeInformationGain(items.ToList()));
            var fuu = ff.OrderByDescending(i => i.Order).ThenByDescending(i => i.InformationGain).ToList();
            return fuu;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="featureToAsk"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PimItem>> FilterItemsByFeature(IEnumerable<PimItem> items, FeatureToAsk featureToAsk,
            string value, int index = -1)
        {
            var pimItemResult = new List<PimItem>();
            switch (featureToAsk.Type)
            {
                case FeatureType.Alphanumeric:
                    foreach (var item in items)
                    {
                        var features = await _featuresService.GetFeaturesByNoAsync(item.No);
                        var filteredItem = features.Where(i => i.Number == featureToAsk.Number).ToList();
                        if (filteredItem.Count() == 0)
                        {
                            pimItemResult.Add(item);
                        }
                        else
                        {
                            if (filteredItem[0].Value == value)
                            {
                                pimItemResult.Add(item);
                            }
                        }
                    }

                    return pimItemResult;

                case FeatureType.Numeric:
                    foreach (var item in items)
                    {
                        var features = await _featuresService.GetFeaturesByNoAsync(item.No);
                        var filteredItem = features.Where(i => i.Number == featureToAsk.Number).ToList();
                        if (filteredItem.Count() == 0)
                        {
                            pimItemResult.Add(item);
                        }
                        else
                        {
                            if (index == 0)
                            {
                                if (Convert.ToDouble(filteredItem[0].Value) <= Convert.ToDouble(value))
                                {
                                    pimItemResult.Add(item);
                                }
                            }

                            if (index == 1)
                            {
                                if (Convert.ToDouble(filteredItem[0].Value) > Convert.ToDouble(value))
                                {
                                    pimItemResult.Add(item);
                                }
                            }
                        }
                    }

                    return pimItemResult;
                default:
                    return items;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public IEnumerable<PimItemGroup> GetAllItemsCategory(IEnumerable<PimItem> items)
        {
            var categories = new HashSet<PimItemGroup>();
            foreach (var item in items)
            {
                var groups = item.PimItemGroups;
                if (groups != null)
                {
                    categories.UnionWith(groups);
                }
            }

            return categories;
        }


        private async Task<IEnumerable<PimItem>> FilterByCategory(IEnumerable<PimItem> pimItems, string entity)
        {
            var categoryIds = await _categoryService.GetItemGroupIdsByDescription(entity);
            return pimItems.Where(o => categoryIds.Contains(o.Standardartikelgruppe));
        }

        private IEnumerable<PimItem> FilterByKeywordsMatch(
            IEnumerable<PimItem> items,
            string key,
            Dictionary<string, List<PimKeyword>> keywordsByItem)
        {
            return items.Where(item => IsItemContainsKeyword(item, key, keywordsByItem)).ToList();
        }

        private bool IsItemContainsKeyword(PimItem item, string key, Dictionary<string, List<PimKeyword>> keywordsByItem)
        {
            if (!keywordsByItem.ContainsKey(item.No))
            {
                return false;
            }

            var keywords = keywordsByItem[item.No];

            foreach (var keyword in keywords)
            {
                if (CommonUtil.ContainsIgnoreCase(keyword.Keyword, key))
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<PimItem> FilterByDescription(IEnumerable<PimItem> items, string key)
        {
            return items
                .Where(o => CommonUtil.ContainsIgnoreCase(o.Description, key))
                .ToList();
        }

        private IEnumerable<PimItem> MapItems(IEnumerable<IDictionary<string, object>> items)
        {
            List<PimItem> pimItems = new List<PimItem>();
            foreach (var item in items)
            {
                var pimItem = MapPimItem(item);
                pimItems.Add(pimItem);
            }

            return pimItems;
        }

        private PimItem MapPimItem(IDictionary<string, object> item)
        {
            var pimItem = new PimItem();

            pimItem.No = (string)item["No"];
            pimItem.Description = (string)item["Description"];
            pimItem.Systemstatus = (string)item["Systemstatus"];
            pimItem.Assembly_BOM = (bool)item["Assembly_BOM"];
            pimItem.Base_Unit_of_Measure = (string)item["Base_Unit_of_Measure"];
            pimItem.Shelf_No = (string)item["Shelf_No"];
            pimItem.Costing_Method = (string)item["Costing_Method"];
            pimItem.Standard_Cost = (decimal)item["Standard_Cost"];
            pimItem.Unit_Cost = (decimal)item["Unit_Cost"];
            pimItem.Last_Direct_Cost = (decimal)item["Last_Direct_Cost"];
            pimItem.Price_Profit_Calculation = (string)item["Price_Profit_Calculation"];
            pimItem.Profit_Percent = (decimal)item["Profit_Percent"];
            pimItem.Unit_Price = (decimal)item["Unit_Price"];
            pimItem.Inventory_Posting_Group = (string)item["Inventory_Posting_Group"];
            pimItem.Gen_Prod_Posting_Group = (string)item["Gen_Prod_Posting_Group"];
            pimItem.VAT_Prod_Posting_Group = (string)item["VAT_Prod_Posting_Group"];
            pimItem.Vendor_No = (string)item["Vendor_No"];
            pimItem.Vendor_Item_No = (string)item["Vendor_Item_No"];
            pimItem.Tariff_No = (string)item["Tariff_No"];
            pimItem.Search_Description = (string)item["Search_Description"];
            pimItem.Durability = (string)item["Durability"];
            pimItem.Picture_Document_ID = (string)item["Picture_Document_ID"];
            pimItem.Standardartikelgruppe = (string)item["Standardartikelgruppe"];
            pimItem.Base_Class_No = (string)item["Base_Class_No"];
            pimItem.Item_Category_Code = (string)item["Item_Category_Code"];
            pimItem.Product_Group_Code = (string)item["Product_Group_Code"];
            pimItem.ETag = (string)item["ETag"];
            return pimItem;
        }
    }
}
