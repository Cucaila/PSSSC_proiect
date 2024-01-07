using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    namespace Domain.Models
    {
        public record CalculateCustomerOrderCancellation(
            OrderRegistrationCode OrderRegistrationCode, // Codul unic de înregistrare al comenzii pentru anulare
            OrderDescription OrderDescription, // Descrierea comenzii pentru anulare
            OrderAmount OrderAmount, // Suma inițială a comenzii înainte de anulare
            OrderAddress OrderAddress, // Adresa de livrare a comenzii inițiale
            OrderPrice CancellationFee, // Orice taxe asociate cu anularea
            OrderPrice RefundAmount // Suma rambursată clientului după anulare
        )
        {
            public int OrderHeaderId { get; set; }
            public int OrderLineId { get; set; }
            public bool IsCancellationConfirmed { get; set; }
            public DateTime CancellationRequestedDate { get; set; }
        }
    }

}
