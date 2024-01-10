using CSharp.Choices;
using System;
using System.Collections.Generic;

namespace Domain.Models
{

            public record BilledOrderLine(OrderRegistrationCode OrderRegistrationCode, OrderDescription OrderDescription, OrderAmount OrderAmount, OrderAddress OrderAddress, OrderPrice OrderPrice)
            {
            public int OrderHeaderId { get; set; }
             public int OrderLineId { get; set; }
             public bool IsUpdated { get; set; }
            }
}

