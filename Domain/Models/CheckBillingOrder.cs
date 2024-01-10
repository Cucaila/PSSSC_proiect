using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public record CheckBillingOrder(OrderRegistrationCode OrderRegistrationCode, IReadOnlyCollection<UnvalidatedBillingOrder> UnvalidatedCustomerOrdersList);
}
