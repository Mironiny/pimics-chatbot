using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PimBot.Repositories;
using PimBot.Service;
using PimBot.State;

namespace PimBot.Services.Impl
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetItemGroupIdsByDescription(string description)
        {
            var pimItemGroups = await _categoryRepository.GetAllItemGroup();

            return pimItemGroups
                .Where(o => CommonUtil.ContainsIgnoreCase(o.Description, description))
                .Select(o => o.Code)
                .ToList();
        }

        public async Task<IEnumerable<PimItemGroup>> GetAllItemGroupAsync()
        {
            return await _categoryRepository.GetAllItemGroup();
        }

        public async Task<IEnumerable<PimProductGroup>> GetAllProductGroupAsync()
        {
            return await _categoryRepository.GetAllProductGroup();
        }

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
