using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PimBotDp.Services
{
    public class CommonUtil
    {
        public static bool ContainsIgnoreCase(string sentance, string key)
        {
            return sentance.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
