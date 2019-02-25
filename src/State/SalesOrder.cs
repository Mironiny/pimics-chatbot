using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PimBotDp.State
{
    public class SalesOrder
    {
        public string Document_Type { get; set; }
        public string No { get; set; }
        public string Sell_to_Customer_Name { get; set; }
        public string Quote_No { get; set; }
        public string Sell_to_Address { get; set; }
        public string Sell_to_Address_2 { get; set; }
        public string Sell_to_Post_Code { get; set; }
        public string Sell_to_City { get; set; }
        public string Sell_to_Contact_No { get; set; }
        public string Sell_to_Contact { get; set; }
        public int No_of_Archived_Versions { get; set; }
        public DateTime Document_Date { get; set; }
        public DateTime Posting_Date { get; set; }
        public DateTime Order_Date { get; set; }
        public DateTime Due_Date { get; set; }
        public DateTime Requested_Delivery_Date { get; set; }
        public DateTime Promised_Delivery_Date { get; set; }
        public string External_Document_No { get; set; }
        public string Salesperson_Code { get; set; }
        public string Campaign_No { get; set; }
        public string Opportunity_No { get; set; }
        public string Responsibility_Center { get; set; }
        public string Assigned_User_ID { get; set; }
        public string Job_Queue_Status { get; set; }
        public string Status { get; set; }
        public string WorkDescription { get; set; }
        public string Currency_Code { get; set; }
        public bool Prices_Including_VAT { get; set; }
        public string VAT_Bus_Posting_Group { get; set; }
        public string Payment_Terms_Code { get; set; }
        public string Payment_Method_Code { get; set; }
        public string SelectedPayments { get; set; }
        public string Transaction_Type { get; set; }
        public string Shortcut_Dimension_1_Code { get; set; }
        public string Shortcut_Dimension_2_Code { get; set; }
        public int Payment_Discount_Percent { get; set; }
        public DateTime Pmt_Discount_Date { get; set; }
        public string Direct_Debit_Mandate_ID { get; set; }
        public string ShippingOptions { get; set; }
        public string Ship_to_Code { get; set; }
        public string Ship_to_Name { get; set; }
        public string Ship_to_Address { get; set; }
        public string Ship_to_Address_2 { get; set; }
        public string Ship_to_Post_Code { get; set; }
        public string Ship_to_City { get; set; }
        public string Ship_to_Country_Region_Code { get; set; }
        public string Ship_to_Contact { get; set; }
        public string Shipment_Method_Code { get; set; }
        public string Shipping_Agent_Code { get; set; }
        public string Shipping_Agent_Service_Code { get; set; }
        public string Package_Tracking_No { get; set; }
        public string BillToOptions { get; set; }
        public string Bill_to_Name { get; set; }
        public string Bill_to_Address { get; set; }
        public string Bill_to_Address_2 { get; set; }
        public string Bill_to_Post_Code { get; set; }
        public string Bill_to_City { get; set; }
        public string Bill_to_Contact_No { get; set; }
        public string Bill_to_Contact { get; set; }
        public string Location_Code { get; set; }
        public DateTime Shipment_Date { get; set; }
        public string Shipping_Advice { get; set; }
        public string Outbound_Whse_Handling_Time { get; set; }
        public string Shipping_Time { get; set; }
        public bool Late_Order_Shipping { get; set; }
        public bool EU_3_Party_Trade { get; set; }
        public string Transaction_Specification { get; set; }
        public string Transport_Method { get; set; }
        public string Exit_Point { get; set; }
        public string Area { get; set; }
        public int Prepayment_Percent { get; set; }
        public bool Compress_Prepayment { get; set; }
        public string Prepmt_Payment_Terms_Code { get; set; }
        public DateTime Prepayment_Due_Date { get; set; }
        public int Prepmt_Payment_Discount_Percent { get; set; }
        public DateTime Prepmt_Pmt_Discount_Date { get; set; }
        public string Date_Filter { get; set; }
        public string ETag { get; set; }
    }
}
