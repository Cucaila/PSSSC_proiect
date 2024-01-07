using Data.Models;
using System.Collections.Generic;

namespace PSSC_Proiect.Models.ServiceBus
{
    public class ProductsPublishedEvent
    {
        public List<ProductDbo> Products { get; init; }
    }
}
