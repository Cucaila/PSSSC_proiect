using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public record UnvalidatedCustomerOrderCancellation
    {
        public UnvalidatedCustomerOrderCancellation(string orderRegistrationCode)
        {
            OrderRegistrationCode = orderRegistrationCode;
        }

        public string OrderRegistrationCode { get; }
    }
}
