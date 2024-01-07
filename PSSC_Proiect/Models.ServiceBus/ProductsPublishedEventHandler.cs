using System.Threading.Tasks;
using System;

namespace PSSC_Proiect.Models.ServiceBus
{
    public class ProductsPublishedEventHandler : AbstractEventHandler<ProductsPublishedEvent>
    {
        public override string[] EventTypes => new string[] { typeof(ProductsPublishedEvent).Name };

        protected override Task<EventProcessingResult> OnHandleAsync(ProductsPublishedEvent eventData)
        {
            Console.WriteLine(eventData.ToString());
            return Task.FromResult(EventProcessingResult.Completed);
        }
    }
   
}
