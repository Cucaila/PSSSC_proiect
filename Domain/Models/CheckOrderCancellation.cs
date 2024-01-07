using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    //public record CheckOrderCancellation(OrderRegistrationCode OrderRegistrationCode, IReadOnlyCollection<UnvalidatedCustomerOrder> UnvalidatedCustomerOrdersList);
    namespace Domain.Models
    {
        public class CheckOrderCancellation
        {
            public OrderRegistrationCode OrderRegistrationCode { get; private set; }
            public DateTime OrderPlacedDate { get; private set; }
            public DateTime? OrderShippedDate { get; private set; }
            public bool IsCancellationRequestReceived { get; private set; }

            public CheckOrderCancellation(OrderRegistrationCode orderRegistrationCode, DateTime orderPlacedDate, DateTime? orderShippedDate, bool isCancellationRequestReceived)
            {
                OrderRegistrationCode = orderRegistrationCode;
                OrderPlacedDate = orderPlacedDate;
                OrderShippedDate = orderShippedDate;
                IsCancellationRequestReceived = isCancellationRequestReceived;
            }

            public bool CanBeCancelled()
            {
                if (OrderShippedDate == null)
                {
                    return true;
                }

                return false;
            }
        }
    }

}

