namespace Discerniy.Domain.Interface.Services
{
    public interface IMessagePublisher : IDisposable
    {
        bool IsConnected { get; }

        void Publish<T>(T body, IDictionary<string, string>? headers = default);
        void Publish(byte[] body, IDictionary<string, string>? headers = default);
        void Publish(string body, IDictionary<string, string>? headers = default);
    }
}
