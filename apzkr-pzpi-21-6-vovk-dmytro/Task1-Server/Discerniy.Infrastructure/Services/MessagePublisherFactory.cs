using Discerniy.Domain.Interface.Services;
using RabbitMQ.Client;

namespace Discerniy.Infrastructure.Services
{
    public class MessagePublisherFactory : IMessagePublisherFactory
    {
        private readonly IRabbitMqConnection connection;

        public MessagePublisherFactory(IRabbitMqConnection connection)
        {
            this.connection = connection;
        }

        public IMessagePublisher CreateMessagePublisher(string exchangeName, string exchangeType, string routingKey, IDictionary<string, string>? headers = null)
        {
            if(string.IsNullOrEmpty(routingKey))
            {
                throw new ArgumentException("Routing key cannot be empty", nameof(routingKey));
            }

            if (!connection.IsConnected)
            {
                connection.TryConnect();
            }

            var channel = connection.CreateModel();
            if (!string.IsNullOrEmpty(exchangeName))
            {
                channel.ExchangeDeclare(exchangeName, exchangeType);
            }

            channel.QueueDeclare(queue: routingKey, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            if (headers != null) {
                foreach (var header in headers)
                {
                    properties.Headers.Add(header.Key, header.Value);
                }
            }

            return new MessagePublisher(channel, exchangeName, routingKey, properties);
        }

        public IMessagePublisher CreateMessagePublisher(string queueName, IDictionary<string, string>? headers = null)
        {
            return CreateMessagePublisher(string.Empty, string.Empty, queueName, headers);
        }
    }
}
