using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;
using PimBot.Service;
using PimBot.State;

namespace PimBot.Services.Impl
{
    public class CategoryService : ICategoryService
    {

        public async Task GetItemGroupsByNo(PimItem item)
        {
            var allGroups = await GetAllItemGroupAsync();

            var pimItemGroups = new List<PimItemGroup>();
            var client = ODataClientSingleton.Get();

            var groups = await client
                .For(Constants.Company).Key(Constants.CompanyName)
                .NavigateTo(Constants.ItemGroupLinks)
                .Filter($"Number%20eq%20%27{item.No}%27")
                .FindEntriesAsync();

            foreach (var group in groups)
            {
                var pimItemGroup = new PimItemGroup();
                pimItemGroup.Code = (string)group["Code"];
                var description = allGroups.Where(i => i.Code == (string) group["Code"]).Select(i => i.Description).First();
                pimItemGroup.Description = description;
//
//                if (description != null && description.Count() > 0)
//                {
//                    var tmp = description.ToList()[0];
//                    pimItemGroup.Description = tmp;
//                }
                //                var xx = await client
                //                    .For(Constants.Company).Key(Constants.CompanyName)
                //                    .NavigateTo(Constants.ItemGroupServiceEndpointName)
                //                    .Filter($"Code%20eq%20%27{(string)group["Code"]}%27")
                //                    .FindEntriesAsync();
                //
                //                if (xx != null && xx.Count() > 0)
                //                {
                //                    var list = xx.ToList()[0];
                //                    pimItemGroup.Description = (string)list["Description"];
                //                }
                //                item.PimItemGroups.Add(pimItemGroup);
                pimItemGroups.Add(pimItemGroup);
            }

         item.PimItemGroups = pimItemGroups;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetItemGroupIdsByDescription(string description)
        {
            var pimItemGroups = await GetAllItemGroupAsync();

            return pimItemGroups
                .Where(o => CommonUtil.ContainsIgnoreCase(o.Description, description))
                .Select(o => o.Code)
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PimItemGroup>> GetAllItemGroupAsync()
        {
            var client = ODataClientSingleton.Get();

            var groups = await client
                .For(Constants.Company).Key(Constants.CompanyName)
                .NavigateTo(Constants.ItemGroupServiceEndpointName)
                .FindEntriesAsync();

            var pimItemGroups = MapPimItemGroup(groups);

            return pimItemGroups;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PimProductGroup>> GetAllProductGroupAsync()
        {
            var client = ODataClientSingleton.Get();

            var groups = await client
                .For(Constants.Company).Key(Constants.CompanyName)
                .NavigateTo(Constants.ProductGroupServiceEndpointName)
                .FindEntriesAsync();

            var pimProductGroup = MapPimProductGroup(groups);

            return pimProductGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemGroups"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemGroups"></param>
        /// <returns></returns>
        private IEnumerable<PimProductGroup> MapPimProductGroup(IEnumerable<IDictionary<string, object>> productGroups)
        {
            List<PimProductGroup> pimItemGroups = new List<PimProductGroup>();
            foreach (var productGroup in productGroups)
            {
                var pimProductGroup = MapPimProductGroup(productGroup);
                pimItemGroups.Add(pimProductGroup);
            }

            return pimItemGroups;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        private PimProductGroup MapPimProductGroup(IDictionary<string, object> keyword)
        {
            var productGroup = new PimProductGroup
            {
                Code = (string)keyword["Code"],
                Description = (string)keyword["Description"],
                Description_2 = (string)keyword["Description_2"],
                System_Status = (string)keyword["System_Status"],
                LocalDescription = (string)keyword["LocalDescription"],
                LocalDescription2 = (string)keyword["LocalDescription2"],
                Base_Unit = (string)keyword["Base_Unit"],
                Standard_Chapter = (string)keyword["Standard_Chapter"],
                Picture_Document_ID = (string)keyword["Picture_Document_ID"],
                Template_Code = (string)keyword["Template_Code"],
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
            return productGroup;
        }
    }
}
