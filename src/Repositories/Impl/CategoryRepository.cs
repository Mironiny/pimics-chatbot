// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date:
// ===

using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dto;

namespace PimBot.Repositories.Impl
{
    /// <summary>
    /// Class responsible for getting categories (implementation).
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        public async Task<IEnumerable<PimItemGroup>> GetAllItemGroup()
        {
            var client = ODataClientSingleton.Get();

            var groups = await client
                .For(Constants.Company).Key(Constants.CompanyName)
                .NavigateTo(Constants.ItemGroupServiceEndpointName)
                .FindEntriesAsync();

            var pimItemGroups = MapPimItemGroup(groups);

            return pimItemGroups;
        }

        public async Task<IEnumerable<PimProductGroup>> GetAllProductGroup()
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
        /// Map pimitem group.
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
        /// Map pim product group.
        /// </summary>
        /// <param name="itemGroups">ItemGroups.</param>
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
        /// Map item Group.
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
                ETag = (string)keyword["ETag"],
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
                ETag = (string)keyword["ETag"],
            };
            return productGroup;
        }
    }
}
