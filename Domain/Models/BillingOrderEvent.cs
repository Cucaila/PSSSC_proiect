using CSharp.Choices;
using System;
using System.Collections.Generic;

namespace Domain.Models
{
    [AsChoice]
    public static partial class BillingOrderEvent
    {
        public interface IBillingOrderEvent { }

        public record BillingOrderSucceededEvent : IBillingOrderEvent
        {
            public decimal AmountBilled { get; }
            public DateTime BillingDate { get; }
            public string InvoiceNumber { get; }
            public IReadOnlyCollection<BilledOrderLine> BilledOrderLines { get; }

            internal BillingOrderSucceededEvent(IReadOnlyCollection<BilledOrderLine> billedOrderLines, string invoiceNumber, decimal amountBilled, DateTime billingDate)
            {
                BilledOrderLines = billedOrderLines;
                InvoiceNumber = invoiceNumber;
                AmountBilled = amountBilled;
                BillingDate = billingDate;
            }
        }

        public record BillingOrderFailedEvent : IBillingOrderEvent
        {
            public string Reason { get; }
            public BillingOrderFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
