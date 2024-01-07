using System;
using System.Collections.Generic;
using static Domain.Models.OrdersCancellationCart;

namespace Domain.Models
{
    public record CancelOrderCommand
    {
        public CancelOrderCommand(IReadOnlyCollection<UnvalidatedOrdersCancellationCart> inputOrderCancellation)
        {
            InputOrderCancellation = inputOrderCancellation;
        }

        public IReadOnlyCollection<UnvalidatedOrdersCancellationCart> InputOrderCancellation { get; }
    }

}

