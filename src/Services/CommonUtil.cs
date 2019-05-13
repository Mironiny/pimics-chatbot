// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 19.3.2019
// ===

using System;
using System.Linq;

namespace PimBot.Services
{
    /// <summary>
    /// Class contains usefull util methods for another classes in solution.
    /// </summary>
    public class CommonUtil
    {
        public static bool ContainsIgnoreCase(string sentance, string key)
        {
            return sentance.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool CompareTokenByToken(string sentence, string key)
        {
            string[] tokens = sentence.Split(new char[] {' ', '.', ','});
            tokens.Where(t => ContainsIgnoreCase(key, t)).ToString();
            return tokens.Any();
        }

        /// <summary>
        /// Compute the distance between two strings - taken from https://www.dotnetperls.com/levenshtein
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int ComputeLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                // Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            // Step 7
            return d[n, m];
        }
    }
}
