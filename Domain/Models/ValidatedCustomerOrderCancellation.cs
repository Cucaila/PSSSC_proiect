using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public record ValidatedCustomerOrderCancellation
    {
        public ValidatedCustomerOrderCancellation(OrderRegistrationCode orderRegistrationCode)
        {
            OrderRegistrationCode = orderRegistrationCode;
        }

        public OrderRegistrationCode OrderRegistrationCode { get; }
    }
}
