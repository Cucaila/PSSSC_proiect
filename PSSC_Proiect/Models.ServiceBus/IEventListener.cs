using System.Threading.Tasks;
using System.Threading;

namespace PSSC_Proiect.Models.ServiceBus
{
    public interface IEventListener
    {
        Task StartAsync(string topicName, string subscriptionName, CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }
}
