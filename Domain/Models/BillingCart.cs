using CSharp.Choices;
using System;
using System.Collections.Generic;

namespace Domain.Models
{
    [AsChoice]
    public static partial class BillingCart
    {
        public interface IBillingCart { }

        public record UnbilledOrdersCart : IBillingCart
        {
            public UnbilledOrdersCart(IReadOnlyCollection<ValidatedCustomerOrder> ordersList)
            {
                OrdersList = ordersList;
            }
            public IReadOnlyCollection<ValidatedCustomerOrder> OrdersList { get; }
        }

        public record BillingPreparationCart : IBillingCart
        {
            public BillingPreparationCart(IReadOnlyCollection<ValidatedCustomerOrder> ordersList)
            {
                OrdersList = ordersList;
            }
            public IReadOnlyCollection<ValidatedCustomerOrder> OrdersList { get; }
        }

        public record FailedBillingCart : IBillingCart
        {
            public FailedBillingCart(string reason)
            {
                Reason = reason;
            }
            public string Reason { get; }
        }

        public record BilledOrdersCart : IBillingCart
        {
            public BilledOrdersCart(IReadOnlyCollection<BilledOrderLine> billedOrderLines, string invoiceNumber, decimal totalAmountBilled, DateTime billingDate)
            {
                BilledOrderLines = billedOrderLines;
                InvoiceNumber = invoiceNumber;
            }
            public IReadOnlyCollection<BilledOrderLine> BilledOrderLines { get; }
            public string InvoiceNumber { get; }
            public decimal AmountBilled { get; }
        }

        public record CheckedBillingOrder : IBillingCart
        {
            public CheckedBillingOrder(IReadOnlyCollection<BilledOrderLine> billedOrderLine)
            {
                BilledOrderLine = billedOrderLine;
            }
            public IReadOnlyCollection<BilledOrderLine> BilledOrderLine { get; }
        }
    }
}
