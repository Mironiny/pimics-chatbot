using PimBotDp.State;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PimBot.Service.Impl
{
    public class SalesOrderService : ISalesOrderService
    {
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
    }
}
