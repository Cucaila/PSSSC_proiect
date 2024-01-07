using Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Models.OrdersCancellation;
using static LanguageExt.Prelude;

namespace Domain
{
    public static class CancelOrdersOperation
    {
        public static Task<IOrdersCancellation> ValidateAndProcessCancellationAsync(Func<OrderRegistrationCode, Option<OrderRegistrationCode>> checkOrderExist,UnvalidatedOrdersCancellation orders)
        {
            return orders.OrderCancellationList  
                .Select(ValidateCancellationOrder(checkOrderExist))
                .Aggregate(CreateEmptyValidatedCancellationOrdersList().ToAsync(), ReduceValidCancellationOrders)
                .MatchAsync(
                    Right: validatedCancellationOrders => new ValidatedOrdersCancellation(validatedCancellationOrders),
                    LeftAsync: errorMessage => Task.FromResult((IOrdersCancellation)new FailedCancellation(orders.OrderCancellationList, errorMessage)) // Update this line
                );
        }

        private static Func<UnvalidatedCustomerOrderCancellation, EitherAsync<string, ValidatedCustomerOrderCancellation>> ValidateCancellationOrder(
            Func<OrderRegistrationCode, Option<OrderRegistrationCode>> checkOrdersExistsByRegistrationCode)
        {
            return unvalidatedCancellationOrder =>
                ValidateCancellationOrder(checkOrdersExistsByRegistrationCode, unvalidatedCancellationOrder);
        }

        private static EitherAsync<string, ValidatedCustomerOrderCancellation> ValidateCancellationOrder(
            Func<OrderRegistrationCode, Option<OrderRegistrationCode>> checkOrderExists,
            UnvalidatedCustomerOrderCancellation unvalidatedCancellationOrder)
        {
            return from registrationCode in OrderRegistrationCode.TryParseRegistrationCode(unvalidatedCancellationOrder.OrderRegistrationCode)
                .ToEitherAsync(() => $"Invalid Registration Code {unvalidatedCancellationOrder.OrderRegistrationCode}")
                   select new ValidatedCustomerOrderCancellation(registrationCode);
        }

        private static Either<string, List<ValidatedCustomerOrderCancellation>> CreateEmptyValidatedCancellationOrdersList() =>
            Right(new List<ValidatedCustomerOrderCancellation>());

        private static EitherAsync<string, List<ValidatedCustomerOrderCancellation>> ReduceValidCancellationOrders(
            EitherAsync<string, List<ValidatedCustomerOrderCancellation>> acc,
            EitherAsync<string, ValidatedCustomerOrderCancellation> next)
        {
            return from list in acc
                   from nextOrder in next
                   select list.AppendValidCancellationOrder(nextOrder);
        }

        private static List<ValidatedCustomerOrderCancellation> AppendValidCancellationOrder(
            this List<ValidatedCustomerOrderCancellation> list,
            ValidatedCustomerOrderCancellation validCancellationOrder)
        {
            list.Add(validCancellationOrder);
            return list;
        }

    }
}
