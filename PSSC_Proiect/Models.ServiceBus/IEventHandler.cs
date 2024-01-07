using System.Threading.Tasks;
using CloudNative.CloudEvents;

namespace PSSC_Proiect.Models.ServiceBus
{
    public interface IEventHandler
    {
        string[] EventTypes { get; }

        Task<EventProcessingResult> HandleAsync(CloudEvent cloudEvent);
    }
}
