// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 1.4.2019
// ===

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PimBot.Dto;
using PimBot.Repositories;

namespace PimBot.Services.Impl
{
    /// <summary>
    /// Service for handling categories (implementation).
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Get item group ids by description.
        /// </summary>
        /// <param name="description">Description.</param>
        /// <returns>Items groups.</returns>
        public async Task<IEnumerable<string>> GetItemGroupIdsByDescription(string description)
        {
            var pimItemGroups = await _categoryRepository.GetAllItemGroup();

            return pimItemGroups
                .Where(o => CommonUtil.ContainsIgnoreCase(o.Description, description))
                .Select(o => o.Code)
                .ToList();
        }

        /// <summary>
        /// Get all items by group.
        /// </summary>
        /// <returns>PimItemGroup.</returns>
        public async Task<IEnumerable<PimItemGroup>> GetAllItemGroupAsync() => await _categoryRepository.GetAllItemGroup();

        /// <summary>
        /// Get all product group.
        /// </summary>
        /// <returns>Product group.</returns>
        public async Task<IEnumerable<PimProductGroup>> GetAllProductGroupAsync() => await _categoryRepository.GetAllProductGroup();

        /// <summary>
        /// Get item groups by No.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <returns>Task.</returns>
        public async Task GetItemGroupsByNo(PimItem item)
        {
            throw new System.NotImplementedException();

            //            var allGroups = await _categoryRepository.GetAllItemGroup();
            //
            //            var pimItemGroups = new List<PimItemGroup>();
            //            var client = ODataClientSingleton.Get();
            //
            //            var groups = await client
            //                .For(Constants.Company).Key(Constants.CompanyName)
            //                .NavigateTo(Constants.ItemGroupLinks)
            //                .Filter($"Number%20eq%20%27{item.No}%27")
            //                .FindEntriesAsync();
            //
            //            foreach (var group in groups)
            //            {
            //                var pimItemGroup = new PimItemGroup();
            //                pimItemGroup.Code = (string)group["Code"];
            //                var description = allGroups.Where(i => i.Code == (string) group["Code"]).Select(i => i.Description).First();
            //                pimItemGroup.Description = description;
            //                pimItemGroups.Add(pimItemGroup);
            //            }
            //
            //         item.PimItemGroups = pimItemGroups;
        }
    }
}
