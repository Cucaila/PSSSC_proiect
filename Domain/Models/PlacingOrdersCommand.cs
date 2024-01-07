using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public record PlacingOrdersCommand
    {
        public PlacingOrdersCommand(IReadOnlyCollection<UnvalidatedCustomerOrder> inputOrder)
        {
            InputOrder = inputOrder;
        }
        public IReadOnlyCollection<UnvalidatedCustomerOrder> InputOrder { get; }
    }
}
