using System.Collections.Generic;
using System.Threading.Tasks;
using PimBot.Dto;

namespace PimBot.Repositories.Impl
{
    public class ItemRepository : IItemRepository
    {
        public async Task<IEnumerable<PimItem>> GetAll()
        {
            var client = ODataClientSingleton.Get();
            var items = await client
                .For(Constants.Company).Key(Constants.CompanyName)
                .NavigateTo(Constants.ItemsServiceEndpointName)
                .FindEntriesAsync();

            return MapItems(items);
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
            var pimItem = new PimItem
            {
                No = (string)item["No"],
                Description = (string)item["Description"],
                Systemstatus = (string)item["Systemstatus"],
                Assembly_BOM = (bool)item["Assembly_BOM"],
                Base_Unit_of_Measure = (string)item["Base_Unit_of_Measure"],
                Shelf_No = (string)item["Shelf_No"],
                Costing_Method = (string)item["Costing_Method"],
                Standard_Cost = (decimal)item["Standard_Cost"],
                Unit_Cost = (decimal)item["Unit_Cost"],
                Last_Direct_Cost = (decimal)item["Last_Direct_Cost"],
                Price_Profit_Calculation = (string)item["Price_Profit_Calculation"],
                Profit_Percent = (decimal)item["Profit_Percent"],
                Unit_Price = (decimal)item["Unit_Price"],
                Inventory_Posting_Group = (string)item["Inventory_Posting_Group"],
                Gen_Prod_Posting_Group = (string)item["Gen_Prod_Posting_Group"],
                VAT_Prod_Posting_Group = (string)item["VAT_Prod_Posting_Group"],
                Vendor_No = (string)item["Vendor_No"],
                Vendor_Item_No = (string)item["Vendor_Item_No"],
                Tariff_No = (string)item["Tariff_No"],
                Search_Description = (string)item["Search_Description"],
                Durability = (string)item["Durability"],
                Picture_Document_ID = (string)item["Picture_Document_ID"],
                Standardartikelgruppe = (string)item["Standardartikelgruppe"],
                Base_Class_No = (string)item["Base_Class_No"],
                Item_Category_Code = (string)item["Item_Category_Code"],
                Product_Group_Code = (string)item["Product_Group_Code"],
                ETag = (string)item["ETag"],
            };
            return pimItem;
        }
    }
}
