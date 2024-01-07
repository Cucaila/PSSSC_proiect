using LanguageExt;

namespace PSSC_Proiect.Models.ServiceBus
{
    public interface IEventSender
    {
        TryAsync<Unit> SendAsync<T>(string topicName, T @event);
    }
}
