using System;
using System.Collections.Generic;

namespace PimBot.State
{
    /// <summary>
    /// Ordered status.
    /// </summary>
    public enum OrderStatus
    {
        OrderCancelled,
        OrderDelivered, OrderInTransit,
        OrderPaymentDue, OrderPickupAvailable, OrderProblem, OrderProcessing, OrderReturned
    };

    public class OrderState : CartState
    {
        public OrderState(List<PimItem> items, DateTime currentDateTime, OrderStatus status)
        {
            Items = items;
            CreateDateTime = currentDateTime;
            Status = status.ToString();
        }

        public DateTime CreateDateTime { get; set; }

        public string Status { get; set; }
    }
}
