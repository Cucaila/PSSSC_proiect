using Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
using static Domain.Models.BillingCart;

namespace Domain
{
    public static class BillingOperations
    {
        public static Task<IBillingCart> ValidateBillingCartAsync(Func<OrderRegistrationCode, Option<OrderRegistrationCode>> checkInvoiceExist, UnbilledOrdersCart cart) =>
            cart.OrdersList
                .Select(ValidateBillingLine(checkInvoiceExist))
                .Aggregate(CreateEmptyValidatedBillingCart().ToAsync(), ReduceValidBilling)
                .MatchAsync(
                    Right: validatedLines => new ValidatedBilledOrdersCart(validatedLines),
                    LeftAsync: errorMessage => Task.FromResult((IBillingCart)new BillingPreparationCart(cart.OrdersList, errorMessage))
         );

        private static Func<UnvalidatedBillingOrder, EitherAsync<string, ValidatedBillingLine>> ValidateBillingLine(Func<OrderRegistrationCode, Option<OrderRegistrationCode>> checkInvoiceExist) =>
            unvalidatedLine => ValidateBillingLine(checkInvoiceExist, unvalidatedLine);

        private static EitherAsync<string, ValidatedBillingLine> ValidateBillingLine(Func<OrderRegistrationCode, Option<OrderRegistrationCode>> checkInvoiceExist, UnvalidatedBillingOrder line) =>
     from registrationCode in OrderRegistrationCode.TryParseRegistrationCode(line.OrderRegistrationCode)
                                                                .ToEitherAsync(() => $"Invalid Registration Code {line.OrderRegistrationCode }")
     from description in OrderDescription.TryParseOrderDescription(line.OrderDescription)
                                                      .ToEitherAsync(() => $"Invalid Description  ({line.OrderRegistrationCode } , {line.OrderDescription} )")
     from amount in OrderAmount.TryParseOrderAmount(line.OrderAmount)
                                                      .ToEitherAsync(() => $"Invalid Amount ( {line.OrderRegistrationCode } , {line.OrderAmount} )")
     from address in OrderAddress.TryParseOrderAddress(line.OrderAddress)
                                                      .ToEitherAsync(() => $"Invalid Address ( {line.OrderRegistrationCode }, {line.OrderAddress} )")
     from price in OrderPrice.TryParsePrice(line.OrderPrice)
                                                      .ToEitherAsync(() => $"Invalid Price ( {line.OrderRegistrationCode }, { line.OrderPrice} )")
     select new ValidatedBillingLine(registrationCode, description, amount, address, price
    );

        private static Either<string, List<ValidatedBillingLine>> CreateEmptyValidatedBillingCart() =>
            Right(new List<ValidatedBillingLine>());

        private static EitherAsync<string, List<ValidatedBillingLine>> ReduceValidBilling(EitherAsync<string, List<ValidatedBillingLine>> acc, EitherAsync<string, ValidatedBillingLine> next) =>
            from list in acc
            from nextLine in next
            select list.AppendValidBillingLine(nextLine);

        private static List<ValidatedBillingLine> AppendValidBillingLine(this List<ValidatedBillingLine> list, ValidatedBillingLine line)
        {
            list.Add(line);
            return list;
        }

        public static IBillingCart CalculateBillingAmount(IBillingCart billing) => billing.Match(
             whenUnbilledOrdersCart: unbilledOrdersCart => unbilledOrdersCart,
             whenBillingPreparationCart: billinPreparationCart => billinPreparationCart,
             whenFailedBillingCart: failedBillingCart => failedBillingCart,
             whenBilledOrdersCart: billedOrder => billedOrder,
             whenCalculatedBillingOrder: calculatedBillingOrder => calculatedBillingOrder,
             whenCheckedBillingOrder: checkedBillingOrderByCode => checkedBillingOrderByCode,
             whenValidatedBilledOrdersCart: CalculateTotalAmount

          );

        private static IBillingCart CalculateTotalAmount(ValidatedBilledOrdersCart validatedCart) =>
            new CalculatedBillingOrder(validatedCart.OrdersList
                                                      .Select(CalculateLineTotal)
                                                      .ToList()
                                                      .AsReadOnly());

        private static BilledOrderLine CalculateLineTotal(ValidatedBillingLine validOrder) =>
           new BilledOrderLine(validOrder.OrderRegistrationCode,
                                      validOrder.OrderDescription,
                                      validOrder.OrderAmount,
                                      validOrder.OrderAddress,
                                      validOrder.OrderPrice * validOrder.OrderAmount
            );
            
    }
}
