// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 19.03.2019
// ===

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PimBot.Dto;
using PimBot.Repositories;
using PimBot.State;

namespace PimBot.Services.Impl
{
    /// <summary>
    /// Service for handling sales orders (implementation).
    /// </summary>
    public class SalesOrderService : ISalesOrderService
    {
        public async Task<IEnumerable<SalesOrder>> GetSalesOrderByCustomer(string customerNo)
        {
            var client = ODataClientSingleton.Get();

            var orders = await client
                .For(Constants.Company).Key(Constants.CompanyName)
                .NavigateTo("SalesOrder")
         //       .Filter($"Sell_to_Customer_Name%20eq%20%27{customerNo}%27")
                .FindEntriesAsync();

            var mapOrders = MapOrder(orders).Where(o => o.Sell_to_Customer_Name == customerNo).ToList();
            return mapOrders;
        }

        public async Task<bool> CreateOrder(CustomerState customerState)
        {
            var client = ODataClientSingleton.Get();
            try
            {
                var product = await client
                    .For<SalesOrder>("SalesOrder")
                    .Set(CreateSaleOrder(customerState))
                    .InsertEntryAsync();
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                return false;
            }

            return true;
        }

        private SalesOrder CreateSaleOrder(CustomerState customerState)
        {
            var saleOrder = new SalesOrder();
            saleOrder.No = RandomString(7);
            saleOrder.Document_Type = "Order";
            saleOrder.Sell_to_Customer_Name = customerState.Name;
            saleOrder.Sell_to_Address = customerState.ShippingAddress;
            saleOrder.Sell_to_Post_Code = customerState.PostCode;
            saleOrder.Sell_to_City = customerState.City;
            return saleOrder;
        }

        private SalesOrder CreateDummyOrder()
        {
            var saleOrder = new SalesOrder();
            saleOrder.No = RandomString(7);
            saleOrder.Document_Type = "Order";
            return saleOrder;
        }

        private static Random random = new Random();

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public IEnumerable<SalesOrder> MapOrder(IEnumerable<IDictionary<string, object>> orders)
        {
            List<SalesOrder> pimorders = new List<SalesOrder>();
            foreach (var order in orders)
            {
                var pimOrder = MapPimOrder(order);
                pimorders.Add(pimOrder);
            }

            return pimorders;
        }

        private SalesOrder MapPimOrder(IDictionary<string, object> keyword)
        {
            var order = new SalesOrder();

            order.Document_Type = (string) keyword["Document_Type"];
            order.No = (string) keyword["No"];
            order.Sell_to_Customer_Name = (string) keyword["Sell_to_Customer_Name"];
            order.Quote_No = (string) keyword["Quote_No"];
            order.Sell_to_Address = (string) keyword["Sell_to_Address"];
            order.Sell_to_Address_2 = (string) keyword["Sell_to_Address_2"];
            order.Sell_to_Post_Code = (string) keyword["Sell_to_Post_Code"];
            order.Sell_to_City = (string) keyword["Sell_to_City"];
            order.Sell_to_Contact_No = (string) keyword["Sell_to_Contact_No"];
            order.Sell_to_Contact = (string) keyword["Sell_to_Contact"];
//              order.  No_of_Archived_Versions = (int) keyword["No_of_Archived_Versions"],
            order.External_Document_No = (string) keyword["External_Document_No"];
            order.Salesperson_Code = (string) keyword["Salesperson_Code"];
            order.Campaign_No = (string) keyword["Campaign_No"];
            order.Opportunity_No = (string) keyword["Opportunity_No"];
            order.Responsibility_Center = (string) keyword["Responsibility_Center"];
            order.Assigned_User_ID = (string) keyword["Assigned_User_ID"];
            order.Job_Queue_Status = (string) keyword["Job_Queue_Status"];
            order.Status = (string) keyword["Status"];
            order.WorkDescription = (string) keyword["WorkDescription"];
            order.Currency_Code = (string) keyword["Currency_Code"];
            order.Prices_Including_VAT = (bool) keyword["Prices_Including_VAT"];
            order.VAT_Bus_Posting_Group = (string) keyword["VAT_Bus_Posting_Group"];
            order.Payment_Terms_Code = (string) keyword["Payment_Terms_Code"];
                order.Payment_Method_Code = (string) keyword["Payment_Method_Code"];
                order.SelectedPayments = (string) keyword["SelectedPayments"];
                order.Transaction_Type = (string) keyword["Transaction_Type"];
                order.Shortcut_Dimension_1_Code = (string) keyword["Shortcut_Dimension_1_Code"];
                order.Shortcut_Dimension_2_Code = (string) keyword["Shortcut_Dimension_2_Code"];
//              order.  Payment_Discount_Percent = (int) keyword["Payment_Discount_Percent"],
            order.Direct_Debit_Mandate_ID = (string) keyword["Direct_Debit_Mandate_ID"];
            order.ShippingOptions = (string) keyword["ShippingOptions"];
                order.Ship_to_Code = (string) keyword["Ship_to_Code"];
                order.Ship_to_Name = (string) keyword["Ship_to_Name"];
                order.Ship_to_Address = (string) keyword["Ship_to_Address"];
                order.Ship_to_Address_2 = (string) keyword["Ship_to_Address_2"];
                order.Ship_to_Post_Code = (string) keyword["Ship_to_Post_Code"];
                order.Ship_to_City = (string) keyword["Ship_to_City"];
                order.Ship_to_Country_Region_Code = (string) keyword["Ship_to_Country_Region_Code"];
                order.Ship_to_Contact = (string) keyword["Ship_to_Contact"];
                order.Shipment_Method_Code = (string) keyword["Shipment_Method_Code"];
                order.Shipping_Agent_Code = (string) keyword["Shipping_Agent_Code"];
                order.Shipping_Agent_Service_Code = (string) keyword["Shipping_Agent_Service_Code"];
                order.Package_Tracking_No = (string) keyword["Package_Tracking_No"];
                order.BillToOptions = (string) keyword["BillToOptions"];
                order.Bill_to_Name = (string) keyword["Bill_to_Name"];
                order.Bill_to_Address = (string) keyword["Bill_to_Address"];
                order.Bill_to_Address_2 = (string) keyword["Bill_to_Address_2"];
                order.Bill_to_Post_Code = (string) keyword["Bill_to_Post_Code"];
                order.Bill_to_City = (string) keyword["Bill_to_City"];
                order.Bill_to_Contact_No = (string) keyword["Bill_to_Contact_No"];
                order.Bill_to_Contact = (string) keyword["Bill_to_Contact"];
                order.Location_Code = (string) keyword["Location_Code"];
                order.Shipping_Advice = (string) keyword["Shipping_Advice"];
                order.Outbound_Whse_Handling_Time = (string) keyword["Outbound_Whse_Handling_Time"];
                order.Shipping_Time = (string) keyword["Shipping_Time"];
                order.Late_Order_Shipping = (bool) keyword["Late_Order_Shipping"];
                order.EU_3_Party_Trade = (bool) keyword["EU_3_Party_Trade"];
                order.Transaction_Specification = (string) keyword["Transaction_Specification"];
                order.Transport_Method = (string) keyword["Transport_Method"];
                order.Exit_Point = (string) keyword["Exit_Point"];
                order.Area = (string) keyword["Area"];
//              order.  Prepayment_Percent = (int) keyword["Prepayment_Percent"],
            order.Compress_Prepayment = (bool) keyword["Compress_Prepayment"];
            order.Prepmt_Payment_Terms_Code = (string) keyword["Prepmt_Payment_Terms_Code"];
       //     order.Prepmt_Payment_Discount_Percent = (int) keyword["Prepmt_Payment_Discount_Percent"];
            order.Date_Filter = (string) keyword["Date_Filter"];
            order.ETag = (string) keyword["ETag"];

            return order;
        }

    }

    public class Order
    {
        public List<SalesOrder> value { get; set; }
    }
}
