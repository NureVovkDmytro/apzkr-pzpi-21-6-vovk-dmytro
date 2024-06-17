using Discerniy.Domain.Entity.RabbitMqModels;
using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Interface.Commands;
using Discerniy.Domain.Interface.Services;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Discerniy.Infrastructure.Services.RabbitMqConsumerHandlers.WebSockerHandlers
{
    public class UpdateUserUpdateLocationIntervalWebSockerHandler : IRabbitMqWebSocketHandler
    {
        protected readonly IHubContext<UserConnectionHub> hubContext;
        protected readonly IDeviceWebSocketManager deviceWebSocketManager;
        public string FunctionName => "updateUserUpdateLocationInterval";

        public UpdateUserUpdateLocationIntervalWebSockerHandler(IHubContext<UserConnectionHub> hubContext, IDeviceWebSocketManager deviceWebSocketManager)
        {
            this.hubContext = hubContext;
            this.deviceWebSocketManager = deviceWebSocketManager;
        }

        public async Task Handle(object? model, BasicDeliverEventArgs args)
        {
            IBasicProperties properties = args.BasicProperties;

            var message = Encoding.UTF8.GetString(args.Body.ToArray());
            UpdateUserInterval locationResponse = JsonSerializer.Deserialize<UpdateUserInterval>(message) ?? throw new ArgumentNullException("message is null");

            await this.hubContext.Clients.Groups(locationResponse.UserId).SendAsync(FunctionName, locationResponse.LocationSecondsInterval);

            var command = new DeviceCommand<UpdateUserInterval>(FunctionName, locationResponse);

            await deviceWebSocketManager.SendToGroup(locationResponse.UserId, JsonSerializer.Serialize(command));
        }
    }
}
