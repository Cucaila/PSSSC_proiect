using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [AsChoice]
    public static partial class ShippingOrderEvent
    {
        public interface IShippingOrderEvent { }

        public record ShippingOrderSucceededEvent : IShippingOrderEvent
        {
            public decimal AmountBilled { get; }
            public DateTime BillingDate { get; }
            public string InvoiceNumber { get; }
            public IReadOnlyCollection<ShippedOrder> BilledOrderLines { get; }

            internal ShippingOrderSucceededEvent(IReadOnlyCollection<ShippedOrder> billedOrderLines, string invoiceNumber, decimal amountBilled, DateTime billingDate)
            {
                BilledOrderLines = billedOrderLines;
                InvoiceNumber = invoiceNumber;
                AmountBilled = amountBilled;
                BillingDate = billingDate;
            }
        }

        public record ShippingOrderFailedEvent : IShippingOrderEvent
        {
            public string Reason { get; }
            public ShippingOrderFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
