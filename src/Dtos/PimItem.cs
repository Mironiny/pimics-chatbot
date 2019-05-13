// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 4.5.2019
// ===

using System.Collections.Generic;

namespace PimBot.Dto
{
    /// <summary>
    /// PimItem dto class.
    /// </summary>
    public class PimItem
    {
        // Not in PIM - represents how many item you order
        public int Count { get; set; }

        // Not in PIM
        public List<PimItemGroup> PimItemGroups { get; set; }

        // Not in PIM
        public List<PimFeature> PimFeatures { get; set; }

        public string No { get; set; }

        public string Systemstatus { get; set; }

        public string Description { get; set; }

        public bool Assembly_BOM { get; set; }

        public string Base_Unit_of_Measure { get; set; }

        public string Shelf_No { get; set; }

        public string Costing_Method { get; set; }

        public decimal Standard_Cost { get; set; }

        public decimal Unit_Cost { get; set; }

        public decimal Last_Direct_Cost { get; set; }

        public string Price_Profit_Calculation { get; set; }

        public decimal Profit_Percent { get; set; }

        public decimal Unit_Price { get; set; }

        public string Inventory_Posting_Group { get; set; }

        public string Gen_Prod_Posting_Group { get; set; }

        public string VAT_Prod_Posting_Group { get; set; }

        public string Vendor_No { get; set; }

        public string Vendor_Item_No { get; set; }

        public string Tariff_No { get; set; }

        public string Search_Description { get; set; }

        public string Durability { get; set; }

        public string Picture_Document_ID { get; set; }

        public string Standardartikelgruppe { get; set; }

        public string Base_Class_No { get; set; }

        public string Item_Category_Code { get; set; }

        public string Product_Group_Code { get; set; }

        public string ETag { get; set; }
    }
}
