namespace PimBot.State
{
    public class CustomerState
    {
        public int SmallTalkCount { get; set; }

        public string Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string AddressSecondary { get; set; }

        public string PostCode { get; set; }

        public string City { get; set; }

        // Nullable boolean
        public bool? IsShippingAdressMatch { get; set; }

        public string ShippingName { get; set; }

        public string ShippingAddress { get; set; }

        public string ShippingAddressSecondary { get; set; }

        public string ShippingPostCode { get; set; }

        public string ShippingCity { get; set; }

    }
}
