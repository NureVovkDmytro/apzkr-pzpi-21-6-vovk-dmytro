using Discerniy.Domain.Interface.Services;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Discerniy.Infrastructure.Services
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IModel channel;
        private readonly string exchangeName;
        private readonly string routingKey;
        private readonly IBasicProperties properties;

        public bool IsConnected => channel.IsOpen;

        public MessagePublisher(IModel model, string exchangeName, string queueName, IBasicProperties properties)
        {
            this.channel = model;
            this.exchangeName = exchangeName;
            this.routingKey = queueName;
            this.properties = properties;
        }

        public void Publish<T>(T body, IDictionary<string, string>? headers = default)
        {
            var message = JsonSerializer.Serialize(body);
            Publish(message, headers);
        }

        public void Publish(string message, IDictionary<string, string>? headers = default)
        {
            var body = Encoding.UTF8.GetBytes(message);
            Publish(body, headers);
        }

        public void Publish(byte[] body, IDictionary<string, string>? headers = default)
        {
            if (headers != null)
            {
                if(properties.Headers == null)
                {
                    properties.Headers = new Dictionary<string, object>();
                }
                foreach (var header in headers)
                {
                    properties.Headers[header.Key] = header.Value;
                }
            }
            channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: properties, body: body);
        }

        public void Dispose()
        {
            channel.Dispose();
        }
    }
}
