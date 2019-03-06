using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PimBot.Service;
using PimBot.State;

namespace PimBot.Services.Impl
{
    public class CategoryService : ICategoryService
    {
        public async Task<IEnumerable<string>> GetItemGroupIdsByDescription(string description)
        {
            var pimItemGroups = await GetAllItemGroupAsync();

            return pimItemGroups
                .Where(o => CommonUtil.ContainsIgnoreCase(o.Description, description))
                .Select(o => o.Code)
                .ToList();
        }

        public async Task<IEnumerable<PimItemGroup>> GetAllItemGroupAsync()
        {
            var client = ODataClientSingleton.Get();

            var groups = await client
                .For(Constants.ItemGroupServiceEndpointName)
                .FindEntriesAsync();

            var pimItemGroups = MapPimItemGroup(groups);

            return pimItemGroups;
        }

        private IEnumerable<PimItemGroup> MapPimItemGroup(IEnumerable<IDictionary<string, object>> itemGroups)
        {
            List<PimItemGroup> pimItemGroups = new List<PimItemGroup>();
            foreach (var itemGroup in itemGroups)
            {
                var pimItemGroup = MapPimItemGroup(itemGroup);
                pimItemGroups.Add(pimItemGroup);
            }

            return pimItemGroups;
        }

        private PimItemGroup MapPimItemGroup(IDictionary<string, object> keyword)
        {
            var pimKeyword = new PimItemGroup
            {
                Code = (string)keyword["Code"],
                Description = (string)keyword["Description"],
                Description_2 = (string)keyword["Description_2"],
                System_Status = (string)keyword["System_Status"],
                LocalDescription = (string)keyword["LocalDescription"],
                LocalDescription2 = (string)keyword["LocalDescription2"],
                Base_Unit = (string)keyword["Base_Unit"],
                Standard_Product_Group = (string)keyword["Standard_Product_Group"],
                Picture_Document_ID = (string)keyword["Picture_Document_ID"],
                Template_Code = (string)keyword["Template_Code"],
                Item_Template_Code = (string)keyword["Item_Template_Code"],
                Publication_Group = (string)keyword["Publication_Group"],
                Short_Text = (string)keyword["Short_Text"],
                Created_By = (string)keyword["Created_By"],
                Updated_By = (string)keyword["Updated_By"],
                Additional_Information_1 = (string)keyword["Additional_Information_1"],
                Additional_Information_2 = (string)keyword["Additional_Information_2"],
                Additional_Information_3 = (string)keyword["Additional_Information_3"],
                Additional_Information_4 = (string)keyword["Additional_Information_4"],
                Additional_Information_5 = (string)keyword["Additional_Information_5"],
                ETag = (string)keyword["ETag"]
            };
            return pimKeyword;
        }
    }
}
