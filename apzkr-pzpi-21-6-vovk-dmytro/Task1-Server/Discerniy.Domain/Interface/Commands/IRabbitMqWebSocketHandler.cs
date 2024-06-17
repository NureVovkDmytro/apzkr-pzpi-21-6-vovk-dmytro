using RabbitMQ.Client.Events;

namespace Discerniy.Domain.Interface.Commands
{
    public interface IRabbitMqWebSocketHandler
    {
        string FunctionName { get; }
        Task Handle(object? model, BasicDeliverEventArgs args);
    }
}
