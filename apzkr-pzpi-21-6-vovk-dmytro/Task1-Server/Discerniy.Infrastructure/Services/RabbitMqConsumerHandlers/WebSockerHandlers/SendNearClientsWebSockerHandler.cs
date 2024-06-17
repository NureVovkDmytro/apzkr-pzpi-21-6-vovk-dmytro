using Discerniy.Domain.Interface.Commands;
using Discerniy.Domain.Responses;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Discerniy.Infrastructure.Services.RabbitMqConsumerHandlers.WebSockerHandlers
{
    public class SendNearClientsWebSockerHandler : IRabbitMqWebSocketHandler
    {
        public string FunctionName => "NearClients";

        private readonly IHubContext<UserConnectionHub> hubContext;

        public SendNearClientsWebSockerHandler(IHubContext<UserConnectionHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task Handle(object? model, BasicDeliverEventArgs args)
        {
            var properties = args.BasicProperties;
            var userId = Encoding.UTF8.GetString(properties.Headers["userId"] as byte[] ?? throw new ArgumentNullException("groups is null"));
            var message = Encoding.UTF8.GetString(args.Body.ToArray());

            var nearClients = JsonSerializer.Deserialize<List<LocationResponse>>(message);

            if (nearClients == null)
            {
                return;
            }

            await hubContext.Clients.Groups(userId).SendAsync(FunctionName, nearClients);
        }
    }
}
