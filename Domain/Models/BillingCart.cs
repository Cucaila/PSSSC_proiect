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
            public UnbilledOrdersCart(IReadOnlyCollection<UnvalidatedBillingOrder> ordersList)
            {
                OrdersList = ordersList;
            }
            public IReadOnlyCollection<UnvalidatedBillingOrder> OrdersList { get; }
        }

        public record BillingPreparationCart : IBillingCart
        {
            public BillingPreparationCart(IReadOnlyCollection<UnvalidatedBillingOrder> ordersList, string reason)
            {
                OrdersList = ordersList;
                Reason = reason;
            }
            public IReadOnlyCollection<UnvalidatedBillingOrder> OrdersList { get; }
            public string Reason { get; }
        }

        public record FailedBillingCart : IBillingCart
        {
            public FailedBillingCart(string reason)
            {
                Reason = reason;
            }
            public string Reason { get; }
        }

        public record ValidatedBilledOrdersCart : IBillingCart
        {
            internal ValidatedBilledOrdersCart(IReadOnlyCollection<ValidatedBillingLine> ordersList)
            {
                OrdersList = ordersList;
            }
            public IReadOnlyCollection<ValidatedBillingLine> OrdersList { get; }
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

        public record CalculatedBillingOrder : IBillingCart
        {
            internal CalculatedBillingOrder(IReadOnlyCollection<BilledOrderLine> ordersList)
            {
                OrdersList = ordersList;
            }
            public IReadOnlyCollection<BilledOrderLine> OrdersList { get; }
        }

        public record CheckedBillingOrder : IBillingCart
        {
            public CheckedBillingOrder(IReadOnlyCollection<ValidatedBillingLine> billedOrderLine)
            {
                BilledOrderLine = billedOrderLine;
            }
            public IReadOnlyCollection<ValidatedBillingLine> BilledOrderLine { get; }
        }
    }
}
