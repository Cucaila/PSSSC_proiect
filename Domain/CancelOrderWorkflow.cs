using Domain.Models;
using Domain.Repositories;
using LanguageExt;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Domain.Models.CancelOrderEvent;
using static Domain.Models.OrdersCancellation;
using static LanguageExt.Prelude;

namespace Domain
{
    public class CancelOrderWorkflow
    {
        private readonly IOrderHeaderRepository orderHeaderRepository;
        private readonly IOrderLineRepository orderLineRepository;
        private readonly ILogger<CancelOrderWorkflow> logger;

        public CancelOrderWorkflow(IOrderHeaderRepository orderHeaderRepository, IOrderLineRepository orderLineRepository, ILogger<CancelOrderWorkflow> logger)
        {
            this.orderHeaderRepository = orderHeaderRepository;
            this.orderLineRepository = orderLineRepository;
            this.logger = logger;
        }

        public async Task<ICancelOrderEvent> ExecuteAsync(CancelOrderCommand command)
        {
            UnvalidatedOrdersCancellation unvalidatedCancellation = new UnvalidatedOrdersCancellation(command.InputOrderCancellation);


            var result = from existingOrder in orderLineRepository.TryGetExistingOrders()
                          .ToEither(ex => new FailedCancellation(ex.ToString()) as IOrdersCancellation)
                         let checkOrdersExists = (Func<OrderRegistrationCode, Option<OrderRegistrationCode>>)(order => CheckOrderExists(existingOrder, order))
                         from canceledOrders in ExecuteWorkflowAsync(unvalidatedCancellation, existingOrder, checkOrdersExists).ToAsync()
                         from _ in orderLineRepository.TryCancelOrders(canceledOrders)
                         .ToEither(ex => new FailedCancellation(ex.ToString()) as IOrdersCancellation)
                         select canceledOrders;

            return await result.Match(
                Left: ordersCancellation => GenerateFailedEvent(ordersCancellation) as ICancelOrderEvent,
                Right: canceledOrders => new CancelOrderSucceededEvent(canceledOrders.NumberOfOrder, canceledOrders.CancellationDate)
            );
        }

        private async Task<Either<IOrdersCancellation, CanceledOrder>> ExecuteWorkflowAsync(UnvalidatedOrdersCancellation unvalidatedCancellation,
                                                                                            IEnumerable<CalculateCustomerOrder> existingOrder,
                                                                                            Func<OrderRegistrationCode, Option<OrderRegistrationCode>> checkOrderExist)
        {
            IOrdersCancellation cancellation = await ValidatedOrdersCancellationOP(checkOrderExist, unvalidatedCancellation);
            cancellation = CancelOrder(cancellation);

            return cancellation.Match<Either<IOrdersCancellation, CanceledOrder>>(
                whenUnvalidatedOrdersCancellation: unvalidatedOrdersCancellation => Left(unvalidatedOrdersCancellation as IOrdersCancellation),
                whenInvalidatedOrdersCancellation: InvalidatedCustomerOrderCancellation => Left(InvalidatedCustomerOrderCancellation as IOrdersCancellation),
                whenValidatedOrdersCancellation: validatedOrdersCancellation => Left(validatedOrdersCancellation as IOrdersCancellation),
                whenCanceledOrder: canceledOrder => Right(canceledOrder),
                whenFailedCancellation: failed => Left(failed as IOrdersCancellation)
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

        private CancelOrderFailedEvent GenerateFailedEvent(IOrdersCancellation cancellation) =>
            cancellation.Match<CancelOrderFailedEvent>(
                whenUnvalidatedOrdersCancellation: unvalidatedOrdersCancellation => new($"Invalid state {nameof(UnvalidatedCustomerOrderCancellation)}"),
                whenInvalidatedOrdersCancellation: invalidatedCustomerOrderCancellation => new(invalidatedCustomerOrderCancellation.Reason),
                whenValidatedOrdersCancellation: validatedOrdersCancellation => new($"Invalid state {nameof(validatedOrdersCancellation)}"),
                whenFailedCancellation: failed => new($"Invalid state {nameof(FailedCancellation)}"),
                whenCanceledOrder: canceledOrder => new($"Invalid state {nameof(CanceledOrder)}")
            );
    }
}

