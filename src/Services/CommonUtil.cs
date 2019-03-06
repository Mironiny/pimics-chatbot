using System;

namespace PimBot.Services
{
    public class CommonUtil
    {
        public static bool ContainsIgnoreCase(string sentance, string key)
        {
            return sentance.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
