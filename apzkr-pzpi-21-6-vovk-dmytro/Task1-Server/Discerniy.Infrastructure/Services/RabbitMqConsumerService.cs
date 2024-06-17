using Discerniy.Domain.Entity.Options;
using Discerniy.Domain.Interface.Commands;
using Discerniy.Domain.Interface.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Discerniy.Infrastructure.Services
{
    public class RabbitMqConsumerService : IHostedService
    {
        protected readonly IRabbitMqConnection connection;
        protected readonly RabbitMqOptions options;
        protected readonly IServiceProvider serviceProvider;

        private readonly Dictionary<string, RabbitMqConsumerHandlerRecord> handlers = new Dictionary<string, RabbitMqConsumerHandlerRecord>();

        public RabbitMqConsumerService(IRabbitMqConnection connection, RabbitMqOptions options, IServiceProvider serviceProvider)
        {
            this.connection = connection;
            this.options = options;
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!connection.IsConnected)
            {
                connection.TryConnect();
            }

            RegisterHandlers();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var handler in handlers)
            {
                handler.Value.Dispose();
            }

            handlers.Clear();
            return Task.CompletedTask;
        }

        private void RegisterHandlers()
        {
            var services = serviceProvider.GetServices<IConsumerHandler>();
            foreach (var service in services)
            {
                var channel = connection.CreateModel();
                handlers.Add(service.QueueName, new RabbitMqConsumerHandlerRecord(service, channel));
            }
        }
    }
}
