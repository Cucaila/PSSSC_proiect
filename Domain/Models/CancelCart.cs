using CSharp.Choices;
using Domain.Models.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Models
    {
        [AsChoice]
        public static partial class OrdersCancellationCart
        {
            public interface IOrdersCancellationCart { }

            public record UnvalidatedOrdersCancellationCart : IOrdersCancellationCart
            {
                public UnvalidatedOrdersCancellationCart(IReadOnlyCollection<UnvalidatedOrdersCancellationCart> cancellationList)
                {
                    CancellationList = cancellationList;
                }
            public IReadOnlyCollection<UnvalidatedOrdersCancellationCart> CancellationList { get; }
            }

            public record InvalidatedOrdersCancellationCart : IOrdersCancellationCart
            {
                internal InvalidatedOrdersCancellationCart(IReadOnlyCollection<UnvalidatedCustomerOrderCancellation> cancellationList, string reason)
                {
                    CancellationList = cancellationList;
                    Reason = reason;
                }
                public IReadOnlyCollection<UnvalidatedCustomerOrderCancellation> CancellationList { get; }
                public string Reason { get; }
            }

            public record FailedCancellationCart : IOrdersCancellationCart
            {
                internal FailedCancellationCart(string reason)
                {
                    Reason = reason;
                }
                public string Reason { get; }
            }

            public record ValidatedOrdersCancellationCart : IOrdersCancellationCart
            {
                internal ValidatedOrdersCancellationCart(IReadOnlyCollection<ValidatedCustomerOrderCancellation> cancellationsList)
                {
                    CancellationsList = cancellationsList;
                }
                public IReadOnlyCollection<ValidatedCustomerOrderCancellation> CancellationsList { get; }
            }

            public record CalculatedCancellation : IOrdersCancellationCart
            {
                internal CalculatedCancellation(IReadOnlyCollection<CalculateCustomerOrderCancellation> cancellationsList)
                {
                    CancellationsList = cancellationsList;
                }
                public IReadOnlyCollection<CalculateCustomerOrderCancellation> CancellationsList { get; }
            }

            public record CheckedCancellationByCode : IOrdersCancellationCart
            {
                internal CheckedCancellationByCode(IReadOnlyCollection<CheckOrderCancellation> checkCancellation)
                {
                    CheckCancellation = checkCancellation;
                }
                public IReadOnlyCollection<CheckOrderCancellation> CheckCancellation { get; }
            }

            public record CanceledOrder : IOrdersCancellationCart
            {
                internal CanceledOrder(IReadOnlyCollection<CalculateCustomerOrderCancellation> calculateCustomerOrderCancellations, string csv, int numberOfCancellation, DateTime canceledDate)
                {
                    CalculateCustomerOrderCancellations = calculateCustomerOrderCancellations;
                    NumberOfCancellation = numberOfCancellation;
                    CanceledDate = canceledDate;
                    Csv = csv;
                }
                public IReadOnlyCollection<CalculateCustomerOrderCancellation> CalculateCustomerOrderCancellations { get; }
                public int NumberOfCancellation { get; }
                public DateTime CanceledDate { get; }
                public string Csv { get; }
            }
        }
    }
