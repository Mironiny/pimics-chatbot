using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PimBot.Service
{
    public interface IItemService
    {
        Task<IEnumerable<string>> GetAllItemsAsync();
    }
}
