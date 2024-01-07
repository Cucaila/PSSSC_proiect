using Domain.Models;
using Domain.Repositories;
using LanguageExt;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Models.OrdersCart;
using static Domain.Models.PlacingOrderEvent;
using static LanguageExt.Prelude;
using static Domain.OrdersOperation;



namespace Domain
{
    public class PlacingOrderWorkflow
    {
        private readonly IOrderHeaderRepository orderHeaderRepository;
        private readonly IOrderLineRepository orderLineRepository;
        private readonly IProductRepository productRepository;
        private readonly ILogger<PlacingOrderWorkflow> logger;

        public PlacingOrderWorkflow(IOrderHeaderRepository orderHeaderRepository, IProductRepository productRepository, IOrderLineRepository orderLineRepository, ILogger<PlacingOrderWorkflow> logger)
        {
            this.orderHeaderRepository = orderHeaderRepository;
            this.productRepository = productRepository;
            this.orderLineRepository = orderLineRepository;
            this.logger = logger;
        }
        public async Task<IPlacingOrderEvent> ExecuteAsync(PlacingOrdersCommand command)
        {
            UnvalidatedOrdersCart unvalidatedOrders = new UnvalidatedOrdersCart(command.InputOrder);

            var result = from product in productRepository.TryGetExistingOrders(unvalidatedOrders.OrderList.Select(order => order.OrderRegistrationCode))
                                                        .ToEither(ex => new FailedCart("") as IOrdersCart)
                         from existingOrder in orderLineRepository.TryGetExistingOrders()
                                          .ToEither(ex => new FailedCart("") as IOrdersCart)
                         let checkOrdersExists = (Func<OrderRegistrationCode, Option<OrderRegistrationCode>>)(order => CheckOrderExists(product, order))
                         from placedOrders in ExecuteWorkFlowAsync(unvalidatedOrders, existingOrder, checkOrdersExists).ToAsync()
                         from _ in orderLineRepository.TrySaveOrders(placedOrders)
                                          .ToEither(ex => new FailedCart("") as IOrdersCart)
                         select placedOrders;

            return await result.Match(
                    Left: orders => GenerateFailedEvent(orders) as IPlacingOrderEvent,
                    Right: placedOrders => new PlacingOrderSuccedeedEvent(placedOrders.CalculateCustomerOrders, placedOrders.Csv, placedOrders.NumberOfOrder, placedOrders.PlacedDate)
                );
        }

        private async Task<Either<IOrdersCart, PlacedOrder>> ExecuteWorkFlowAsync(UnvalidatedOrdersCart unvalidatedOrder,
                                                                                  IEnumerable<CalculateCustomerOrder> existingOrder,
                                                                                  Func<OrderRegistrationCode, Option<OrderRegistrationCode>> checkOrderExist)
        {

            IOrdersCart orders = await ValidatedOrdersCartOP(checkOrderExist, unvalidatedOrder);
            orders = CalculatePriceOfCart(orders);
            orders = PlacedOrder(orders);

            return orders.Match<Either<IOrdersCart, PlacedOrder>>(
                whenUnvalidatedOrdersCart: unvalidatedOrdersCart => Left(unvalidatedOrdersCart as IOrdersCart),
                whenInvalidatedOrdersCart: InvalidatedCustomerOrder => Left(InvalidatedCustomerOrder as IOrdersCart),
                whenValidatedOrdersCart: validateOrdersCart => Left(validateOrdersCart as IOrdersCart),
                whenCalculatedOrder: calculateOrder => Left(calculateOrder as IOrdersCart),
                whenCheckedOrderByCode: checkedOrderByCode => Left(checkedOrderByCode as IOrdersCart),
                whenFailedCart: failed => Left(failed as IOrdersCart),
                whenPlacedOrder: placedOrder => Right(placedOrder)
                );

        }

        private Option<OrderRegistrationCode> CheckOrderExists(IEnumerable<OrderRegistrationCode> orders, OrderRegistrationCode orderRegistrationNumber)
        {
            if (orders.Any(s => s == orderRegistrationNumber))
            {
                return Some(orderRegistrationNumber);
            }
            else
            {
                return None;
            }
        }

        private PlacingOrderFailedEvent GenerateFailedEvent(IOrdersCart orders) =>
            orders.Match<PlacingOrderFailedEvent>(
                whenUnvalidatedOrdersCart: unvalidatedOrdersCart => new($"Invalid state {nameof(UnvalidatedCustomerOrder)}"),
                whenInvalidatedOrdersCart: invalidatedCustomerOrder => new(invalidatedCustomerOrder.Reason),
                whenValidatedOrdersCart: validatedCustomerOrder => new($"Invalid state {nameof(validatedCustomerOrder)}"),
                whenFailedCart: failed => new($"Invalid state {nameof(FailedCart)}"),
                whenCheckedOrderByCode: checkedOrderByCode => new($"Invalid state {nameof(CheckedOrderByCode)}"),
                whenCalculatedOrder: calculateCustomerOrder => new($"Invalid state {nameof(CalculateCustomerOrder)}"),
                whenPlacedOrder: placedOrder => new($"Invalid state {nameof(PlacedOrder)}")
             );
    }
}
