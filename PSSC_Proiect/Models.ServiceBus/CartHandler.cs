using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace PSSC_Proiect.Models.ServiceBus
{
    public class CartHandler
    {
        private readonly ILogger<CartHandler> _logger;

        public CartHandler(ILogger<CartHandler> log)
        {
            _logger = log;
        }

        [FunctionName("CartHandler")]
        public void Run([ServiceBusTrigger("PsscDb", "ac55ddc4-e61a-4d99-a4b5-a98de0b8d08f")] ServiceBusReceivedMessage mySbMsg)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");

            //Message needs to be firstly deserialized as a CloudEvent because that's how it was sent.
            //Examples.Event.ServiceBus => ServiceBusTopicEventSender => SendAsync

            var productsEvent = mySbMsg.Body.ToObjectFromJson<Azure.Messaging.CloudEvent>();
            //Afterwards, the body of the event can be deserialised to a GradesPublishedEvent.
            var products = productsEvent.Data.ToObjectFromJson<ProductsPublishedEvent>();

            _logger.LogInformation($"Received message:");
            _logger.LogInformation(products.ToString());
        }
    }
}
