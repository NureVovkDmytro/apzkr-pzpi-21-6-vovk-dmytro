using Discerniy.Domain.Interface.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;
using System.Text;

namespace Discerniy.Domain.Entity
{
    public class WebSocketConnetion
    {
        public string ConnectionId => Context.TraceIdentifier;
        public WebSocket Socket { get; }
        public HttpContext Context { get; }
        public string UserId { get; }

        public WebSocketConnetion(HttpContext context, WebSocket socket, string userId)
        {
            Socket = socket;
            Context = context;
            UserId = userId;
        }

        public async Task Handle()
        {
            IDeviceWebSocketCommandHandler deviceWebSocketCommandHandler = Context.RequestServices.GetRequiredService<IDeviceWebSocketCommandHandler>();

            while (Socket.State != WebSocketState.Closed && !Context.RequestAborted.IsCancellationRequested)
            {
                var receiveBuffer = new byte[1024];
                var receiveResult = await Socket.ReceiveAsync(receiveBuffer, Context.RequestAborted);
                if (receiveResult.MessageType != WebSocketMessageType.Text || receiveResult.EndOfMessage == false)
                {
                    continue;
                }
                var message = Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);
                await deviceWebSocketCommandHandler.HandleCommand(UserId, Socket, Context, message, Context.RequestAborted);
            }
        }
    }
}
