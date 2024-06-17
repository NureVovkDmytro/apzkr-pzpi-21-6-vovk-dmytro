using Discerniy.Domain.Entity.Options;
using Discerniy.Domain.Interface.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Discerniy.Infrastructure.Services.RabbitMqConsumerHandlers
{
    public class RabbitMqWebSocketHandler : IConsumerHandler
    {
        public string QueueName { get; }

        protected readonly IServiceProvider serviceProvider;
        protected readonly ILogger<RabbitMqWebSocketHandler> logger;

        protected IDictionary<string, IRabbitMqWebSocketHandler> handlers = new Dictionary<string, IRabbitMqWebSocketHandler>();

        public RabbitMqWebSocketHandler(RabbitMqOptions options, IServiceProvider serviceProvider, ILogger<RabbitMqWebSocketHandler> logger)
        {
            QueueName = options.Queues.WebSocket;
            this.serviceProvider = serviceProvider;
            this.logger = logger;

            RegisterHandlers();
        }

        public async void Handle(object? model, BasicDeliverEventArgs args)
        {
            try
            {
                IBasicProperties properties = args.BasicProperties;

                string functionName = Encoding.UTF8.GetString(properties.Headers["function"] as byte[] ?? throw new ArgumentNullException("function is null"));

                if (!handlers.ContainsKey(functionName))
                {
                    return;
                }
                await handlers[functionName].Handle(model, args);
            }catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while handling RabbitMQ message");
            }
        }

        private void RegisterHandlers()
        {
            var services = serviceProvider.GetServices<IRabbitMqWebSocketHandler>();
            foreach (var service in services)
            {
                handlers.Add(service.FunctionName, service);
            }
        }
    }
}
