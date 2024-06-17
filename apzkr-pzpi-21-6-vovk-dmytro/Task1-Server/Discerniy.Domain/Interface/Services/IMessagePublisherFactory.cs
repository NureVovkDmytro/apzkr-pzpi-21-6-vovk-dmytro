namespace Discerniy.Domain.Interface.Services
{
    public interface IMessagePublisherFactory
    {
        IMessagePublisher CreateMessagePublisher(string exchangeName, string exchangeType, string routingKey, IDictionary<string, string>? headers = default);
        IMessagePublisher CreateMessagePublisher(string routingKey, IDictionary<string, string>? headers = default);
    }
}
