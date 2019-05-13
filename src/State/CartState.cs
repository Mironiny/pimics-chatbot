// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 18.02.2019
// ===

using System.Collections.Generic;
using PimBot.Dto;

namespace PimBot.State
{
    /// <summary>
    /// Cart state object.
    /// </summary>
    public class CartState
    {
        public List<PimItem> Items { get; set; }
    }
}
