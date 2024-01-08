using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public record BillingOrderCommand
    {
        public BillingOrderCommand(IReadOnlyCollection<ValidatedCustomerOrder> inputOrders)
        {
            InputOrders = inputOrders;
        }

        public IReadOnlyCollection<ValidatedCustomerOrder> InputOrders { get; }
    }

}
