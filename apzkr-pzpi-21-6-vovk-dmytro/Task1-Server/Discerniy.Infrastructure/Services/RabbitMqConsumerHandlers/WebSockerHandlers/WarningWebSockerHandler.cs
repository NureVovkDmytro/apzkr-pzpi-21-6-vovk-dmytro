using Discerniy.Domain.Interface.Commands;
using Discerniy.Domain.Responses;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Discerniy.Infrastructure.Services.RabbitMqConsumerHandlers.WebSockerHandlers
{
    public class WarningWebSockerHandler : IRabbitMqWebSocketHandler
    {
        public string FunctionName => "warning";

        private readonly IHubContext<UserConnectionHub> hubContext;

        public WarningWebSockerHandler(IHubContext<UserConnectionHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task Handle(object? model, BasicDeliverEventArgs args)
        {

            var properties = args.BasicProperties;
            var userId = Encoding.UTF8.GetString(properties.Headers["userId"] as byte[] ?? throw new ArgumentNullException("groups is null"));
            var message = Encoding.UTF8.GetString(args.Body.ToArray());

            var response = JsonSerializer.Deserialize<ErrorResponse>(message);

            if (response == null)
            {
                return;
            }

            await hubContext.Clients.Groups(userId).SendAsync(FunctionName, response);
        }
    }
}
