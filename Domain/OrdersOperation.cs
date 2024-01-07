using Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using static Domain.Models.OrdersCart;

namespace Domain
{
    public static class OrdersOperation
    {
        public static Task<IOrdersCart> ValidatedOrdersCartOP(Func<OrderRegistrationCode, Option<OrderRegistrationCode>> checkOrderExist, UnvalidatedOrdersCart orders) =>
                orders.OrderList
                      .Select(ValidateCustomerOrder(checkOrderExist))
                      .Aggregate(CreateEmptyValidatedOrdersList().ToAsync(), ReduceValidOrders)
                      .MatchAsync(
                            Right: validatedOrders => new ValidatedOrdersCart(validatedOrders),
                            LeftAsync: errorMessage => Task.FromResult((IOrdersCart)new InvalidatedOrdersCart(orders.OrderList, errorMessage))
                       );

        private static Func<UnvalidatedCustomerOrder, EitherAsync<string, ValidatedCustomerOrder>> ValidateCustomerOrder(Func<OrderRegistrationCode, Option<OrderRegistrationCode>> checkOrdersExistsByRegsitrationCode) =>
                unvalidatedCustomerOrder => ValidateCustomerOrder(checkOrdersExistsByRegsitrationCode, unvalidatedCustomerOrder);

        private static EitherAsync<string, ValidatedCustomerOrder> ValidateCustomerOrder(Func<OrderRegistrationCode, Option<OrderRegistrationCode>> checkOrderExists, UnvalidatedCustomerOrder unvalidatedCustomerOrder) =>
               from registrationCode in OrderRegistrationCode.TryParseRegistrationCode(unvalidatedCustomerOrder.OrderRegistrationCode)
                                                                .ToEitherAsync(() => $"Invalid Registration Code {unvalidatedCustomerOrder.OrderRegistrationCode }")
               from description in OrderDescription.TryParseOrderDescription(unvalidatedCustomerOrder.OrderDescription)
                                                                .ToEitherAsync(() => $"Invalid Description  ({unvalidatedCustomerOrder.OrderRegistrationCode } , {unvalidatedCustomerOrder.OrderDescription} )")
               from amount in OrderAmount.TryParseOrderAmount(unvalidatedCustomerOrder.OrderAmount)
                                                                .ToEitherAsync(() => $"Invalid Amount ( {unvalidatedCustomerOrder.OrderRegistrationCode } , {unvalidatedCustomerOrder.OrderAmount} )")
               from address in OrderAddress.TryParseOrderAddress(unvalidatedCustomerOrder.OrderAddress)
                                                                .ToEitherAsync(() => $"Invalid Address ( {unvalidatedCustomerOrder.OrderRegistrationCode }, {unvalidatedCustomerOrder.OrderAddress} )")
               from price in OrderPrice.TryParsePrice(unvalidatedCustomerOrder.OrderPrice)
                                                                .ToEitherAsync(() => $"Invalid Price ( {unvalidatedCustomerOrder.OrderRegistrationCode }, { unvalidatedCustomerOrder.OrderPrice} )")
               select new ValidatedCustomerOrder(registrationCode, description, amount, address, price);

        private static Either<string, List<ValidatedCustomerOrder>> CreateEmptyValidatedOrdersList() =>
            Right(new List<ValidatedCustomerOrder>());

        private static EitherAsync<string, List<ValidatedCustomerOrder>> ReduceValidOrders(EitherAsync<string, List<ValidatedCustomerOrder>> acc, EitherAsync<string, ValidatedCustomerOrder> next) =>
                from list in acc
                from nextOrder in next
                select list.AppendValidOrder(nextOrder);

        private static List<ValidatedCustomerOrder> AppendValidOrder(this List<ValidatedCustomerOrder> list, ValidatedCustomerOrder validOrder)
        {
            list.Add(validOrder);
            return list;
        }
        public static IOrdersCart CalculatePriceOfCart(IOrdersCart orders) => orders.Match(
             whenUnvalidatedOrdersCart: unvalidatedCustomerOrder => unvalidatedCustomerOrder,
             whenInvalidatedOrdersCart: invalidatedCustomerOrder => invalidatedCustomerOrder,
             whenFailedCart: failedCart => failedCart,
             whenCalculatedOrder: calculatedOrder => calculatedOrder,
             whenPlacedOrder: placedOrder => placedOrder,
             whenCheckedOrderByCode: checkedOrderByCode => checkedOrderByCode,
             whenValidatedOrdersCart: CalculateFinalPrice

          );

        private static IOrdersCart CalculateFinalPrice(ValidatedOrdersCart validOrders) =>
           new CalculatedOrder(validOrders.OrdersList
                                                   .Select(CalculateOrderFinalPrice)
                                                   .ToList()
                                                   .AsReadOnly());

        private static CalculateCustomerOrder CalculateOrderFinalPrice(ValidatedCustomerOrder validOrder) =>
            new CalculateCustomerOrder(validOrder.OrderRegistrationCode,
                                      validOrder.OrderDescription,
                                      validOrder.OrderAmount,
                                      validOrder.OrderAddress,
                                      validOrder.OrderPrice,
                                      validOrder.OrderPrice * validOrder.OrderAmount);
        public static IOrdersCart MergeOrders(IOrdersCart orders, IEnumerable<CalculateCustomerOrder> existingOrders) => orders.Match(
                whenUnvalidatedOrdersCart: unvalidatedCustomerOrder => unvalidatedCustomerOrder,
                whenInvalidatedOrdersCart: invalidatedCustomerOrder => invalidatedCustomerOrder,
                whenFailedCart: failedCart => failedCart,
                whenPlacedOrder: placedOrder => placedOrder,
                whenCheckedOrderByCode: checkedOrderByCode => checkedOrderByCode,
                whenValidatedOrdersCart: unvalidatedOrdersCart => unvalidatedOrdersCart,
                whenCalculatedOrder: calculatedOrder => MergeOrders1(calculatedOrder.OrdersList, existingOrders)
             );

        private static CalculatedOrder MergeOrders1(IEnumerable<CalculateCustomerOrder> newList, IEnumerable<CalculateCustomerOrder> existingList)
        {
            var updatedNewOrders = newList.Select(sp => sp with { OrderHeaderId = existingList.FirstOrDefault(s => s.OrderRegistrationCode == sp.OrderRegistrationCode)?.OrderHeaderId ?? 0, IsUpdated = true });
            var oldList = existingList.Where(sp => !newList.Any(s => s.OrderRegistrationCode == sp.OrderRegistrationCode));
            var allShoppingCarts = updatedNewOrders.Union(oldList)
                                               .ToList()
                                               .AsReadOnly();
            return new CalculatedOrder(allShoppingCarts);
        }


        public static IOrdersCart PlacedOrder(IOrdersCart orders) => orders.Match(
             whenUnvalidatedOrdersCart: unvalidatedCustomerOrder => unvalidatedCustomerOrder,
             whenInvalidatedOrdersCart: invalidatedCustomerOrder => invalidatedCustomerOrder,
             whenPlacedOrder: placedOrder => placedOrder,
             whenFailedCart: failedCart => failedCart,
             whenCheckedOrderByCode: checkedOrderByCode => checkedOrderByCode,
             whenValidatedOrdersCart: validatedOrdersCart => validatedOrdersCart,
             whenCalculatedOrder: GenerateExportOfOrder
            );

        private static IOrdersCart GenerateExportOfOrder(CalculatedOrder calculatedOrder)
        {
            Random random = new Random();
            int numberOfOrder = random.Next(1000, 9999);

            return new PlacedOrder(calculatedOrder.OrdersList,
                                   calculatedOrder.OrdersList.Aggregate(new StringBuilder(), CreateCSVLine).ToString(),
                                   numberOfOrder,
                                   DateTime.Now);
        }

        private static StringBuilder CreateCSVLine(StringBuilder export, CalculateCustomerOrder order) =>
            export.AppendLine($"Code: {order.OrderRegistrationCode.Value} : {order.OrderDescription.Description} -- Amount: {order.OrderAmount.Amount} -- Address: {order.OrderAddress.Address} : {order.OrderPrice.Price} per Order -- {order.FinalPrice.Price} LEI");

    }
}
