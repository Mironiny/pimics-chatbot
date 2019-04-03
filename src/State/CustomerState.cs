using System.Collections.Generic;
using PimBot.Service.Impl;

namespace PimBot.State
{
    public class CustomerState
    {
        public int SmallTalkCount { get; set; }

        public string Login { get; set; }

        public string CustomerNo { get; set; }

        public bool IsAuthenticated { get; set; }

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

        public List<OrderState> Orders { get; set; }

        public List<CartState> Cart { get; set; }

    }
}
