using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{

    public record ShippingOrderCommand
    {
        public ShippingOrderCommand(IReadOnlyCollection<ValidatedCustomerOrder> inputOrders)
        {
            InputOrders = inputOrders;
        }

        public IReadOnlyCollection<ValidatedCustomerOrder> InputOrders { get; }
    }
}
