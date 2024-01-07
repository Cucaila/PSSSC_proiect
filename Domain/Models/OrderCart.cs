using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [AsChoice]
    public static partial class OrdersCart
    {
        public interface IOrdersCart { }

        public record UnvalidatedOrdersCart : IOrdersCart
        {
            public UnvalidatedOrdersCart(IReadOnlyCollection<UnvalidatedCustomerOrder> orderList)
            {
                OrderList = orderList;
            }
            public IReadOnlyCollection<UnvalidatedCustomerOrder> OrderList { get; }
        }

        public record InvalidatedOrdersCart : IOrdersCart
        {
            internal InvalidatedOrdersCart(IReadOnlyCollection<UnvalidatedCustomerOrder> orderList, string reason)
            {
                OrderList = orderList;
                Reason = reason;
            }
            public IReadOnlyCollection<UnvalidatedCustomerOrder> OrderList { get; }
            public string Reason { get; }
        }

        public record FailedCart : IOrdersCart
        {
            internal FailedCart(string reason)
            {
                Reason = reason;
            }
            public string Reason { get; }
        }

        public record ValidatedOrdersCart : IOrdersCart
        {
            internal ValidatedOrdersCart(IReadOnlyCollection<ValidatedCustomerOrder> ordersList)
            {
                OrdersList = ordersList;
            }
            public IReadOnlyCollection<ValidatedCustomerOrder> OrdersList { get; }
        }

        public record CalculatedOrder : IOrdersCart
        {
            internal CalculatedOrder(IReadOnlyCollection<CalculateCustomerOrder> ordersList)
            {
                OrdersList = ordersList;
            }
            public IReadOnlyCollection<CalculateCustomerOrder> OrdersList { get; }
        }

        public record CheckedOrderByCode : IOrdersCart
        {
            internal CheckedOrderByCode(IReadOnlyCollection<CheckOrder> checkOrder)
            {
                CheckOrder = checkOrder;
            }
            public IReadOnlyCollection<CheckOrder> CheckOrder { get; }
        }
        public record PlacedOrder : IOrdersCart
        {
            internal PlacedOrder(IReadOnlyCollection<CalculateCustomerOrder> calculateCustomerOrders, string csv, int numberOfOrder, DateTime placedDate)
            {
                CalculateCustomerOrders = calculateCustomerOrders;
                NumberOfOrder = numberOfOrder;
                PlacedDate = placedDate;
                Csv = csv;
            }
            public IReadOnlyCollection<CalculateCustomerOrder> CalculateCustomerOrders { get; }
            public int NumberOfOrder { get; }
            public DateTime PlacedDate { get; }
            public string Csv { get; }

        }


    }
}
