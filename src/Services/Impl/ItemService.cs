using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PimBot.Services.Impl;
using PimBotDp.Services;
using PimBotDp.State;

namespace PimBot.Service.Impl
{
    public class ItemService : IItemService
    {
        private readonly IKeywordService _keywordService = new KeywordService();
        private readonly IFeatureService _featuresService = new FeatureService();
        private readonly ICategoryService _categoryService = new CategoryService();

        public async Task<PimItem> FindItemByNo(string no)
        {
            // Refactor to look only for one item
            var itemList = new List<string>();

            var client = ODataClientSingleton.Get();

            var items = await client
                .For(Constants.ItemsServiceEndpointName)
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

        public async Task<IEnumerable<PimItem>> GetAllItemsAsync(string entity)
        {
            var itemList = new List<string>();

            var client = ODataClientSingleton.Get();

            var items = await client
                .For(Constants.ItemsServiceEndpointName)
                .FindEntriesAsync();

            var keywordsByItemSet = await _keywordService.GetAllKeywordsByItemAsync();
            var featuresByItem = await _featuresService.GetAllFeaturesByItemAsync();

            var pimItems = MapItems(items);
            var filteredByCategory = await FilterByCategory(pimItems, entity);
            var filteredByKeywords = FilterByKeywordsMatch(pimItems, entity, keywordsByItemSet);
            var filteredByDescription = FilterByDescription(pimItems, entity);

            return filteredByKeywords;
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
