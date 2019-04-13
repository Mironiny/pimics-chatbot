using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using PimBot.Dialogs.FindItem;
using PimBot.Repositories;
using PimBot.Services;
using PimBot.State;

namespace PimBot.Service.Impl
{
    public class ItemService : IItemService
    {

        private readonly IItemRepository _itemRepository;
        private readonly IPictureRepository _pictureRepository;

        private readonly IKeywordService _keywordService;
        private readonly IFeatureService _featuresService;
        private readonly ICategoryService _categoryService;

        public ItemService(
            IItemRepository itemRepository,
            IFeatureService featureService,
            IKeywordService keywordService,
            ICategoryService categoryService,
            IPictureRepository pictureRepository)
        {
            _itemRepository = itemRepository;
            _featuresService = featureService;
            _keywordService = keywordService;
            _categoryService = categoryService;
            _pictureRepository = pictureRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public async Task<PimItem> FindItemByNo(string no)
        {
            // Refactor to look only for one items
            var pimItems = await _itemRepository.GetAll();

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
        /// Method returns every items which match item description or category description or has keywords with same value.
        /// </summary>
        /// <param name="description">Value to look for in item description, category and keywords</param>
        /// <returns></returns>
        public async Task<IEnumerable<PimItem>> GetAllItemsByMatchAsync(string description)
        {
            var pimItems = await _itemRepository.GetAll();
            var keywordsByItemSet = await _keywordService.GetAllKeywordsGroupByItemCodeAsync();

            var filteredByCategory = await FilterByCategory(pimItems, description);
            var filteredByKeywords = FilterByKeywordsMatch(pimItems, description, keywordsByItemSet);
            var filteredByDescription = FilterByItemDescription(pimItems, description);

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
        /// Method choose from inputs items features to ask. It's sorts features by order and computed information gain.
        /// </summary>
        /// <param name="items"></param>
        /// <returns>Ordered features to ask</returns>
        public async Task<List<FeatureToAsk>> GetAllFeaturesToAsk(IEnumerable<PimItem> items)
        {
            var features = await _featuresService.GetAllFeaturesByItemAsync();
            var itemsNos = items.ToList().Select(i => i.No).ToList();
            var featuresByInputItems = features.Where(f => itemsNos.Contains(f.Key)).ToList();

            var featureToAsks = new List<FeatureToAsk>();
            foreach (var feature in featuresByInputItems)
            {
                List<string> added = new List<string>();
                foreach (var ftr in feature.Value)
                {
                    // Check if already in feature to ask
                    var index = featureToAsks.FindIndex(f => f.Number == ftr.Number);
                    if (!(index >= 0))
                    {
                        // Add to feature to ask
                        var featureToAsk = new FeatureToAsk(ftr.Number, ftr.Description, ftr.Order, ftr.Unit_Shorthand_Description);
                        featureToAsk.ValuesList = new HashSet<string>();

                        // Added new feature value
                        if (ftr.Value != string.Empty)
                        {
                            added.Add(ftr.Description);
                            featureToAsk.ValuesList.Add(ftr.Value);
                        }

                        featureToAsks.Add(featureToAsk);
                    }
                    else
                    {
                        // Check if same item doesnt content same attribut
                        if (!added.Contains(ftr.Description))
                        {
                            if (ftr.Value != string.Empty)
                            {
                                added.Add(ftr.Description);
                                featureToAsks[index].ValuesList.Add(ftr.Value);
                            }
                        }
                    }
                }
            }

            // Added to features to ask the unit price which is by default not in features
            var unitPriceFeature = new FeatureToAsk(Constants.UnitPriceType, Messages.UnitPrice, 1, string.Empty);
            unitPriceFeature.ValuesList = new HashSet<string>();
            foreach (var item in items)
            {
                unitPriceFeature.ValuesList.Add(item.Unit_Price.ToString("F", CultureInfo.CreateSpecificCulture("en-US")));
            }

            featureToAsks.Add(unitPriceFeature);

            // Features without value or with one value are meaningless - they don't help
            var filteredFeatureToAsk = featureToAsks.Where(i => i.ValuesList.Count > 1).ToList();

            // Set type of feature - numeric, text, etc
            filteredFeatureToAsk.ForEach(i => i.SetAndCheckType());

            // For every feature compute information gain
            filteredFeatureToAsk.ForEach(i => i.ComputeInformationGain(items.ToList()));

            // Order features by Order and information gain
            var orderedFeaturesToAsk = filteredFeatureToAsk
                .OrderByDescending(i => i.Order)
                .ThenByDescending(i => i.InformationGain)
                .ToList();

            return orderedFeaturesToAsk;
        }

        /// <summary>
        /// Filter items by feature.
        /// </summary>
        /// <param name="items">Input items to filter.</param>
        /// <param name="featureToAsk">Selected feature which is was asked.</param>
        /// <param name="value">Value which user put. If type of feature is numeric, then this value will be median</param>
        /// <param name="index">Valid only in numerics type. If user select under median, the index is 0 otherwise 1</param>
        /// <returns></returns>
        public async Task<IEnumerable<PimItem>> FilterItemsByFeature(
            IEnumerable<PimItem> items,
            FeatureToAsk featureToAsk,
            string value,
            int index = -1)
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

                    // Special case for Unit Price
                    if (featureToAsk.Number == Constants.UnitPriceType)
                    {
                        if (index == 0)
                        {
                            return items.Where(i => Convert.ToDouble(i.Unit_Price) <= Convert.ToDouble(value)).ToList();
                        }
                        else if (index == 1)
                        {
                            return items.Where(i => Convert.ToDouble(i.Unit_Price) > Convert.ToDouble(value)).ToList();
                        }
                    }

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
        /// Get all categories which match input items.
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

        public async Task<string> FindSimilarItemsByDescription(string description)
        {
            var pimItems = await _itemRepository.GetAll();

            var keywords = await _keywordService.GetAllKeywordsAsync();
            var groups = await _categoryService.GetAllItemGroupAsync();
            var groupsProduct = await _categoryService.GetAllProductGroupAsync();

            var unitedItems = pimItems.Select(i => i.Description)
                .Union(keywords.Select(k => k.Keyword))
                .Union(groups.Select(g => g.Description))
                .Union(groupsProduct.Select(g => g.Description))
                .ToList();

            var orderedItems = unitedItems.OrderBy(i => CommonUtil.ComputeLevenshteinDistance(i, description)).ToList();
            if (orderedItems != null)
            {
                return orderedItems[0];
            }

            return null;
        }

        public async Task<string> GetImageUrl(PimItem item)
        {
            return await _pictureRepository.GetPictureUrlByPictureDocumentId(item.Picture_Document_ID);
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
                if (CommonUtil.ContainsIgnoreCase(keyword.Keyword, key) || CommonUtil.ContainsIgnoreCase(key, keyword.Keyword))
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<PimItem> FilterByItemDescription(IEnumerable<PimItem> items, string key)
        {
            return items
                .Where(o => CommonUtil.ContainsIgnoreCase(o.Description, key) || CommonUtil.ContainsIgnoreCase(key, o.Description))
                .ToList();
        }

    }
}
