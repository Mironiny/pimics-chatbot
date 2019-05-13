// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 21.02.2019
// ===

using System.Collections.Generic;

namespace PimBot.State
{
    /// <summary>
    /// Customer state object.
    /// </summary>
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

        public CartState Cart { get; set; }

    }
}
