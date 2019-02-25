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
        public async Task CreateOrder(CustomerState customerState)
        {
            var client = ODataClientSingleton.Get();

            //            await client.For("Company").Key("CRONUS%20International%20Ltd.").NavigateTo("Keywords").Set(CreateDummyKeyword())
            //                .InsertEntryAsync();

            var items = await client
                .For("SalesOrder")
                .FindEntriesAsync();

            await client
                .For<Keyword>("Keywords")
                .Set(CreateDummyKeyword())
                .InsertEntryAsync();

//            var product = await client
//                    .For<SalesOrder>("SalesOrder")
//                    .Set(CreateDummyOrder())
//                    .InsertEntryAsync();

//            var product = await client
//                    .For("KeywordsPIM")
//                    .Set(new
//                    {
//                        Source = "Item",
//                        Group_System_Number = "INTERNAL",
//                        Code = "1000",
//                        Source_Type = "Item",
//                        Source_Code = "",
//                        Line_No = "1000",
//                        Keyword_ID = "INT100007",
//                        Keyword = "Bicycle1",
//                        Classification_System = "Internal",
//                        Classification_System_Version = "1.1",
//                        Usage_Type_Code = "",
//                        Inherited = false,
//                    //
//                    //                    Description = "test2",
//                    //                    Beschreibung = "",
//                    //                    Usage_Table_of_Contents = false,
//                    //                    Group_System_Number = "INTERNAL",
//                    //                    Created_On = DateTimeOffset.Parse("2018-06-01T23:11:17.5479185-07:00"),
//                    //                    Updated_By = "PIMICS-CHATBOT\\ALLIUM",
//                })
//                    .InsertEntryAsync();

            //            var product = await client
            //                .For("SalesOrder")
            //                .Set(new { Document_Type = "Order", No = "111111", Sell_to_Customer_Name = "Mirek", })
            //                .InsertEntryAsync();
        }

        private Keyword CreateDummyKeyword()
        {
            var keyword = new Keyword();
            keyword.Keyword_ID = "INT100007";
            keyword.Description = "test2";
            keyword.Beschreibung = "";
            keyword.Usage_Table_of_Contents = false;
            keyword.Group_System_Number = "INTERNAL";
            keyword.Created_On = DateTime.Parse("2018-06-01T23:11:17.5479185-07:00");
            keyword.Updated_By = "PIMICS-CHATBOT\\ALLIUM";
            keyword.ETag = "netusimcosemdat";
            return keyword;
        }

        private SalesOrder CreateDummyOrder()
        {
            var saleOrder = new SalesOrder();
            saleOrder.No = "100005";
            saleOrder.Document_Type = "Order";

            return saleOrder;
        }
    }
}
