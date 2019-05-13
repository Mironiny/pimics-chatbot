// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date:
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
