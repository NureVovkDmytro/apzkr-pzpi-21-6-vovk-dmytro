using RabbitMQ.Client.Events;

namespace Discerniy.Domain.Interface.Commands
{
    public interface IConsumerHandler
    {
        string QueueName { get; }
        void Handle(object? model, BasicDeliverEventArgs args);
    }
}
