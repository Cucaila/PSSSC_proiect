using CSharp.Choices;
using System;
using System.Collections.Generic;

namespace Domain.Models
{
    [AsChoice]
    public static partial class OrdersCancellation
    {
        public interface IOrdersCancellation { }

        public record UnvalidatedOrdersCancellation : IOrdersCancellation
        {
            public UnvalidatedOrdersCancellation(IReadOnlyCollection<UnvalidatedCustomerOrderCancellation> orderCancellationList)
            {
                OrderCancellationList = orderCancellationList;
            }

            public IReadOnlyCollection<UnvalidatedCustomerOrderCancellation> OrderCancellationList { get; }
        }

        public record InvalidatedOrdersCancellation : IOrdersCancellation
        {
            internal InvalidatedOrdersCancellation(IReadOnlyCollection<UnvalidatedCustomerOrderCancellation> orderCancellationList, string reason)
            {
                OrderCancellationList = orderCancellationList;
                Reason = reason;
            }

            public IReadOnlyCollection<UnvalidatedCustomerOrderCancellation> OrderCancellationList { get; }
            public string Reason { get; }
        }

        public record FailedCancellation : IOrdersCancellation
        {
            internal FailedCancellation(IReadOnlyCollection<UnvalidatedCustomerOrderCancellation> orderCancellationList, string reason)
            {
                OrderCancellationList = orderCancellationList;
                Reason = reason;
            }

            public IReadOnlyCollection<UnvalidatedCustomerOrderCancellation> OrderCancellationList { get; }
            public string Reason { get; }
        }
        public record FailCancellation : IOrdersCancellation
        {
            internal FailCancellation(string reason)
            {
                Reason = reason;
            }
            public string Reason { get; }
        }

        public record ValidatedOrdersCancellation : IOrdersCancellation
        {
            internal ValidatedOrdersCancellation(IReadOnlyCollection<ValidatedCustomerOrderCancellation> cancellationOrdersList)
            {
                CancellationOrdersList = cancellationOrdersList;
            }

            public IReadOnlyCollection<ValidatedCustomerOrderCancellation> CancellationOrdersList { get; }
        }

        public record CalculatedCancellation : IOrdersCancellation
        {
            internal CalculatedCancellation(IReadOnlyCollection<CalculateCustomerOrder> cancellationOrdersList)
            {
                CancellationOrdersList = cancellationOrdersList;
            }

            public IReadOnlyCollection<CalculateCustomerOrder> CancellationOrdersList { get; }
        }

        public record CheckedOrderByCodeCancellation : IOrdersCancellation
        {
            internal CheckedOrderByCodeCancellation(IReadOnlyCollection<CheckOrder> checkOrder)
            {
                CheckOrder = checkOrder;
            }

            public IReadOnlyCollection<CheckOrder> CheckOrder { get; }
        }

        public record PlacedCancellationOrder : IOrdersCancellation
        {
            internal PlacedCancellationOrder(IReadOnlyCollection<CalculateCustomerOrder> calculateCancellationOrders, string csv, int numberOfCancellationOrder, DateTime placedDate)
            {
                CalculateCancellationOrders = calculateCancellationOrders;
                NumberOfCancellationOrder = numberOfCancellationOrder;
                PlacedDate = placedDate;
                Csv = csv;
            }

            public IReadOnlyCollection<CalculateCustomerOrder> CalculateCancellationOrders { get; }
            public int NumberOfCancellationOrder { get; }
            public DateTime PlacedDate { get; }
            public string Csv { get; }
        }
    }
}

