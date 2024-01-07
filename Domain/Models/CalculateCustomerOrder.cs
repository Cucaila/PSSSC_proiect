using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public record CalculateCustomerOrder(OrderRegistrationCode OrderRegistrationCode, OrderDescription OrderDescription, OrderAmount OrderAmount, OrderAddress OrderAddress, OrderPrice OrderPrice, OrderPrice FinalPrice)
    {
        public int OrderHeaderId { get; set; }
        public int OrderLineId { get; set; }
        public bool IsUpdated { get; set; }
    }
}
