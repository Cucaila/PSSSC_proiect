using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public record BillingOrderCommand
    {
        public BillingOrderCommand(IReadOnlyCollection<UnvalidatedBillingOrder> inputOrders)
        {
            InputOrders = inputOrders;
        }

        public IReadOnlyCollection<UnvalidatedBillingOrder> InputOrders { get; }
    }

}
