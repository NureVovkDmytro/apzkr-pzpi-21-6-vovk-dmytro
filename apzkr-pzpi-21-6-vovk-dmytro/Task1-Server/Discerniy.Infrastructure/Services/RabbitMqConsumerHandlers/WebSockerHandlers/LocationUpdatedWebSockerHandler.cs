using Discerniy.Domain.Interface.Commands;
using Discerniy.Domain.Responses;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Discerniy.Infrastructure.Services.RabbitMqConsumerHandlers.WebSockerHandlers
{
    public class LocationUpdatedWebSockerHandler : IRabbitMqWebSocketHandler
    {
        protected readonly IHubContext<UserConnectionHub> hubContext;
        public string FunctionName => "LocationUpdated";

        public LocationUpdatedWebSockerHandler(IHubContext<UserConnectionHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task Handle(object? model, BasicDeliverEventArgs args)
        {
            IBasicProperties properties = args.BasicProperties;

            var message = Encoding.UTF8.GetString(args.Body.ToArray());
            LocationResponse locationResponse = JsonSerializer.Deserialize<LocationResponse>(message) ?? throw new ArgumentNullException("message is null");

            if (properties.Headers.ContainsKey("groups"))
            {
                string groups = Encoding.UTF8.GetString(properties.Headers["groups"] as byte[] ?? throw new ArgumentNullException("groups is null"));
                IReadOnlyList<string> splitedGroups = groups.Split(',');
                if (splitedGroups.Count > 0)
                {
                    await hubContext.Clients.Groups(splitedGroups).SendAsync(FunctionName, locationResponse);
                }
            }

            if (properties.Headers.ContainsKey("userId"))
            {
                string userId = Encoding.UTF8.GetString(properties.Headers["userId"] as byte[] ?? throw new ArgumentNullException("userId is null"));
                await hubContext.Clients.Groups(userId).SendAsync(FunctionName, locationResponse);
            }
        }
    }
}
