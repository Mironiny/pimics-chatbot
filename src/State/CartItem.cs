namespace PimBot.State
{
    public class CartItem
    {
        public CartItem(string description)
        {
            Description = description;
        }

        public string No { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string MessureUnit { get; set; }

        public string UnitCost { get; set; }

        public string UnitPrice { get; set; }

        public int VendorNo { get; set; }

        public int Count { get; set; }
    }
}
