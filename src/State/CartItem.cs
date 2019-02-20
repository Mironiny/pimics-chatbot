using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PimBotDp.State
{
    public class CartItem
    {
        public CartItem(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public int Count { get; set; }
    }
}
