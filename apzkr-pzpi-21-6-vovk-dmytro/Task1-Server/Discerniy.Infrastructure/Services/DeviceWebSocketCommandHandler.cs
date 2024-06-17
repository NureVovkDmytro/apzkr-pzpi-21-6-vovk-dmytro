using Discerniy.Domain.Entity.SubEntity;
using Discerniy.Domain.Interface.Commands;
using Discerniy.Domain.Interface.Services;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Text.Json;

namespace Discerniy.Infrastructure.Services
{
    public class DeviceWebSocketCommandHandler : IDeviceWebSocketCommandHandler, IDisposable
    {
        private IDictionary<string, IDeviceWebSocketHandler> handlers = new Dictionary<string, IDeviceWebSocketHandler>();

        public DeviceWebSocketCommandHandler(IEnumerable<IDeviceWebSocketHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                this.handlers.Add(handler.Command, handler);
            }
        }

        public void Dispose()
        {
            this.handlers.Clear();
        }

        public async Task HandleCommand(string deviceId, WebSocket client, HttpContext httpContext, string message, CancellationToken cancellationToken = default)
        {
            var command = JsonSerializer.Deserialize<DeviceCommand<object>>(message) ?? new DeviceCommand<object>();

            if (handlers.TryGetValue(command.Command, out var handler))
            {
                await handler.Handle(deviceId, message, client, httpContext, cancellationToken);
            }
        }
    }
}
