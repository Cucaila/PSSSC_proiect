using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public record CancelOrderCommand
    {
        public CancelOrderCommand(IReadOnlyCollection<UnvalidatedCustomerOrder> inputOrder)
        {
            InputOrder = inputOrder;
        }

        public IReadOnlyCollection<UnvalidatedCustomerOrder> InputOrder { get; }
    }

}

