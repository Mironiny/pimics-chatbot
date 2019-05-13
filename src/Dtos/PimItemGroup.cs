// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 4.5.2019
// ===

namespace PimBot.Dto
{
    /// <summary>
    /// PimItemGroup dto class.
    /// </summary>
    public class PimItemGroup : PimGroup
    {
        public string Item_Template_Code { get; set; }

        public string Standard_Product_Group { get; set; }
    }
}
