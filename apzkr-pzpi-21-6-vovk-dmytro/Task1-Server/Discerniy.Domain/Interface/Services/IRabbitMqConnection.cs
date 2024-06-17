using RabbitMQ.Client;

namespace Discerniy.Domain.Interface.Services
{
    public interface IRabbitMqConnection : IDisposable
    {
        IModel CreateModel();
        bool IsConnected { get; }
        bool TryConnect();
    }
}
