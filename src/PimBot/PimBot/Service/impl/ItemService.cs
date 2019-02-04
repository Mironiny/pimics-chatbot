using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PimBot.Service.impl
{
    public class ItemService : IItemService
    {
        public async Task<IEnumerable<string>> GetAllItemsAsync()
        {
            var itemList = new List<string>();

            var client = ODataClientSingleton.Get();

            var items = await client
                .For(Constants.ItemsServiceEndpointName)
                .FindEntriesAsync();

            foreach (var item in items)
            {
                itemList.Add((string) item["Description"]);          
            }

            return itemList;
        }
    }
}
