using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Discerniy.Domain.Interface.Commands
{
    public class RabbitMqConsumerHandlerRecord : IDisposable
    {
        public string QueueName => Handler.QueueName;
        public IConsumerHandler Handler { get; set; }
        public IModel Channel { get; set; }
        public EventingBasicConsumer Consumer { get; }

        public RabbitMqConsumerHandlerRecord(IConsumerHandler handler, IModel channel)
        {
            Handler = handler;
            Channel = channel;
            Consumer = new EventingBasicConsumer(channel);

            Consumer.Received += Handler.Handle;
            Channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            Channel.BasicConsume(queue: QueueName, autoAck: true, consumer: Consumer);
        }

        public void Dispose()
        {
            Consumer.Received -= Handler.Handle;

            if (Channel.IsOpen)
            {
                Channel.Close();
            }
            Channel.Dispose();
        }
    }
}
