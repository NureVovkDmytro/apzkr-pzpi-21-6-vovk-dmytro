using Discerniy.Domain.Entity.Options;
using Discerniy.Domain.Interface.Services;

namespace Discerniy.Infrastructure.Services
{
    public class WebSocketMessagePublisher : IWebSocketMessagePublisher
    {
        private readonly IMessagePublisher publisher;

        public bool IsConnected => publisher.IsConnected;

        public WebSocketMessagePublisher(IMessagePublisherFactory factory, RabbitMqOptions rabbitMqOptions)
        {
            publisher = factory.CreateMessagePublisher(rabbitMqOptions.Queues.WebSocket);
        }

        public void Publish<T>(T body, IDictionary<string, string>? headers = default)
        {
            publisher.Publish(body, headers);
        }

        public void Publish(string message, IDictionary<string, string>? headers = default)
        {
            publisher.Publish(message, headers);
        }

        public void Publish(byte[] body, IDictionary<string, string>? headers = default)
        {
            publisher.Publish(body, headers);
        }

        public void Dispose()
        {
            publisher.Dispose();
        }
    }
}
