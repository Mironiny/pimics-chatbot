// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 5.2.2019
// ===

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PimBot.Dialogs;
using PimBot.Dto;
using PimBot.Repositories;

namespace PimBot.Services.Impl
{
    /// <summary>
    /// Service for handling items (implementation).
    /// </summary>
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
        /// Find item by NO number.
        /// </summary>
        /// <param name="no">No.</param>
        /// <returns>Item.</returns>
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
        /// <returns>All items by match.</returns>
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

//            unitedItems.ToList().ForEach(i => _categoryService.GetItemGroupsByNo(i));
            var allFeatures = await _featuresService.GetAllFeatures();
            foreach (var item in unitedItems)
            {
                item.PimFeatures = await _featuresService.GetFeaturesByNoAsync(item.No, allFeatures);
            }

            return unitedItems;
        }

        /// <summary>
        /// Method choose from inputs items features to ask. It's sorts features by order and computed information gain.
        /// </summary>
        /// <param name="items">List of items.</param>
        /// <returns>Ordered features to ask.</returns>
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
        /// <param name="value">Value which user put. If type of feature is numeric, then this value will be median.</param>
        /// <param name="index">Valid only in numerics type. If user select under median, the index is 0 otherwise 1.</param>
        /// <returns>Filtered items.</returns>
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
                        var allFeatures = await _featuresService.GetAllFeatures();
                        var features = await _featuresService.GetFeaturesByNoAsync(item.No, allFeatures);
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
                        var allFeatures = await _featuresService.GetAllFeatures();
                        var features = await _featuresService.GetFeaturesByNoAsync(item.No, allFeatures);
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
        /// <param name="items">List of items.</param>
        /// <returns>Items group.</returns>
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

        /// <summary>
        /// Find similar items in by description using Levenshtein distance.
        /// </summary>
        /// <param name="description">Description.</param>
        /// <returns>Description of similar item.</returns>
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

        /// <summary>
        /// Get image Url, try create base64 URI. If the size is too big, resize until its gonna be OK.
        /// </summary>
        /// <param name="item">Items.</param>
        /// <returns>Image url.</returns>
        public async Task<string> GetImageUrl(PimItem item)
        {
            string base64string = await _pictureRepository.GetPictureUrlByPictureDocumentId(item.Picture_Document_ID);
            if (base64string == null)
            {
                return null;
            }

            try
            {
                while (!Uri.TryCreate(base64string, UriKind.RelativeOrAbsolute, out Uri uri))
                {
                    var image = Base64ToImage(base64string);
                    var comprimImage = ResizeImage(image, image.Width / 4, image.Height / 4);

                    MemoryStream ms = new MemoryStream();
                    comprimImage.Save(ms, ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    base64string = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }

                return base64string;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Filter items by category.
        /// </summary>
        /// <param name="pimItems">Items to filter.</param>
        /// <param name="groupName">Group name.</param>
        /// <returns>Filtered items.</returns>
        private async Task<IEnumerable<PimItem>> FilterByCategory(IEnumerable<PimItem> pimItems, string groupName)
        {
            var categoryIds = await _categoryService.GetItemGroupIdsByDescription(groupName);
            return pimItems.Where(o => categoryIds.Contains(o.Standardartikelgruppe));
        }

        /// <summary>
        /// Filter items by keywords.
        /// </summary>
        /// <param name="items">Items to filter.</param>
        /// <param name="key">Key to filter.</param>
        /// <param name="keywordsByItem">Keywords.</param>
        /// <returns>Filtered items.</returns>
        private IEnumerable<PimItem> FilterByKeywordsMatch(
            IEnumerable<PimItem> items,
            string key,
            Dictionary<string, List<PimKeyword>> keywordsByItem)
        {
            return items.Where(item => IsItemContainsKeyword(item, key, keywordsByItem)).ToList();
        }

        /// <summary>
        /// Is item cointains keywords.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="key">Key.</param>
        /// <param name="keywordsByItem">KeywordsByItem.</param>
        /// <returns>If items cointains keyword.</returns>
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

        /// <summary>
        /// Filter items by description.
        /// </summary>
        /// <param name="items">Items to filter.</param>
        /// <param name="key">Key to look for.</param>
        /// <returns>Filtered items.</returns>
        private IEnumerable<PimItem> FilterByItemDescription(IEnumerable<PimItem> items, string key)
        {
            return items
                .Where(o => CommonUtil.ContainsIgnoreCase(o.Description, key) || CommonUtil.ContainsIgnoreCase(key, o.Description))
                .ToList();
        }

        /// <summary>
        /// Convert Base64 image to Image object.
        /// </summary>
        /// <param name="base64String">Base64 image.</param>
        /// <returns>Image.</returns>
        private Image Base64ToImage(string base64String)
        {
            var cleanerBase64 = base64String.Substring(22);

            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(cleanerBase64);

            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height. Source - STACK OVERFLOW.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
