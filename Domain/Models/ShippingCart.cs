using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [AsChoice]
    public static class ShippingCart
    {
        public interface IShippingCart { }

        public record UnshipedOrdersCart : IShippingCart
        {
            public UnshipedOrdersCart(IReadOnlyCollection<ValidatedCustomerOrder> ordersList)
            {
                OrdersList = ordersList;
            }
            public IReadOnlyCollection<ValidatedCustomerOrder> OrdersList { get; }
        }

        public record ShippingOrderCart : IShippingCart
        {
            public ShippingOrderCart(IReadOnlyCollection<ValidatedCustomerOrder> ordersList)
            {
                OrdersList = ordersList;
            }
            public IReadOnlyCollection<ValidatedCustomerOrder> OrdersList { get; }
        }

        public record FailedShippingCart : IShippingCart
        {
            public FailedShippingCart(string reason)
            {
                Reason = reason;
            }
            public string Reason { get; }
        }

        public record ShipedOrdersCart : IShippingCart
        {
            public ShipedOrdersCart(IReadOnlyCollection<ShippedOrder> shippedOrder, string invoiceNumber, decimal totalAmountBilled, DateTime billingDate)
            {
                ShippedOrder = shippedOrder;
                InvoiceNumber = invoiceNumber;
            }
            public IReadOnlyCollection<ShippedOrder> ShippedOrder { get; }
            public string InvoiceNumber { get; }
            public decimal AmountBilled { get; }
        }

        public record CheckedBillingOrder : IShippingCart
        {
            public CheckedBillingOrder(IReadOnlyCollection<ShippedOrder> shippedOrder)
            {
                ShippedOrder = shippedOrder;
            }
            public IReadOnlyCollection<ShippedOrder> ShippedOrder { get; }
        }
    }
}

