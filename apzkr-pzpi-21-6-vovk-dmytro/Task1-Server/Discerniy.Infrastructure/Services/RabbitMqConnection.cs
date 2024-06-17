using Discerniy.Domain.Interface.Services;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;

namespace Discerniy.Infrastructure.Services
{
    public class RabbitMqConnection : IRabbitMqConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection = default!;
        private bool _disposed = false;
        private readonly ILogger _logger;

        public RabbitMqConnection(IConnectionFactory connectionFactory, ILogger<RabbitMqConnection> logger)
        {
            _logger = logger;
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            if (!IsConnected)
            {
                TryConnect();
            }
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex, "Error while disposing RabbitMQ connection");
            }
            _disposed = true;
        }

        public bool TryConnect()
        {
            try
            {
                _connection = _connectionFactory.CreateConnection();
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogCritical(ex, "Error while connecting to RabbitMQ");
            }

            return IsConnected;
        }
    }
}
